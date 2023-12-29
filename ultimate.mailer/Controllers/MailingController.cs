using MailKit;
using MailKit.Net.Proxy;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using ultimate.mailer.Extensions;
using ultimate.mailer.Factory;
using ultimate.mailer.Models;
using ultimate.mailer.Views;
using static ultimate.mailer.Models.Log;
using static ultimate.mailer.Models.Server;

namespace ultimate.mailer.Controllers
{
    public class MailingController : IDisposable
    {
        private bool IsDisposed { get; set; }

        private readonly IMailingView m_view;

        private List<Tracker> m_trackers;
        private ConcurrentQueue<Tracker> m_trackersQueue;
        private IDictionary<Task, CancellationTokenSource> m_threads;

        private int m_globalServerIndex;
        private int m_globalProxyIndex;

        public IReadOnlyCollection<Tracker> Trackers { get { return m_trackers.AsReadOnly(); } }

        private event EventHandler<MessagePerformedEventArgs> MessagePerformed;
        private event EventHandler<MessageErrorEventArgs> MessageError;
        private EventWaitHandle m_eventWaitHandlerPaused;


        public MailingController(IMailingView view)
        {
            m_view = view;
            m_view.SetController(this);

            m_trackers = new List<Tracker>();

            foreach (Recipient recipient in Project.Instance.Recipients)
            {
                for (int i = 0; i < Project.Instance.ExtrasBomberAmount; ++i)
                {
                    m_trackers.Add(new Tracker(recipient));
                }
            }

            m_trackersQueue = new ConcurrentQueue<Tracker>(m_trackers);
            m_threads = new Dictionary<Task, CancellationTokenSource>();

            m_globalServerIndex = -1;
            m_globalProxyIndex = -1;

            MessagePerformed += MailingController_MessagePerformed;
            MessageError += MailingController_MessageError;

            LoadView();
        }

        private void LoadView()
        {
            m_view.Title = Project.Instance.Path;
        }

        public void WriteLog(Log log)
        {
            m_view.WriteLog(log);
        }

        public async Task TrackersExportAsync(string filePath, int exportMode)
        {
            string fileExtension = Path.GetExtension(filePath);
            char separator = fileExtension.Equals(".txt") ? ':' : ',';
            await Task.Run(() => ExportTrackers(filePath, separator, exportMode));
        }

        private bool ExportTrackers(string filePath, char separator, int exportMode)
        {
            var stringBuilder = new StringBuilder();

            IEnumerable<Tracker> trackers = new List<Tracker>();

            switch (exportMode)
            {
                case 0 :
                    trackers = m_trackers;
                    break;
                case 5 :
                case 1 :
                    trackers = m_trackers.Where(tracker => !tracker.Performed);
                    break;
                case 6 :
                case 2 :
                    trackers = m_trackers.Where(tracker => tracker.Performed);
                    break;
                case 7 : 
                case 3 :
                    trackers = m_trackers.Where(tracker => tracker.Sent);
                    break;
                case 8 :
                case 4 :
                    trackers = m_trackers.Where(tracker => tracker.Error);
                    break;
            }

            foreach (Tracker tracker in trackers)
            {
                var tempStringBuilder = new StringBuilder();

                if (exportMode < 5)
                {
                    tempStringBuilder.Append(tracker.Identifier);
                    tempStringBuilder.Append(separator);
                }

                if (tracker.Recipient != null)
                    tempStringBuilder.Append(tracker.Recipient.ToString());

                if (exportMode  < 5)
                {
                    tempStringBuilder.Append(separator);

                    if (tracker.Server != null)
                        tempStringBuilder.Append(tracker.Server.ToString());

                    tempStringBuilder.Append(separator);
                    tempStringBuilder.Append("Performed(" + tracker.Performed + ')');
                    tempStringBuilder.Append(separator);
                    tempStringBuilder.Append("Sent(" + tracker.Sent + ')');
                    tempStringBuilder.Append(separator);
                    tempStringBuilder.Append("Error(" + tracker.Error + ')');
                    tempStringBuilder.Append(separator);
                    tempStringBuilder.Append("Response(" + tracker.Response + ')');
                }

                stringBuilder.AppendLine(tempStringBuilder.ToString());
                tempStringBuilder.Clear();
            }

            try
            {
                File.WriteAllText(filePath, stringBuilder.ToString());
            }
            catch (Exception exception)
            {
                MessageBox.Show("An error occured while exporting trackers to file " + Path.GetFileName(filePath) + " : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public void StartExecution()
        {
            int threadsCount = Project.Instance.ExtrasThreadsAmount < m_trackersQueue.Count ? Project.Instance.ExtrasThreadsAmount : m_trackersQueue.Count;

            for (int i = 0; i < threadsCount; ++i)
            {
                var cancellationTokenSource = new CancellationTokenSource();
                Task task = Task.Factory.StartNew(() => StartExecution(cancellationTokenSource.Token),
                    cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                m_threads.Add(task, cancellationTokenSource);
            }
        }

        private void MailingController_MessagePerformed(object sender, MessagePerformedEventArgs e)
        {
            e.Tracker.Performed = true;
            e.Tracker.Identifier = e.Identifier;
            e.Tracker.Server = e.Server;

            new ReportManager(e.Tracker.Server).Notify(ReportManager.NOTIFICATION.PERFORMED);
        }

        private void SmtpClient_MessageSent(object sender, MessageSentEventArgs e)
        {
            Tracker tracker = m_trackers.FirstOrDefault(trck => trck.Identifier == e.Message.MessageId);

            if (tracker != null)
            {
                tracker.Sent = true;
                tracker.Response = e.Response;

                new ReportManager(tracker.Server).Notify(ReportManager.NOTIFICATION.SENT);
            }
        }

        private void MailingController_MessageError(object sender, MessageErrorEventArgs e)
        {
            e.Tracker.Error = true;
            e.Tracker.Response = e.Exception.Message;

            WriteLog(new Log(VERBOSE.ERROR, e.Tracker.Server.ToString() + " An error occurred while sending the message : " + e.Exception.Message));
            new ReportManager(e.Tracker.Server).Notify(ReportManager.NOTIFICATION.ERROR);
        }

        private IDictionary<Server, SmtpClient> InitializeServers()
        {
            IDictionary<Server, SmtpClient> serverDictionary = new Dictionary<Server, SmtpClient>();

            foreach (Server server in Project.Instance.Servers)
            {
                var smtpClient = new SmtpClient();

                if (!Properties.Settings.Default.SettingServersCertificate)
                {
                    smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                }

                smtpClient.SslProtocols = server.ConnectionGeneralProtocol;
                smtpClient.Timeout = server.SettingsGeneralTimeout;

                smtpClient.MessageSent += SmtpClient_MessageSent;
                smtpClient.Connected += server.SmtpClient_Connected;
                smtpClient.Disconnected += server.SmtpClient_Disconnected;

                serverDictionary.Add(server, smtpClient);
            }

            return serverDictionary;
        }

        private IList<ProxyClient> InitializeProxies()
        {
            IList<ProxyClient> proxies = new List<ProxyClient>();

            foreach (Proxy proxy in Project.Instance.Proxies)
            {
                ProxyClient proxyClient = null;
                var networkCredential = new NetworkCredential(proxy.AuthenticationUsername, proxy.AuthenticationPassword);

                switch (proxy.GeneralType)
                {
                    case Proxy.TYPE.HTTP:
                        proxyClient = proxy.Authentication ? new HttpProxyClient(proxy.GeneralHost, proxy.GeneralPort, networkCredential) : new HttpProxyClient(proxy.GeneralHost, proxy.GeneralPort);
                        break;
                    case Proxy.TYPE.SOCKS4:
                        proxyClient = proxy.Authentication ? new Socks4Client(proxy.GeneralHost, proxy.GeneralPort, networkCredential) : new Socks4Client(proxy.GeneralHost, proxy.GeneralPort);
                        break;
                    case Proxy.TYPE.SOCKS4a:
                        proxyClient = proxy.Authentication ? new Socks4aClient(proxy.GeneralHost, proxy.GeneralPort, networkCredential) : new Socks4aClient(proxy.GeneralHost, proxy.GeneralPort);
                        break;
                    case Proxy.TYPE.SOCKS5:
                        proxyClient = proxy.Authentication ? new Socks5Client(proxy.GeneralHost, proxy.GeneralPort, networkCredential) : new Socks5Client(proxy.GeneralHost, proxy.GeneralPort);
                        break;
                }

                proxies.Add(proxyClient);
            }

            return proxies;
        }

        private void StartExecution(CancellationToken cancellationToken)
        {
            int threadID = Thread.CurrentThread.ManagedThreadId;
            WriteLog(new Log(VERBOSE.INFO, "[Thread n°" + threadID + "] This thread has been started."));

            IDictionary<Server, SmtpClient> servers = InitializeServers();
            IList<ProxyClient> proxies = InitializeProxies();

            int serverIndex = -1;
            int proxyIndex = -1;

            int serverMessageCount = 0;
            int proxyMessageCount = 0;

            bool serverRotation = true;
            bool proxyRotation = true;

            while (!cancellationToken.IsCancellationRequested)
            {
                IDictionary<Server, SmtpClient> availableServers = servers.Where(dico => new ReportManager(dico.Key).IsAvailable()).ToDictionary(t => t.Key, t => t.Value);

                if (availableServers.Count == 0)
                {
                    Server nextAvailable = servers.OrderBy(pair => new ReportManager(pair.Key).GetAvailability()).ToList().Last().Key;
                    WriteLog(new Log(VERBOSE.WARNING, "[Thread n°" + threadID + "] No server available for the moment, next available at " + new ReportManager(nextAvailable).GetAvailability() + "."));
                    while (!cancellationToken.IsCancellationRequested && new ReportManager(nextAvailable).GetAvailability() > DateTime.Now) { Thread.Sleep(1000); }
                    continue;
                }

                if (serverRotation)
                {
                    switch (Project.Instance.ExtrasServersSelection)
                    {
                        case Project.SELECTION.ASYNCHRONICALLY:
                            serverIndex = Interlocked.Increment(ref m_globalServerIndex) % availableServers.Count;
                            break;
                        case Project.SELECTION.SYNCHRONICALLY:
                            serverIndex = (++serverIndex) % availableServers.Count;
                            break;
                        case Project.SELECTION.RANDOMLY:
                            serverIndex = new Random().Next(availableServers.Count);
                            break;
                    }

                    Thread.Sleep(Project.Instance.ExtrasServersDelay);

                    serverMessageCount = 0;
                    serverRotation = false;
                }

                Server server = availableServers.ElementAt(serverIndex).Key;
                SmtpClient smtpClient = availableServers.ElementAt(serverIndex).Value;

                availableServers.Clear();

                if (proxyRotation)
                {
                    if (server.SettingsAdvancedProxy && proxies.Count > 0)
                    {
                        switch (Project.Instance.ExtrasProxiesSelection)
                        {
                            case Project.SELECTION.ASYNCHRONICALLY:
                                proxyIndex = Interlocked.Increment(ref m_globalProxyIndex) % proxies.Count;
                                break;
                            case Project.SELECTION.SYNCHRONICALLY:
                                proxyIndex = (++proxyIndex) % proxies.Count;
                                break;
                            case Project.SELECTION.RANDOMLY:
                                proxyIndex = new Random().Next(proxies.Count);
                                break;
                        }

                        smtpClient.ProxyClient = proxies[proxyIndex];
                        Thread.Sleep(Project.Instance.ExtrasProxiesDelay);
                    }

                    proxyMessageCount = 0;
                    proxyRotation = false;
                }

                if (!smtpClient.IsConnected)
                {
                    try
                    {
                        smtpClient.Connect(server.ConnectionGeneralHost, server.ConnectionGeneralPort, SecureSocketOptions.Auto, cancellationToken);
                    }
                    catch (ProxyProtocolException exception)
                    {
                        WriteLog(new Log(VERBOSE.ERROR, "[Thread n°" + threadID + "] " + server.ToString() + " Unable to connect on proxy server : " + exception.Message));
                        proxyRotation = true;
                        continue;
                    }
                    catch (Exception exception)
                    {
                        WriteLog(new Log(VERBOSE.ERROR, "[Thread n°" + threadID + "] " + server.ToString() + " Unable to connect on SMTP server : " + exception.Message));
                        proxyRotation = true;
                        serverRotation = true;
                        continue;
                    }

                    if (!smtpClient.IsConnected)
                    {
                        WriteLog(new Log(VERBOSE.ERROR, "[Thread n°" + threadID + "] " + server.ToString() + " Unable to connect on SMTP server."));
                        proxyRotation = true;
                        serverRotation = true;
                        continue;
                    }

                    WriteLog(new Log(VERBOSE.SUCCESS, "[Thread n°" + threadID + "] " + server.ToString() + " Successfully connected on SMTP server."));
                }

                if (server.ConnectionAuthentication && !smtpClient.IsAuthenticated)
                {
                    try
                    {
                        smtpClient.Authenticate(server.ConnectionAuthenticationUsername, server.ConnectionAuthenticationPassword, cancellationToken);
                    }
                    catch (Exception exception)
                    {
                        WriteLog(new Log(VERBOSE.ERROR, "[Thread n°" + threadID + "] " + server.ToString() + " Unable to authenticate on SMTP server : " + exception.Message));
                        serverRotation = true;
                        continue;
                    }

                    if (!smtpClient.IsAuthenticated)
                    {
                        WriteLog(new Log(VERBOSE.ERROR, "[Thread n°" + threadID + "] " + server.ToString() + " Unable to authenticate on SMTP server."));
                        serverRotation = true;
                        continue;
                    }

                    WriteLog(new Log(VERBOSE.SUCCESS, "[Thread n°" + threadID + "] " + server.ToString() + " Successfully authenticated on SMTP server."));
                }

                while (m_trackersQueue.TryDequeue(out Tracker tracker))
                {
                    MessageFactory messageFactory = new MimeMessageFactory(server, tracker.Recipient);
                    MimeMessage message = messageFactory.GetMimeMessage();

                    MessagePerformed?.Invoke(this, new MessagePerformedEventArgs(tracker, message.MessageId, server));

                    bool messageSent = false;

                    try
                    {
                        smtpClient.Send(message, cancellationToken);
                        messageSent = true;
                    }
                    catch (Exception exception)
                    {
                        MessageError?.Invoke(this, new MessageErrorEventArgs(tracker, exception));
                        messageSent = false;
                    }

                    message = null;

                    Thread.Sleep(server.SettingsGeneralDelay);

                    ++serverMessageCount;
                    ++proxyMessageCount;

                    if (!messageSent)
                    {
                        serverRotation = true;
                    }

                    if (serverMessageCount == Project.Instance.ExtrasServersRotation && servers.Count > 1)
                    {
                        WriteLog(new Log(VERBOSE.INFO, "[Thread n°" + threadID + "] " + server.ToString() + " The amount of message sent before server rotation, has been reached."));
                        serverRotation = true;
                    }

                    if (proxyMessageCount == Project.Instance.ExtrasProxiesRotation && server.SettingsAdvancedProxy && proxies.Count > 1)
                    {
                        WriteLog(new Log(VERBOSE.INFO, "[Thread n°" + threadID + "] " + server.ToString() + " The amount of message sent before proxy rotation, has been reached."));
                        proxyRotation = true;
                    }

                    if (server.Lock != LOCK.NONE)
                    {
                        WriteLog(new Log(VERBOSE.WARNING, "[Thread n°" + threadID + "] " + server.ToString() + " This SMTP server has reached one of its sending limits."));
                        serverRotation = true;
                    }

                    if (serverRotation || proxyRotation)
                    {
                        if (smtpClient.IsConnected)
                        {
                            try
                            {
                                smtpClient.Disconnect(true, cancellationToken);
                            }
                            catch (Exception exception)
                            {
                                WriteLog(new Log(VERBOSE.ERROR, "[Thread n°" + threadID + "] " + server.ToString() + " Unable to disconnect from SMTP server : " + exception.Message));
                                break;
                            }

                            if (smtpClient.IsConnected)
                            {
                                WriteLog(new Log(VERBOSE.ERROR, "[Thread n°" + threadID + "] " + server.ToString() + " Unable to disconnect from the SMTP server."));
                                break;
                            }

                            WriteLog(new Log(VERBOSE.SUCCESS, "[Thread n°" + threadID + "] " + server.ToString() + " Successfully disconnected from the SMTP server."));
                        }

                        break;
                    }

                    if (!smtpClient.IsConnected)
                    {
                        WriteLog(new Log(VERBOSE.WARNING, "[Thread n°" + threadID + "] " + server.ToString() + " This SMTP server has been unfortunately disconnected."));
                        break;
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }

                if (m_trackersQueue.IsEmpty)
                {
                    break;
                }
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                Task.Delay(2000);
            }

            foreach (var pair in servers)
            {
                if (pair.Value.IsConnected)
                {
                    try
                    {
                        pair.Value.Disconnect(true);
                    }
                    catch (Exception exception)
                    {
                        WriteLog(new Log(VERBOSE.ERROR, "[Thread n°" + threadID + "] " + pair.Key.ToString() + " Unable to disconnect from SMTP server : " + exception.Message));
                    }

                    if (!pair.Value.IsConnected)
                    {
                        WriteLog(new Log(VERBOSE.SUCCESS, "[Thread n°" + threadID + "] " + pair.Key.ToString() + " Successfully disconnected from the SMTP server."));
                    }
                }

                pair.Value.Dispose();
            }

            servers.Clear();
            servers = null;

            proxies.Clear();
            proxies = null;

            WriteLog(new Log(VERBOSE.INFO, "[Thread n°" + threadID + "] This thread has been " + (cancellationToken.IsCancellationRequested ? "cancelled" : "completed") + "."));
        }

        public void StopExecution()
        {
            WriteLog(new Log(VERBOSE.INFO, "Canceling sending threads in progress, please wait a moment..."));

            foreach (var task in m_threads)
            {
                try
                {
                    task.Value.Cancel(true);
                }
                catch (Exception exception)
                {
                    WriteLog(new Log(VERBOSE.ERROR, "An error occured while trying to cancel background thread execution : " + exception.Message));
                }
            }

            try
            {
                Task.WaitAll(m_threads.Keys.ToArray());
                WriteLog(new Log(VERBOSE.SUCCESS, "All threads have been successfully canceled."));
            }
            catch (OperationCanceledException)
            {
                WriteLog(new Log(VERBOSE.ERROR, "An error occured while trying to cancel background thread execution."));
            }

            return;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MailingController()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    foreach (KeyValuePair<Task, CancellationTokenSource> keyValuePair in m_threads)
                    {
                        keyValuePair.Key.Dispose();
                        keyValuePair.Value.Dispose();
                    }

                    m_threads.Clear();
                    m_threads = null;

                    m_trackers.Clear();
                    m_trackers.TrimExcess();
                    m_trackers = null;

                    m_trackersQueue.Clear();
                    m_trackersQueue = null;

                    MessagePerformed = null;
                    MessageError = null;
                }

                IsDisposed = true;
            }
        }
    }
}
