using DnsClient;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Proxy;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

using ultimate.mailer.Extensions;
using ultimate.mailer.Factory;
using ultimate.mailer.Models;
using ultimate.mailer.Views;
using static System.Windows.Forms.ListView;
using static ultimate.mailer.Models.PostmarkResponse;

namespace ultimate.mailer.Controllers
{
    public class MainController : IDisposable
    {
        private readonly IMainView m_view;

        private readonly List<string> m_previewMailboxAttachments;

        private readonly List<Rule> m_checkupMessageRules;
        private List<Inspection> m_checkupInspections;

        private int m_previewRecipientSelectedIndex;
        private Recipient m_previewRecipientSelected;
        private int m_checkupMessageServerSelectedIndex;
        private Server m_checkupMessageServerSelected;

        private bool IsDisposed { get; set; }

        public bool AllowModelsUpdate { get; private set; }

        public Server SelectedServer { get; private set; }

        public Proxy SelectedProxy { get; private set; }

        public Recipient SelectedRecipient { get; private set; }

        public ReadOnlyCollection<string> PreviewMailboxAttachments { get { return m_previewMailboxAttachments.AsReadOnly(); } }

        public ReadOnlyCollection<Rule> CheckupMessageRules { get { return m_checkupMessageRules.AsReadOnly(); } }

        public ReadOnlyCollection<Inspection> CheckupInspections { get { return m_checkupInspections.AsReadOnly(); } }

        public MainController(IMainView view) //OK
        {
            m_view = view;
            m_view.SetController(this);

            m_previewMailboxAttachments = new List<string>();

            m_checkupMessageRules = new List<Rule>();
            m_checkupInspections = new List<Inspection>();

            m_previewRecipientSelectedIndex = -1;
            m_previewRecipientSelected = null;
            m_checkupMessageServerSelectedIndex = -1;
            m_checkupMessageServerSelected = null;

            AllowModelsUpdate = true;

            SelectedServer = null;
            SelectedProxy = null;
            SelectedRecipient = null;

            LoadView();
        }

        private void LoadView() //OK
        {
            AllowModelsUpdate = false;

            m_view.Title = Project.Instance.Path;

            m_view.ServersCount = Project.Instance.Servers.Count;

            m_view.ProxiesCount = Project.Instance.Proxies.Count;

            m_view.RecipientsCount = Project.Instance.Recipients.Count;
            m_view.RecipientsFieldsCount = Project.Instance.RecipientFieldCount;

            m_view.SubjectsCount = Project.Instance.Subjects.Count;

            m_view.HeaderGeneralIdentifier = Project.Instance.HeaderGeneralIdentifier;
            m_view.HeaderGeneralDateValue = Project.Instance.HeaderGeneralDateValue;
            m_view.HeaderGeneralDate = Project.Instance.HeaderGeneralDate;

            m_view.HeaderFrom = Project.Instance.HeaderFrom;
            m_view.HeaderFromName = Project.Instance.HeaderFromName;
            m_view.HeaderFromEmail = Project.Instance.HeaderFromEmail;

            m_view.HeaderReplyTo = Project.Instance.HeaderReplyTo;
            m_view.HeaderReplyToName = Project.Instance.HeaderReplyToName;
            m_view.HeaderReplyToEmail = Project.Instance.HeaderReplyToEmail;

            m_view.HeaderListUnsubscribe = Project.Instance.HeaderListUnsubscribe;
            m_view.HeaderListUnsubscribeEmail = Project.Instance.HeaderListUnsubscribeEmail;
            m_view.HeaderListUnsubscribeUrl = Project.Instance.HeaderListUnsubscribeUrl;

            m_view.HeaderAdvancedReturnPath = Project.Instance.HeaderAdvancedReturnPath;
            m_view.HeaderAdvancedImportance = Project.Instance.HeaderAdvancedImportance;
            m_view.HeaderAdvancedPriority = Project.Instance.HeaderAdvancedPriority;

            m_view.MessageBodyHtml = Project.Instance.MessageBodyHtml;
            m_view.MessageBody = Project.Instance.MessageBody;

            m_view.MessageAttachmentsCount = Project.Instance.MessageAttachments.Count;

            m_view.ExtrasThreadsAmount = Project.Instance.ExtrasThreadsAmount;

            m_view.ExtrasServersSelection = Project.Instance.ExtrasServersSelection;
            m_view.ExtrasServersRotation = Project.Instance.ExtrasServersRotation;
            m_view.ExtrasServersDelay = Project.Instance.ExtrasServersDelay;

            m_view.ExtrasProxiesSelection = Project.Instance.ExtrasProxiesSelection;
            m_view.ExtrasProxiesRotation = Project.Instance.ExtrasProxiesRotation;
            m_view.ExtrasProxiesDelay = Project.Instance.ExtrasProxiesDelay;

            m_view.ExtrasBomberAmount = Project.Instance.ExtrasBomberAmount;

            SelectedCheckupMessageServerUpdate();
            SelectedPreviewRecipientUpdate();

            AllowModelsUpdate = true;
        }

        public void ProjectNew() //OK
        {
            Project.Instance.Path = "untilted.umproj";

            Project.Instance.Servers.Clear();
            Project.Instance.Servers.TrimExcess();

            Project.Instance.Proxies.Clear();
            Project.Instance.Proxies.TrimExcess();

            Project.Instance.Recipients.Clear();
            Project.Instance.Recipients.TrimExcess();

            Project.Instance.RecipientFieldCount = 0;

            Project.Instance.Subjects.Clear();
            Project.Instance.Subjects.TrimExcess();

            Project.Instance.HeaderGeneralIdentifier = "{SERVER_HOST}";
            Project.Instance.HeaderGeneralDateValue = DateTime.Now;
            Project.Instance.HeaderGeneralDate = false;

            Project.Instance.HeaderFrom = false;
            Project.Instance.HeaderFromName = string.Empty;
            Project.Instance.HeaderFromEmail = string.Empty;

            Project.Instance.HeaderReplyTo = false;
            Project.Instance.HeaderReplyToName = string.Empty;
            Project.Instance.HeaderReplyToEmail = string.Empty;

            Project.Instance.HeaderListUnsubscribe = false;
            Project.Instance.HeaderListUnsubscribeEmail = string.Empty;
            Project.Instance.HeaderListUnsubscribeUrl = string.Empty;

            Project.Instance.HeaderAdvancedReturnPath = string.Empty;
            Project.Instance.HeaderAdvancedPriority = MessagePriority.Normal;
            Project.Instance.HeaderAdvancedImportance = MessageImportance.Normal;

            Project.Instance.MessageBodyHtml = false;
            Project.Instance.MessageBody = string.Empty;

            Project.Instance.MessageAttachments.Clear();
            Project.Instance.MessageAttachments.TrimExcess();

            Project.Instance.ExtrasThreadsAmount = 1;

            Project.Instance.ExtrasServersSelection = Project.SELECTION.ASYNCHRONICALLY;
            Project.Instance.ExtrasServersRotation = 1000;
            Project.Instance.ExtrasServersDelay = 100;

            Project.Instance.ExtrasProxiesSelection = Project.SELECTION.ASYNCHRONICALLY;
            Project.Instance.ExtrasProxiesRotation = 1000;
            Project.Instance.ExtrasProxiesDelay = 100;

            Project.Instance.ExtrasBomberAmount = 1;

            LoadView();
        }

        public bool ProjectOpen(string filePath) //OK
        {
            ProjectNew();

            var document = new XmlDocument();

            try
            {
                document.Load(filePath);
                XmlNode nodeProject = document.SelectSingleNode("project");
                Project.Instance.Deserialize(document, nodeProject);
            }
            catch (Exception exception)
            {
                MessageBox.Show("An error occured while opening project " + Path.GetFileName(filePath) + " : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Project.Instance.Path = filePath;
            LoadView();

            return true;
        }

        public bool ProjectSave(string filePath) //OK
        {
            var document = new XmlDocument();

            try
            {
                XmlNode projectNode = document.CreateElement("project");
                document.AppendChild(projectNode);
                Project.Instance.Serialize(document, projectNode);
                document.Save(filePath);
            }
            catch (Exception exception)
            {
                MessageBox.Show("An error occured while saving project " + filePath + " : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Project.Instance.Path = filePath;
            m_view.Title = Project.Instance.Path;

            return true;
        }

        public void ServersAdd() //OK
        {
            Project.Instance.Servers.Add(new Server());
            m_view.ServersCount = Project.Instance.Servers.Count;
            SelectedCheckupMessageServerUpdate();
        }

        public void ServersRemove() //OK
        {
            if (SelectedServer != null)
            {
                Project.Instance.Servers.Remove(SelectedServer);
                m_view.ServersCount = Project.Instance.Servers.Count;
                SelectedCheckupMessageServerUpdate();
            }
        }

        public void ServersClear() //OK
        {
            m_view.ServersCount = 0;
            Project.Instance.Servers.Clear();
            Project.Instance.Servers.TrimExcess();
            SelectedCheckupMessageServerUpdate();
        }

        public async Task ServersImportAsync(string filePath) //OK
        {
            string fileExtension = Path.GetExtension(filePath);
            char separator = fileExtension.Equals(".txt") ? ':' : ',';

            IList<Server> servers = null;
            Task<bool> importTask = Task.Run(() => ImportServers(filePath, separator, out servers));

            if (await importTask)
            {
                Project.Instance.Servers.AddRange(servers);
                m_view.ServersCount = Project.Instance.Servers.Count;
                SelectedCheckupMessageServerUpdate();
            }
        }

        public async Task ServersExportAsync(string filePath) //OK
        {
            string fileExtension = Path.GetExtension(filePath);
            char separator = fileExtension.Equals(".txt") ? ':' : ',';
            await Task.Run(() => ExportServers(filePath, separator));
        }

        public async Task ServersValidationAsync() //OK
        {
            if (!await Utils.IsNetworkAvailable())
            {
                MessageBox.Show("No internet connection, please check your network.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await Task.Factory.StartNew(() => Parallel.ForEach(Project.Instance.Servers, server => server.IsValidated = null));
            await Task.Factory.StartNew(() => Parallel.ForEach(Project.Instance.Servers, server => server.IsValidated = ServerValidation(server)));

            Project.Instance.Servers.RemoveAll(server => server.IsValidated == false);
            m_view.ServersCount = Project.Instance.Servers.Count;
            SelectedCheckupMessageServerUpdate();
        }

        private bool ServerValidation(Server server) //OK
        {
            if (string.IsNullOrWhiteSpace(server.ConnectionGeneralHost))
            {
                return false;
            }

            if (server.ConnectionAuthentication && (string.IsNullOrWhiteSpace(server.ConnectionAuthenticationUsername) || string.IsNullOrWhiteSpace(server.ConnectionAuthenticationPassword)))
            {
                return false;
            }

            using (var smtpClient = new SmtpClient())
            {
                if (!Properties.Settings.Default.SettingServersCertificate)
                {
                    smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                }

                smtpClient.SslProtocols = server.ConnectionGeneralProtocol;
                smtpClient.Timeout = server.SettingsGeneralTimeout;

                try
                {
                    smtpClient.Connect(server.ConnectionGeneralHost, server.ConnectionGeneralPort, SecureSocketOptions.Auto);
                    if (server.ConnectionAuthentication) smtpClient.Authenticate(server.ConnectionAuthenticationUsername, server.ConnectionAuthenticationPassword);
                    else if (Properties.Settings.Default.SettingServersAuthentication && smtpClient.Capabilities.HasFlag(SmtpCapabilities.Authentication)) throw new AuthenticationException();
                }
                catch
                {
                    return false;
                }
                finally
                {
                    smtpClient.Disconnect(true);
                }
            }

            return true;
        }

        public void ServersSelectionChanged(int selectedIndex) //OK
        {
            SelectedServer = selectedIndex != -1 ? Project.Instance.Servers[selectedIndex] : null;

            if (SelectedServer != null)
            {
                UpdateServerView();
            }
        }

        private void UpdateServerView() //OK
        {
            AllowModelsUpdate = false;

            m_view.ServerConnectionGeneralHost = SelectedServer.ConnectionGeneralHost;
            m_view.ServerConnectionGeneralPort = SelectedServer.ConnectionGeneralPort;
            m_view.ServerConnectionGeneralProtocol = SelectedServer.ConnectionGeneralProtocol;

            m_view.ServerConnectionAuthentication = SelectedServer.ConnectionAuthentication;
            m_view.ServerConnectionAuthenticationUsername = SelectedServer.ConnectionAuthenticationUsername;
            m_view.ServerConnectionAuthenticationPassword = SelectedServer.ConnectionAuthenticationPassword;

            m_view.ServerSettingsGeneralTimeout = SelectedServer.SettingsGeneralTimeout;
            m_view.ServerSettingsGeneralDelay = SelectedServer.SettingsGeneralDelay;

            m_view.ServerSettingsSenderName = SelectedServer.SettingsSenderName;
            m_view.ServerSettingsSenderEmail = SelectedServer.SettingsSenderEmail;

            m_view.ServerSettingsLimitSession = SelectedServer.SettingsLimitSession;
            m_view.ServerSettingsLimitSessionValue = SelectedServer.SettingsLimitSessionValue;
            m_view.ServerSettingsLimitHourly = SelectedServer.SettingsLimitHourly;
            m_view.ServerSettingsLimitHourlyValue = SelectedServer.SettingsLimitHourlyValue;
            m_view.ServerSettingsLimitDaily = SelectedServer.SettingsLimitDaily;
            m_view.ServerSettingsLimitDailyValue = SelectedServer.SettingsLimitDailyValue;
            m_view.ServerSettingsLimitMonthly = SelectedServer.SettingsLimitMonthly;
            m_view.ServerSettingsLimitMonthlyValue = SelectedServer.SettingsLimitMonthlyValue;

            m_view.ServerSettingsAdvancedSessionDelay = SelectedServer.SettingsAdvancedSessionDelay;
            m_view.ServerSettingsAdvancedPing = SelectedServer.SettingsAdvancedPing;
            m_view.ServerSettingsAdvancedPingValue = SelectedServer.SettingsAdvancedPingValue;
            m_view.ServerSettingsAdvancedProxy = SelectedServer.SettingsAdvancedProxy;

            var reportManager = new ReportManager(SelectedServer);

            reportManager.Update();

            m_view.ServerReportsAvailable = reportManager.IsAvailable();
            m_view.ServerReportsAvailability = reportManager.GetAvailability();
            m_view.ServerReportsPerformedCount = reportManager.PerformedCount;
            m_view.ServerReportsSentCount = reportManager.SentCount;
            m_view.ServerReportsErrorCount = reportManager.ErrorCount;
            m_view.ServerReportsHourlyCount = reportManager.HourlyCount;
            m_view.ServerReportsDailyCount = reportManager.DailyCount;
            m_view.ServerReportsMonthlyCount = reportManager.MonthlyCount;
            m_view.ServerReportsCapabilities = reportManager.GetCapabilities();

            AllowModelsUpdate = true;
        }

        public async Task<bool> ServerConnectionTestAsync() //OK
        {
            var server = SelectedServer;
            server.IsTested = true;

            if (string.IsNullOrWhiteSpace(server.ConnectionGeneralHost))
            {
                MessageBox.Show(server.ToString() + " Please fill 'Host' field in Servers > Connection > General panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                server.IsTested = false;
                return false;
            }

            if (server.ConnectionAuthentication)
            {
                if (string.IsNullOrWhiteSpace(server.ConnectionAuthenticationUsername))
                {
                    MessageBox.Show(server.ToString() + " Please fill 'Username' field in Servers > Connection > Authentication panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    server.IsTested = false;
                    return false;
                }

                if (string.IsNullOrWhiteSpace(server.ConnectionAuthenticationPassword))
                {
                    MessageBox.Show(server.ToString() + " Please fill 'Password' field in Servers > Connection > Authentication panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    server.IsTested = false;
                    return false;
                }
            }

            if (!await Utils.IsNetworkAvailable())
            {
                MessageBox.Show("No internet connection, please check your network.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                server.IsTested = false;
                return false;
            }

            using (var smtpClient = new SmtpClient())
            {
                if (!Properties.Settings.Default.SettingServersCertificate)
                {
                    smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                }

                smtpClient.SslProtocols = server.ConnectionGeneralProtocol;
                smtpClient.Timeout = server.SettingsGeneralTimeout;

                if (!smtpClient.IsConnected)
                {
                    try
                    {
                        await smtpClient.ConnectAsync(server.ConnectionGeneralHost, server.ConnectionGeneralPort, SecureSocketOptions.Auto);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(server.ToString() + " Unable to connect on SMTP server : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        server.IsTested = false;
                        return false;
                    }

                    if (!smtpClient.IsConnected)
                    {
                        MessageBox.Show(server.ToString() + " Unable to connect on SMTP server.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        server.IsTested = false;
                        return false;
                    }
                }

                if (server.ConnectionAuthentication && !smtpClient.IsAuthenticated)
                {
                    try
                    {
                        await smtpClient.AuthenticateAsync(server.ConnectionAuthenticationUsername, server.ConnectionAuthenticationPassword);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(server.ToString() + " Unable to authenticate on SMTP server : " + e.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        try
                        {
                            await smtpClient.DisconnectAsync(true);
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(server.ToString() + " Unable to disconnect from SMTP server : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        server.IsTested = false;
                        return false;
                    }

                    if (!smtpClient.IsAuthenticated)
                    {
                        MessageBox.Show(server.ToString() + " Unable to authenticate on SMTP server.", "Error - Ultimate Mailer", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        try
                        {
                            await smtpClient.DisconnectAsync(true);
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(server.ToString() + " Unable to disconnect from SMTP server : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        server.IsTested = false;
                        return false;
                    }
                }
                else if (Properties.Settings.Default.SettingServersAuthentication && smtpClient.Capabilities.HasFlag(SmtpCapabilities.Authentication))
                {
                    MessageBox.Show(server.ToString() + " This server is protected with an authentication mechanism.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    try
                    {
                        await smtpClient.DisconnectAsync(true);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(server.ToString() + " Unable to disconnect from SMTP server : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    server.IsTested = false;
                    return false;
                }

                try
                {
                    await smtpClient.DisconnectAsync(true);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(server.ToString() + " Unable to disconnect from SMTP server : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    server.IsTested = false;
                    return false;
                }
            }

            MessageBox.Show(server.ToString() + " This SMTP server have been successfully tested.", "Information - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Information);

            server.IsTested = false;
            return true;
        }

        public void ServerReportsRefresh() //OK
        {
            var reportManager = new ReportManager(SelectedServer);
            reportManager.Update();

            m_view.ServerReportsAvailable = reportManager.IsAvailable();
            m_view.ServerReportsAvailability = reportManager.GetAvailability();
            m_view.ServerReportsPerformedCount = reportManager.PerformedCount;
            m_view.ServerReportsSentCount = reportManager.SentCount;
            m_view.ServerReportsErrorCount = reportManager.ErrorCount;
            m_view.ServerReportsHourlyCount = reportManager.HourlyCount;
            m_view.ServerReportsDailyCount = reportManager.DailyCount;
            m_view.ServerReportsMonthlyCount = reportManager.MonthlyCount;
            m_view.ServerReportsCapabilities = reportManager.GetCapabilities();
        }

        public void ProxiesAdd() //OK
        {
            Project.Instance.Proxies.Add(new Proxy());
            m_view.ProxiesCount = Project.Instance.Proxies.Count;
        }

        public void ProxiesRemove() //OK
        {
            if (SelectedProxy != null)
            {
                Project.Instance.Proxies.Remove(SelectedProxy);
                m_view.ProxiesCount = Project.Instance.Proxies.Count;
            }
        }

        public void ProxiesClear() //OK
        {
            Project.Instance.Proxies.Clear();
            Project.Instance.Proxies.TrimExcess();
            m_view.ProxiesCount = 0;
        }

        public async Task ProxiesExportAsync(string filePath) //OK
        {
            string fileExtension = Path.GetExtension(filePath);
            char separator = fileExtension.Equals(".txt") ? ':' : ',';
            await Task.Run(() => ExportProxies(filePath, separator));
        }

        public async Task ProxiesImportAsync(string filePath) //OK
        {
            string fileExtension = Path.GetExtension(filePath);
            char separator = fileExtension.Equals(".txt") ? ':' : ',';

            IList<Proxy> proxies = null;
            Task<bool> importTask = Task.Run(() => ImportProxies(filePath, separator, out proxies));

            if (await importTask)
            {
                Project.Instance.Proxies.AddRange(proxies);
                m_view.ProxiesCount = Project.Instance.Proxies.Count;
            }
        }

        public async Task ProxiesValidationAsync() //OK
        {
            if (!await Utils.IsNetworkAvailable())
            {
                MessageBox.Show("No internet connection, please check your network.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await Task.Factory.StartNew(() => Parallel.ForEach(Project.Instance.Proxies, proxy => proxy.IsValidated = null));
            await Task.Factory.StartNew(() => Parallel.ForEach(Project.Instance.Proxies, proxy => proxy.IsValidated = ProxyValidation(proxy)));

            Project.Instance.Proxies.RemoveAll(proxy => proxy.IsValidated == false);
            m_view.ProxiesCount = Project.Instance.Proxies.Count;
        }

        private bool ProxyValidation(Proxy proxy) //OK
        {
            if (string.IsNullOrWhiteSpace(proxy.GeneralHost))
            {
                return false;
            }

            if (proxy.Authentication && (string.IsNullOrWhiteSpace(proxy.AuthenticationUsername) || string.IsNullOrWhiteSpace(proxy.AuthenticationPassword)))
            {
                return false;
            }

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

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                smtpClient.ProxyClient = proxyClient;
                smtpClient.Timeout = Properties.Settings.Default.SettingProxiesValidationTimeout;

                try
                {
                    smtpClient.Connect(Constantes.SMTP_HOST, Constantes.SMTP_PORT, SecureSocketOptions.Auto);
                    smtpClient.Disconnect(true);
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> ProxyConnectionTestAsync() //OK
        {
            var proxy = SelectedProxy;
            proxy.IsTested = true;

            if (string.IsNullOrWhiteSpace(proxy.GeneralHost))
            {
                MessageBox.Show(proxy.ToString() + " Please fill 'Host' field in Proxies > General panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                proxy.IsTested = false;
                return false;
            }

            if (proxy.Authentication)
            {
                if (string.IsNullOrWhiteSpace(proxy.AuthenticationUsername))
                {
                    MessageBox.Show(proxy.ToString() + " Please fill 'Username' field in Proxies > Authentication panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    proxy.IsTested = false;
                    return false;
                }

                if (string.IsNullOrWhiteSpace(proxy.AuthenticationPassword))
                {
                    MessageBox.Show(proxy.ToString() + " Please fill 'Password' field in Proxies > Authentication panel", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    proxy.IsTested = false;
                    return false;
                }
            }

            if (!await Utils.IsNetworkAvailable())
            {
                MessageBox.Show("No internet connection, please check your network.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                proxy.IsTested = false;
                return false;
            }

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

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                smtpClient.ProxyClient = proxyClient;
                smtpClient.Timeout = Properties.Settings.Default.SettingProxiesValidationTimeout;

                try
                {
                    await smtpClient.ConnectAsync(Constantes.SMTP_HOST, Constantes.SMTP_PORT, SecureSocketOptions.Auto);
                    await smtpClient.DisconnectAsync(true);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(proxy.ToString() + " Unable to connect through proxy server : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    proxy.IsTested = false;
                    return false;
                }
            }

            MessageBox.Show(proxy.ToString() + " This proxy server have been successfully tested.", "Information - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Information);
            proxy.IsTested = false;
            return true;
        }

        public void ProxiesSelectionChanged(int selectedIndex) //OK
        {
            SelectedProxy = selectedIndex != -1 ? Project.Instance.Proxies[selectedIndex] : null;

            if (SelectedProxy != null)
            {
                UpdateProxyView();
            }
        }

        private void UpdateProxyView() //OK
        {
            AllowModelsUpdate = false;

            m_view.ProxyGeneralType = SelectedProxy.GeneralType;
            m_view.ProxyGeneralHost = SelectedProxy.GeneralHost;
            m_view.ProxyGeneralPort = SelectedProxy.GeneralPort;

            m_view.ProxyAuthentication = SelectedProxy.Authentication;
            m_view.ProxyAuthenticationUsername = SelectedProxy.AuthenticationUsername;
            m_view.ProxyAuthenticationPassword = SelectedProxy.AuthenticationPassword;

            m_view.ProxyTest = !SelectedProxy.IsTested;

            AllowModelsUpdate = true;
        }

        public bool SubjectsAdd(string subject)  //OK
        {
            Project.Instance.Subjects.Add(subject);
            m_view.SubjectsCount = Project.Instance.Subjects.Count;

            return true;
        }

        public void SubjectsRemove(SelectedIndexCollection indexes)  //OK
        {
            if (indexes.Count > 0)
            {
                IOrderedEnumerable<int> selectedIndexes = indexes.Cast<int>().ToList().OrderByDescending(i => i);

                foreach (int index in selectedIndexes)
                {
                    Project.Instance.Subjects.RemoveAt(index);
                }

                m_view.SubjectsCount = Project.Instance.Subjects.Count;
            }
        }

        public void SubjectsClear()  //OK
        {
            Project.Instance.Subjects.Clear();
            Project.Instance.Subjects.TrimExcess();
            m_view.SubjectsCount = Project.Instance.Subjects.Count;
        }

        public async Task ImportSubjectsAsync(string filePath) //OK
        {
            IList<string> subjects = null;
            Task<bool> importTask = Task.Run(() => ImportSubjects(filePath, out subjects)); ;

            if (await importTask)
            {
                Project.Instance.Subjects.AddRange(subjects);
                m_view.SubjectsCount = Project.Instance.Subjects.Count;
            }
        }

        public async Task ExportSubjectsAsync(string filePath)
        {
            await Task.Run(() => ExportSubjects(filePath));
        }

        public void RecipientsAdd(string email) //OK
        {
            Project.Instance.Recipients.Add(new Recipient(email.ToLower(), Project.Instance.RecipientFieldCount));
            m_view.RecipientsCount = Project.Instance.Recipients.Count;
            SelectedPreviewRecipientUpdate();
        }


        public void RecipientsRemove(DataGridViewSelectedRowCollection rowCollection) //OK
        {
            if (rowCollection.Count > 0)
            {
                IOrderedEnumerable<int> selectedIndexes = rowCollection.Cast<DataGridViewRow>().ToList().Select(row => row.Index).ToList().OrderByDescending(i => i);

                foreach (int index in selectedIndexes)
                {
                    Project.Instance.Recipients.RemoveAt(index);
                }

                m_view.RecipientsCount = Project.Instance.Recipients.Count;
                SelectedPreviewRecipientUpdate();
            }
        }

        public void RecipientsClear() //OK
        {
            Project.Instance.Recipients.Clear();
            Project.Instance.Recipients.TrimExcess();
            m_view.RecipientsCount = Project.Instance.Recipients.Count;
            SelectedPreviewRecipientUpdate();
        }

        public async Task RecipientsImportAsync(string filePath) //OK
        {
            string fileExtension = Path.GetExtension(filePath);
            char separator = fileExtension.Equals(".txt") ? ':' : ',';

            IList<Recipient> recipients = null;

            if (await Task.Run(() => ImportRecipients(filePath, separator, out recipients)))
            {
                Project.Instance.Recipients.AddRange(recipients);
                m_view.RecipientsCount = Project.Instance.Recipients.Count;
                SelectedPreviewRecipientUpdate();
            }
        }

        public async Task RecipientsExportAsync(string filePath) //OK
        {
            string fileExtension = Path.GetExtension(filePath);
            char separator = fileExtension.Equals(".txt") ? ':' : ',';
            await Task.Run(() => ExportRecipients(filePath, separator));
        }

        public async Task RecipientsExtractAsync(string filePath)
        {
            string fileContent = string.Empty;

            try
            {
                fileContent = File.ReadAllText(filePath);
            }
            catch (Exception exception)
            {
                MessageBox.Show("An error occured while reading file " + Path.GetFileName(filePath) + " : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> extractedEmails = await Task.Run(() => RecipientsExtract(fileContent));

            void addRecipient(string email)
            {
                if (Utils.IsValidEmail(email)) Project.Instance.Recipients.Add(new Recipient(email.ToLower(), Project.Instance.RecipientFieldCount));
            };

            extractedEmails.ForEach(email => addRecipient(email));
            m_view.RecipientsCount = Project.Instance.Recipients.Count;
            SelectedPreviewRecipientUpdate();
        }

        private List<string> RecipientsExtract(string fileContent)
        {
            var emailRegex = new Regex(Constantes.REGEX_PATTERN_EMAIL, RegexOptions.IgnoreCase);
            MatchCollection emailMatches = emailRegex.Matches(fileContent);
            return emailMatches.Cast<Match>().Select(match => match.Value).ToList();
        }

        public async Task RecipientsSortAsync() //OK
        {
            var taskOrderBy = Task.Run(() => Project.Instance.Recipients.OrderBy(recipient => recipient.Email).ToList());
            Project.Instance.Recipients = await taskOrderBy;
            SelectedPreviewRecipientUpdate();
        }

        public async Task RecipientsDuplicateAsync() //OK
        {
            var taskDistinctBy = Task.Run(() => Project.Instance.Recipients.DistinctBy(recipient => recipient.Email).ToList());
            Project.Instance.Recipients = await taskDistinctBy;
            m_view.RecipientsCount = Project.Instance.Recipients.Count;
            SelectedPreviewRecipientUpdate();
        }

        public async Task RecipientsValidationAsync() //OK
        {
            if (!await Utils.IsNetworkAvailable())
            {
                MessageBox.Show("No internet connection, please check your network.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            await Task.Factory.StartNew(() => Parallel.ForEach(Project.Instance.Recipients, recipient => recipient.IsValidated = null));
            await Task.Factory.StartNew(() => Parallel.ForEach(Project.Instance.Recipients, recipient => recipient.IsValidated = RecipientValidation(recipient)));

            Project.Instance.Recipients.RemoveAll(recipient => recipient.IsValidated == false);
            m_view.RecipientsCount = Project.Instance.Recipients.Count;

            return;
        }

        private bool RecipientValidation(Recipient recipient) //OK
        {
            int verificationLevel = Properties.Settings.Default.SettingRecipientsValidationLevel;

            if (!Utils.IsValidEmail(recipient.Email))
            {
                return false;
            }

            var mailboxAddress = new System.Net.Mail.MailAddress(recipient.Email);

            if (verificationLevel > 0 && Properties.Settings.Default.SettingRecipientsValidationLevelRole && Constantes.ROLE_USERS.Contains(mailboxAddress.User))
            {
                return false;
            }

            if (verificationLevel > 1 && Properties.Settings.Default.SettingRecipientsValidationLevelDisposable && Constantes.DEA_DOMAINS.Contains(mailboxAddress.Host))
            {
                return false;
            }

            if (verificationLevel > 2)
            {
                var dnsLookup = new LookupClient();
                IDnsQueryResponse mxRecords = null;

                try
                {
                    mxRecords = dnsLookup.Query(mailboxAddress.Host, QueryType.MX);
                }
                catch
                {
                    return false;
                }
                finally
                {
                    dnsLookup = null;
                }

                if (mxRecords == null || mxRecords.HasError || mxRecords.Answers.MxRecords().Count() == 0)
                {
                    return false;
                }

                if (verificationLevel > 3)
                {
                    bool verificationLevelSmtp = false;

                    using (var smtpClient = new SmtpClient())
                    {
                        smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        smtpClient.Timeout = Properties.Settings.Default.SettingRecipientsValidationTimeout;

                        foreach (var mxRecord in mxRecords.Answers.MxRecords().OrderBy(mxRecord => mxRecord.Preference))
                        {
                            try
                            {
                                smtpClient.Connect(mxRecord.Exchange, 25);
                            }
                            catch
                            {
                                continue;
                            }

                            if (smtpClient.IsConnected)
                            {
                                verificationLevelSmtp = true;
                                smtpClient.Disconnect(true);
                                break;
                            }
                        }
                    }

                    if (!verificationLevelSmtp)
                    {
                        return false;
                    }
                }
            }

            mailboxAddress = null;
            return true;
        }

        public void RecipientFieldsAdd() //OK
        {
            Project.Instance.Recipients.ForEach(recipient => recipient.Fields.Add(string.Empty));
            Project.Instance.RecipientFieldCount += 1;
            m_view.RecipientsFieldsCount = Project.Instance.RecipientFieldCount;
        }

        public void RecipientFieldsRemove() //OK
        {
            Project.Instance.RecipientFieldCount -= 1;
            m_view.RecipientsFieldsCount = Project.Instance.RecipientFieldCount;
            Project.Instance.Recipients.ForEach(recipient => recipient.Fields.RemoveAt(recipient.Fields.Count - 1));
        }

        public void RecipientsFieldsClear() //OK
        {
            Project.Instance.RecipientFieldCount = 0;
            m_view.RecipientsFieldsCount = Project.Instance.RecipientFieldCount;
            Project.Instance.Recipients.ForEach(recipient => recipient.Fields.Clear());
        }

        public void SelectedRecipientChange(int selectedIndex) //OK
        {
            bool selected = selectedIndex != -1 && selectedIndex < Project.Instance.Recipients.Count;
            SelectedRecipient = selected ? Project.Instance.Recipients[selectedIndex] : null;
        }

        public void MessageBodyOpen(string filePath) //OK
        {
            string fileContent = string.Empty;

            try
            {
                fileContent = File.ReadAllText(filePath);
            }
            catch (Exception exception)
            {
                MessageBox.Show("An error occured while reading file " + Path.GetFileName(filePath) + " : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Project.Instance.MessageBody = fileContent;
            m_view.MessageBody = Project.Instance.MessageBody;
        }

        public void MessageBodySave(string filePath) //OK
        {
            try
            {
                File.WriteAllText(filePath, Project.Instance.MessageBody);
            }
            catch (Exception exception)
            {
                MessageBox.Show("An error occured while writing file " + Path.GetFileName(filePath) + " : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void MessageAttachmentAdd(string filePath) //OK
        {
            Project.Instance.MessageAttachments.Add(filePath);
            m_view.MessageAttachmentsCount = Project.Instance.MessageAttachments.Count;
        }

        public void MessageAttachmentRemove(SelectedIndexCollection indexes) //OK
        {
            if (indexes.Count > 0)
            {
                IOrderedEnumerable<int> selectedIndexes = indexes.Cast<int>().ToList().OrderByDescending(i => i);

                foreach (int index in selectedIndexes)
                {
                    Project.Instance.MessageAttachments.RemoveAt(index);
                }

                m_view.MessageAttachmentsCount = Project.Instance.MessageAttachments.Count;
            }
        }

        public void MessageAttachmentsClear()  //OK
        {
            Project.Instance.MessageAttachments.Clear();
            Project.Instance.MessageAttachments.TrimExcess();
            m_view.MessageAttachmentsCount = Project.Instance.MessageAttachments.Count;
        }

        public void PreviewRecipientFirst() //OK
        {
            m_previewRecipientSelectedIndex = Project.Instance.Recipients.Count > 0 ? 0 : -1;
            m_previewRecipientSelected = m_previewRecipientSelectedIndex != -1 ? Project.Instance.Recipients[m_previewRecipientSelectedIndex] : null;
            m_view.PreviewControlRecipient = m_previewRecipientSelected != null ? m_previewRecipientSelected.ToString() : "-";
        }

        public void PreviewRecipientPrevious() //OK
        {
            m_previewRecipientSelectedIndex -= 1;
            m_previewRecipientSelectedIndex = m_previewRecipientSelectedIndex < 0 ? Project.Instance.Recipients.Count - 1 : m_previewRecipientSelectedIndex;
            m_previewRecipientSelected = m_previewRecipientSelectedIndex != -1 ? Project.Instance.Recipients[m_previewRecipientSelectedIndex] : null;
            m_view.PreviewControlRecipient = m_previewRecipientSelected != null ? m_previewRecipientSelected.ToString() : "-";
        }

        public void PreviewRecipientNext() //OK
        {
            m_previewRecipientSelectedIndex += 1;
            m_previewRecipientSelectedIndex = m_previewRecipientSelectedIndex > Project.Instance.Recipients.Count - 1 ? 0 : m_previewRecipientSelectedIndex;
            m_previewRecipientSelected = m_previewRecipientSelectedIndex != -1 ? Project.Instance.Recipients[m_previewRecipientSelectedIndex] : null;
            m_view.PreviewControlRecipient = m_previewRecipientSelected != null ? m_previewRecipientSelected.ToString() : "-";
        }

        public void PreviewRecipientLast() //OK
        {
            m_previewRecipientSelectedIndex = Project.Instance.Recipients.Count > 0 ? Project.Instance.Recipients.Count - 1 : -1;
            m_previewRecipientSelected = m_previewRecipientSelectedIndex != -1 ? Project.Instance.Recipients[m_previewRecipientSelectedIndex] : null;
            m_view.PreviewControlRecipient = m_previewRecipientSelected != null ? m_previewRecipientSelected.ToString() : "-";
        }

        private void SelectedPreviewRecipientUpdate() //OK
        {
            if (Project.Instance.Recipients.Count == 0)
            {
                m_previewRecipientSelectedIndex = -1;
            }
            else if (m_previewRecipientSelectedIndex == -1 || m_previewRecipientSelected == null)
            {
                m_previewRecipientSelectedIndex = 0;
            }
            else if (Project.Instance.Recipients.IndexOf(m_previewRecipientSelected) == -1 && m_previewRecipientSelectedIndex >= Project.Instance.Recipients.Count)
            {
                m_previewRecipientSelectedIndex = Project.Instance.Recipients.Count - 1;
            }
            else if (Project.Instance.Recipients.IndexOf(m_previewRecipientSelected) != -1 && m_previewRecipientSelectedIndex != Project.Instance.Recipients.IndexOf(m_previewRecipientSelected))
            {
                m_previewRecipientSelectedIndex = Project.Instance.Recipients.IndexOf(m_previewRecipientSelected);
            }

            m_previewRecipientSelected = m_previewRecipientSelectedIndex != -1 ? Project.Instance.Recipients[m_previewRecipientSelectedIndex] : null;
            m_view.PreviewControlRecipient = m_previewRecipientSelected != null ? m_previewRecipientSelected.ToString() : "-";
        }

        public void PreviewRefresh()  //OK
        {
            MessageFactory factory = new MimeMessageFactory(null, m_previewRecipientSelected);
            MimeMessage message = factory.GetMimeMessage();

            m_view.PreviewMailboxHeaderIdentifier = message.MessageId;
            m_view.PreviewMailboxHeaderSubject = message.Subject;
            m_view.PreviewMailboxHeaderSender = message.Sender != null ? message.Sender.ToString() : string.Empty;
            m_view.PreviewMailboxHeaderFrom = message.From.Count != 0 ? message.From[0].ToString() : string.Empty;
            m_view.PreviewMailboxHeaderTo = message.To.Count != 0 ? message.To[0].ToString() : string.Empty;

            m_view.PreviewMailboxBodyText = message.TextBody;
            m_view.PreviewMailboxBodyHtml = message.HtmlBody;

            m_view.PreviewMailboxHeaderDate = message.Date.DateTime;
            m_view.PreviewMailboxMessageSize = GetMessageSize(message, out double messageSize) ? (messageSize / 1024) / 1024 : 0;

            m_view.PreviewMailboxAttachmentsCount = 0;
            m_previewMailboxAttachments.Clear();
            m_previewMailboxAttachments.TrimExcess();
            m_previewMailboxAttachments.AddRange(Project.Instance.MessageAttachments);
            m_view.PreviewMailboxAttachmentsCount = m_previewMailboxAttachments.Count;

            m_view.PreviewOriginal = GetMessageRaw(message, out string rawMessage) ? rawMessage : string.Empty;
        }

        public void CheckupMessageServerFirst() //OK
        {
            m_checkupMessageServerSelectedIndex = 0;
            m_checkupMessageServerSelected = m_checkupMessageServerSelectedIndex != -1 ? Project.Instance.Servers[m_checkupMessageServerSelectedIndex] : null;
            m_view.CheckupMessageControlServer = m_checkupMessageServerSelected != null ? m_checkupMessageServerSelected.ToString() : "-";
        }

        public void CheckupMessageServerPrevious() //OK
        {
            m_checkupMessageServerSelectedIndex -= 1;
            m_checkupMessageServerSelectedIndex = m_checkupMessageServerSelectedIndex < 0 ? Project.Instance.Servers.Count - 1 : m_checkupMessageServerSelectedIndex;
            m_checkupMessageServerSelected = m_checkupMessageServerSelectedIndex != -1 ? Project.Instance.Servers[m_checkupMessageServerSelectedIndex] : null;
            m_view.CheckupMessageControlServer = m_checkupMessageServerSelected != null ? m_checkupMessageServerSelected.ToString() : "-";
        }

        public void CheckupMessageServerNext() //OK
        {
            m_checkupMessageServerSelectedIndex += 1;
            m_checkupMessageServerSelectedIndex = m_checkupMessageServerSelectedIndex > Project.Instance.Servers.Count - 1 ? 0 : m_checkupMessageServerSelectedIndex;
            m_checkupMessageServerSelected = m_checkupMessageServerSelectedIndex != -1 ? Project.Instance.Servers[m_checkupMessageServerSelectedIndex] : null;
            m_view.CheckupMessageControlServer = m_checkupMessageServerSelected != null ? m_checkupMessageServerSelected.ToString() : "-";
        }

        public void CheckupMessageServerLast() //OK
        {
            m_checkupMessageServerSelectedIndex = Project.Instance.Servers.Count - 1;
            m_checkupMessageServerSelected = m_checkupMessageServerSelectedIndex != -1 ? Project.Instance.Servers[m_checkupMessageServerSelectedIndex] : null;
            m_view.CheckupMessageControlServer = m_checkupMessageServerSelected != null ? m_checkupMessageServerSelected.ToString() : "-";
        }

        private void SelectedCheckupMessageServerUpdate() //OK
        {
            if (Project.Instance.Servers.Count == 0)
            {
                m_checkupMessageServerSelectedIndex = -1;
            }
            else if (m_checkupMessageServerSelectedIndex == -1 || m_checkupMessageServerSelected == null)
            {
                m_checkupMessageServerSelectedIndex = 0;
            }
            else if (Project.Instance.Servers.IndexOf(m_checkupMessageServerSelected) == -1 && m_checkupMessageServerSelectedIndex >= Project.Instance.Servers.Count)
            {
                m_checkupMessageServerSelectedIndex = Project.Instance.Servers.Count - 1;
            }
            else if (Project.Instance.Servers.IndexOf(m_checkupMessageServerSelected) != -1 && m_checkupMessageServerSelectedIndex != Project.Instance.Servers.IndexOf(m_checkupMessageServerSelected))
            {
                m_checkupMessageServerSelectedIndex = Project.Instance.Servers.IndexOf(m_checkupMessageServerSelected);
            }

            m_checkupMessageServerSelected = m_checkupMessageServerSelectedIndex != -1 ? Project.Instance.Servers[m_checkupMessageServerSelectedIndex] : null;
            m_view.CheckupMessageControlServer = m_checkupMessageServerSelected != null ? m_checkupMessageServerSelected.ToString() : "-";
        }

        public async Task<bool> CheckupMessageCheckAsync() //OK
        {
            m_view.CheckupMessageScore = 0;
            m_view.CheckupMessageScoreResult = RESULT.NONE;
            m_view.CheckupMessageRulesCount = 0;
            m_view.CheckupMessageReport = string.Empty;

            if (m_checkupMessageServerSelected == null)
            {
                MessageBox.Show("Please select a server with the blue arrows in Checkup > Message panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Server server = m_checkupMessageServerSelected;

            if (string.IsNullOrWhiteSpace(server.ConnectionGeneralHost))
            {
                MessageBox.Show(server.ToString() + " Please fill 'Host' field in Server > Connection > General panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (server.ConnectionAuthentication)
            {
                if (string.IsNullOrWhiteSpace(server.ConnectionAuthenticationUsername))
                {
                    MessageBox.Show(server.ToString() + " Please fill 'Username' field in Servers > Connection > Authentication panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(server.ConnectionAuthenticationPassword))
                {
                    MessageBox.Show(server.ToString() + " Please fill 'Password' field in Servers > Connection > Authentication panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(server.SettingsSenderEmail))
            {
                MessageBox.Show(server.ToString() + " Please fill 'Email' field in Servers > Settings > Sender panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (!Utils.IsValidEmail(server.SettingsSenderEmail))
            {
                MessageBox.Show(server.ToString() + " 'Email' field in Servers > Settings > Sender panel is not a valid email address.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            while (!await Utils.IsNetworkAvailable())
            {
                DialogResult dialogResult = MessageBox.Show("No internet connection, please check your network.", "Error - Ultimate Mailer V3", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                if (dialogResult == DialogResult.Cancel)
                {
                    return false;
                }
            }

            var mimeMessageFactory = new MimeMessageFactory(server, new Recipient(Constantes.IMAP_USER, Project.Instance.RecipientFieldCount), false);
            MimeMessage message = mimeMessageFactory.GetMimeMessage();

            using (var smtpClient = new SmtpClient())
            {
                if (!Properties.Settings.Default.SettingServersCertificate)
                {
                    smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                }

                smtpClient.SslProtocols = server.ConnectionGeneralProtocol;
                smtpClient.Timeout = server.SettingsGeneralTimeout;

                try
                {
                    await smtpClient.ConnectAsync(server.ConnectionGeneralHost, server.ConnectionGeneralPort, SecureSocketOptions.Auto);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(server.ToString() + " Unable to connect on SMTP server : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (!smtpClient.IsConnected)
                {
                    MessageBox.Show(server.ToString() + " Unable to connect on SMTP server.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (server.ConnectionAuthentication)
                {
                    try
                    {
                        await smtpClient.AuthenticateAsync(server.ConnectionAuthenticationUsername, server.ConnectionAuthenticationPassword);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(server.ToString() + " Unable to authenticate on SMTP server : " + e.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        try
                        {
                            await smtpClient.DisconnectAsync(true);
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(server.ToString() + " Unable to disconnect from SMTP server : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        return false;
                    }

                    if (!smtpClient.IsAuthenticated)
                    {
                        MessageBox.Show(server.ToString() + " Unable to authenticate on SMTP server.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        try
                        {
                            await smtpClient.DisconnectAsync(true);
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(server.ToString() + " Unable to disconnect from SMTP server : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        return false;
                    }
                }

                try
                {
                    await smtpClient.SendAsync(message);
                }
                catch (Exception e)
                {
                    MessageBox.Show(server.ToString() + " An error occured while sending message : " + e.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                finally
                {
                    try
                    {
                        await smtpClient.DisconnectAsync(true);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(server.ToString() + " Unable to disconnect from SMTP server : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            MimeMessage sentMessage = null;

            using (var imapClient = new ImapClient())
            {
                imapClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                imapClient.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;

                try
                {
                    await imapClient.ConnectAsync(Constantes.IMAP_HOST, 993, SecureSocketOptions.Auto);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to connect on IMAP server : " + e.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (!imapClient.IsConnected)
                {
                    MessageBox.Show("Unable to connect on IMAP server.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    try
                    {
                        await imapClient.DisconnectAsync(true);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Unable to disconnect from IMAP server : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    return false;
                }

                try
                {
                    await imapClient.AuthenticateAsync(Constantes.IMAP_USER, Constantes.IMAP_PASS);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to authenticate on IMAP server : " + e.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    try
                    {
                        await imapClient.DisconnectAsync(true);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Unable to disconnect from IMAP server : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    return false;
                }

                if (!imapClient.IsAuthenticated)
                {
                    MessageBox.Show("Unable to authenticate on IMAP server.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    try
                    {
                        await imapClient.DisconnectAsync(true);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Unable to disconnect from IMAP server : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    return false;
                }

                IMailFolder inboxFolder = null;

                try
                {
                    inboxFolder = imapClient.Inbox;
                    inboxFolder.Open(FolderAccess.ReadWrite);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Unable to open INBOX folder on IMAP server : " + e.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    try
                    {
                        await imapClient.DisconnectAsync(true);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show("Unable to disconnect from IMAP server : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    return false;
                }

                if (inboxFolder == null)
                {
                    MessageBox.Show("Unable to open INBOX folder on IMAP server.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                do
                {
                    int sentMessageIndex = inboxFolder.ToList().FindLastIndex(inboxMessage => inboxMessage.MessageId == message.MessageId);

                    if (sentMessageIndex != -1)
                    {
                        sentMessage = inboxFolder.ToList()[sentMessageIndex];
                        inboxFolder.AddFlags(sentMessageIndex, MessageFlags.Deleted, true);
                        await inboxFolder.ExpungeAsync();
                    }
                    else
                    {
                        DialogResult dialogResult = MessageBox.Show("Unable to find your message on IMAP server.", "Error - Ultimate Mailer V3", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                        if (dialogResult == DialogResult.Cancel)
                        {
                            try
                            {
                                await imapClient.DisconnectAsync(true);
                            }
                            catch (Exception exception)
                            {
                                MessageBox.Show("Unable to disconnect from IMAP server : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return false;
                            }

                            return false;
                        }
                    }
                }
                while (sentMessage == null);

                try
                {
                    await imapClient.DisconnectAsync(true);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Unable to disconnect from IMAP server : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            string messageRaw = string.Empty;

            if (!GetMessageRaw(sentMessage, out messageRaw))
            {
                return false;
            }

            PostmarkResponse postmarkResponse = null;

            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = null;

                do
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var dictionary = new Dictionary<string, string> { { "email", messageRaw } };

                    try
                    {
                        using (var content = new FormUrlEncodedContent(dictionary))
                        {
                            httpResponseMessage = await httpClient.PostAsync(Constantes.URL_API_SPAMASSASSIN, content);
                        }

                        if (httpResponseMessage == null || httpResponseMessage.StatusCode != HttpStatusCode.OK)
                        {
                            DialogResult dialogResult = MessageBox.Show("Unable to reach Postmark API, please check your network.", "Error - Ultimate Mailer V3", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                            if (dialogResult == DialogResult.Cancel)
                            {
                                return false;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        DialogResult dialogResult = MessageBox.Show("An error occured while trying to reach Postmark API : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                        if (dialogResult == DialogResult.Cancel)
                        {
                            return false;
                        }
                    }

                    if (httpResponseMessage != null && httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string resultContent = await httpResponseMessage.Content.ReadAsStringAsync();
                        postmarkResponse = JsonConvert.DeserializeObject<PostmarkResponse>(resultContent);

                        if (!postmarkResponse.Success)
                        {
                            DialogResult dialogResult = MessageBox.Show("An internal Postmark API error occured : " + postmarkResponse.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                            if (dialogResult == DialogResult.Cancel)
                            {
                                return false;
                            }
                        }
                    }
                }
                while (httpResponseMessage == null || httpResponseMessage.StatusCode != HttpStatusCode.OK || postmarkResponse == null || !postmarkResponse.Success);
            }

            m_view.CheckupMessageScore = postmarkResponse.Score;
            m_view.CheckupMessageScoreResult = postmarkResponse.Result;

            m_checkupMessageRules.Clear();
            m_checkupMessageRules.TrimExcess();
            m_checkupMessageRules.AddRange(postmarkResponse.Rules);

            m_view.CheckupMessageRulesCount = m_checkupMessageRules.Count;
            m_view.CheckupMessageReport = postmarkResponse.Report;

            return true;
        }

        public bool InputValidation() //OK
        {
            if (Project.Instance.Servers.Count == 0)
            {
                MessageBox.Show("Please add at least one SMTP server inside Servers tab.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            foreach (Server server in Project.Instance.Servers)
            {
                if (string.IsNullOrWhiteSpace(server.ConnectionGeneralHost))
                {
                    MessageBox.Show(server.ToString() + " Please fill 'Host' field in Servers > Connection > General panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (server.ConnectionAuthentication)
                {
                    if (string.IsNullOrWhiteSpace(server.ConnectionAuthenticationUsername))
                    {
                        MessageBox.Show(server.ToString() + " Please fill 'Username' field in Servers > Connection > Authentication panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(server.ConnectionAuthenticationPassword))
                    {
                        MessageBox.Show(server.ToString() + " Please fill 'Password' field in Servers > Connection > Authentication panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                if (string.IsNullOrWhiteSpace(server.SettingsSenderEmail))
                {
                    MessageBox.Show(server.ToString() + " Please fill 'Email' field in Servers > Settings > Sender panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else if (!Utils.IsValidEmail(server.SettingsSenderEmail))
                {
                    MessageBox.Show(server.ToString() + " 'Email' field in Servers > Settings > Sender panel is not a valid email address.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            foreach (Proxy proxy in Project.Instance.Proxies)
            {
                if (string.IsNullOrWhiteSpace(proxy.GeneralHost))
                {
                    MessageBox.Show(proxy.ToString() + " Please fill 'Host' field in Proxies > Genral panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (proxy.Authentication)
                {
                    if (string.IsNullOrWhiteSpace(proxy.AuthenticationUsername))
                    {
                        MessageBox.Show(proxy.ToString() + " Please fill 'Username' field in Proxies > Authentication panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    if (string.IsNullOrWhiteSpace(proxy.AuthenticationPassword))
                    {
                        MessageBox.Show(proxy.ToString() + " Please fill 'Password' field in Proxies > Authentication panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }

            if (Project.Instance.Recipients.Count == 0)
            {
                MessageBox.Show("Please add at least one recipient inside Recipients tab.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!Utils.IsValidIdentifier(Project.Instance.HeaderGeneralIdentifier) && !Project.Instance.HeaderGeneralIdentifier.Equals("{SERVER_HOST}") && !Project.Instance.HeaderGeneralIdentifier.Equals("{SENDER_HOST}") && !Project.Instance.HeaderGeneralIdentifier.Equals("{FROM_HOST}") && !Project.Instance.HeaderGeneralIdentifier.Equals("{TO_HOST}"))
            {
                MessageBox.Show("'Message-ID' field in Header > General panel is not a valid domain name.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (Project.Instance.HeaderFrom && string.IsNullOrWhiteSpace(Project.Instance.HeaderFromEmail))
            {
                MessageBox.Show("Please fill 'Email' field in Header > From panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (Project.Instance.HeaderFrom && !Utils.IsValidEmail(Project.Instance.HeaderFromEmail) && !Project.Instance.HeaderFromEmail.Equals("{SENDER_EMAIL}") && !Project.Instance.HeaderFromEmail.Equals("{TO_EMAIL}"))
            {
                MessageBox.Show("'Email' field in Header > From panel is not a valid email address.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (Project.Instance.HeaderReplyTo && string.IsNullOrWhiteSpace(Project.Instance.HeaderReplyToEmail))
            {
                MessageBox.Show("Please fill 'Email' field in Header > Reply-To panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (Project.Instance.HeaderReplyTo && !Utils.IsValidEmail(Project.Instance.HeaderReplyToEmail) && !Project.Instance.HeaderReplyToEmail.Equals("{SENDER_EMAIL}") && !Project.Instance.HeaderReplyToEmail.Equals("{FROM_EMAIL}"))
            {
                MessageBox.Show("'Email' field in Header > Reply-To panel is not a valid email address.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (Project.Instance.HeaderListUnsubscribe && string.IsNullOrWhiteSpace(Project.Instance.HeaderListUnsubscribeEmail) && string.IsNullOrWhiteSpace(Project.Instance.HeaderListUnsubscribeUrl))
            {
                MessageBox.Show("Please fill 'Email' or 'Url' field in Header > List-Unsubscribe panel.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (Project.Instance.HeaderListUnsubscribe && !string.IsNullOrWhiteSpace(Project.Instance.HeaderListUnsubscribeEmail) && !Utils.IsValidEmail(Project.Instance.HeaderListUnsubscribeEmail) && !Project.Instance.HeaderListUnsubscribeEmail.Equals("{SENDER_EMAIL}"))
            {
                MessageBox.Show("'Email' field in Header > List-Unsubscribe panel is not a valid email address.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if (Project.Instance.HeaderListUnsubscribe && !string.IsNullOrWhiteSpace(Project.Instance.HeaderListUnsubscribeUrl) && !Utils.IsValidUrl(Project.Instance.HeaderListUnsubscribeUrl))
            {
                MessageBox.Show("'Url' field in Header > List-Unsubscribe panel is not a valid url.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(Project.Instance.HeaderAdvancedReturnPath) && (!Utils.IsValidEmail(Project.Instance.HeaderAdvancedReturnPath) && !Project.Instance.HeaderAdvancedReturnPath.Equals("{SENDER_EMAIL}")))
            {
                MessageBox.Show("'Return-Path' field in Header > Advanced panel is not a valid email address.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            foreach (string attachment in Project.Instance.MessageAttachments)
            {
                if (!File.Exists(attachment))
                {
                    MessageBox.Show("[#" + (Project.Instance.MessageAttachments.IndexOf(attachment) + 1) + " " + Path.GetFileName(attachment) + "] The attachment was not found on your hard drive, please remove it from 'Attachments' list inside Message tab.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            try
            {
                checked
                {
                    int projectCapabilities = Project.Instance.Recipients.Count * Project.Instance.ExtrasBomberAmount;
                }
            }
            catch
            {
                MessageBox.Show("Your campaign requires sending too many emails, please lower the Extras > Bomber > Amount field.", "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public void InputInspection()
        {
            m_view.CheckupInspectionCount = 0;
            m_checkupInspections.TrimExcess();
            m_checkupInspections.Clear();

            if (Project.Instance.Proxies.Count == 0)
            {
                m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.INFO, "Your mailing campaign do not use any proxy, add some private dedicated proxies to increase your inbox rate."));
            }

            if (Project.Instance.Subjects.Count == 0)
            {
                m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.DANGER, "Your message has no subject, it will be flagged as spam."));
            }

            if (Project.Instance.Subjects.Count < 5)
            {
                m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.INFO, "Add at least five subjects to your message to get a better inbox rate."));
            }

            foreach (string subject in Project.Instance.Subjects)
            {
                int subjectID = Project.Instance.Subjects.IndexOf(subject) + 1;
                string subjectCustomized = EmulateStringCustomization(subject);
                string[] subjectWords = subjectCustomized.Words();

                if (subjectCustomized.Contains('!'))
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.DANGER, "The subject #" + subjectID + " contains exclamation mark(s), it will drastically increase your spam score."));
                }

                if (subjectCustomized.Count(char.IsUpper) > 3)
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.WARNING, "The subject #" + subjectID + " contains several uppercase letters, it will increase your spam score."));
                }

                if (subjectWords.Count(word => word == word.ToUpper() && word.Any(char.IsLetter)) != 0)
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.DANGER, "The subject #" + subjectID + " contains uppercase words, it will be flagged as spam."));
                }

                if (!subjectCustomized.Any(char.IsLetter))
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.DANGER, "The subject #" + subjectID + " does not contains any letter, it will be flagged as spam."));
                }

                if (subjectCustomized.Count(c => char.IsPunctuation(c) && !char.IsWhiteSpace(c)) > 2)
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.WARNING, "The subject #" + subjectID + " as more than two punctuation marks, it will increase your spam score."));
                }

                if (subjectCustomized.Count(c => (char.IsSurrogate(c) || char.IsSymbol(c))) > 2)
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.WARNING, "The subject #" + subjectID + " as more than two emoji(s) or symbol(s), it will increase your spam score."));
                }

                if (subjectWords.Length == 1)
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.DANGER, "The subject #" + subjectID + " is only one word long, it will be flagged as spam."));
                }

                if (subjectCustomized.StartsWith("re ", StringComparison.OrdinalIgnoreCase) || subjectCustomized.StartsWith("re:", StringComparison.OrdinalIgnoreCase) || subjectCustomized.StartsWith("[re]", StringComparison.OrdinalIgnoreCase) || subjectCustomized.StartsWith("[re:]", StringComparison.OrdinalIgnoreCase))
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.WARNING, "The subject #" + subjectID + " start with a return badge but is sent for the first time, it will increase your spam score."));
                }

                if (subjectCustomized.StartsWith("fwd ", StringComparison.OrdinalIgnoreCase) || subjectCustomized.StartsWith("fwd:", StringComparison.OrdinalIgnoreCase) || subjectCustomized.StartsWith("[fwd]", StringComparison.OrdinalIgnoreCase) || subjectCustomized.StartsWith("[fwd:]", StringComparison.OrdinalIgnoreCase))
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.WARNING, "The subject #" + subjectID + " start with forward badge but is sent for the first time, it will increase your spam score."));
                }

                if (subjectCustomized.Length > 50)
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.WARNING, "The subject #" + subjectID + " is more than 50 characters long, it will increases your spam score."));
                }

                IEnumerable<string> spammyWords = Constantes.SPAM_WORDS.Where(words => subject.IndexOf(words) != -1);

                if (spammyWords.Count() > 0)
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.DANGER, "The subject #" + subjectID + " contains the following spammy word(s) : '" + string.Join("', '", spammyWords) + "' it will drastically increases your spam score."));
                }
            }

            if (string.IsNullOrWhiteSpace(Project.Instance.HeaderGeneralIdentifier))
            {
                m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.DANGER, "Your message does not have a message identifier, the name of your computer will be used to identify each message, this may damage your sending reputation. Instead, use {SERVER_HOST} or {SENDER HOST} for best results."));
            }

            if (Project.Instance.HeaderFrom)
            {
                if (!string.IsNullOrWhiteSpace(Project.Instance.HeaderFromEmail) && !Project.Instance.HeaderFromEmail.Equals("{SENDER_EMAIL}"))
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.DANGER, "Your message contains a From email header different from the Sender email address, it's often used by spammers spoof email address, it will drastically increase your spam score."));
                }
            }

            if (Project.Instance.HeaderReplyTo)
            {
                if (!string.IsNullOrWhiteSpace(Project.Instance.HeaderReplyToEmail) && !Project.Instance.HeaderReplyToEmail.Equals("{SENDER_EMAIL}"))
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.DANGER, "Your message contains a Reply-To email header different from the Sender email address, it's often used by spammers to get response on message sent from spoofed email address, it will drastically increase your spam score."));
                }
            }

            if (!Project.Instance.HeaderListUnsubscribe)
            {
                m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.INFO, "Your message does not contain a List-Unsubscribe header, the List-Unsubscribe header is necessary if you are sending bulk emails, it allows the user to easily unsubscribe from your lists."));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(Project.Instance.HeaderListUnsubscribeUrl) && string.IsNullOrWhiteSpace(Project.Instance.HeaderListUnsubscribeEmail))
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.WARNING, "The List-Unsubscribe header is only defined by an URL, unfortunately, not all email clients support the http method. If you using it alone, this may damage your sending reputation."));
                }
            }

            if (string.IsNullOrWhiteSpace(Project.Instance.MessageBody))
            {
                m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.DANGER, "The body of your message is empty or contains only whitespaces, it will be flagged as spam."));
            }
            else
            {
                string messageBody = EmulateStringCustomization(Project.Instance.MessageBody);
                string messageBodyText = Project.Instance.MessageBodyHtml ? Utils.HtmlToText(messageBody) : messageBody;
                string[] messageBodyWords = messageBodyText.Words();

                if (messageBodyText.Contains('!'))
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.WARNING, "The message body contains exclamation mark(s), it will increase spam score."));
                }

                if (!messageBodyText.Any(char.IsLetter))
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.DANGER, "The message body does not contains any letter, it will be flagged as spam."));
                }

                if (messageBodyText.Count(c => char.IsSurrogate(c) || char.IsSymbol(c)) > 2)
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.WARNING, "The message body contains more than two emoji(s) or symbol(s), it will increase spam score."));
                }

                if (messageBodyWords.Count() == 1)
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.DANGER, "The message body is only one word long, it will be flagged as spam."));
                }

                if (messageBodyWords.Count(word => word == word.ToUpper() && word.Any(char.IsLetter)) != 0)
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.DANGER, "The message body contains uppercase words, it will drastically increase spam score."));
                }

                IEnumerable<string> spammyWords = Constantes.SPAM_WORDS.Where(words => messageBodyText.IndexOf(words) != -1);

                if (spammyWords.Count() > 0)
                {
                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.DANGER, "The message body contains the following spammy word(s) : '" + string.Join("', '", spammyWords) + "' it will drastically increases your spam score."));
                }

                if (Project.Instance.MessageBodyHtml)
                {
                    if (Project.Instance.MessageBody.IndexOf("<img", StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.WARNING, "The HTML code of message body contains <img> tags, this will increase your spam score."));
                    }
                }
            }

            if (Project.Instance.MessageAttachments.Count > 1)
            {
                m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.WARNING, "The message contains more than one attachment, it will increase your spam score."));
            }

            foreach (string attachement in Project.Instance.MessageAttachments)
            {
                string fileExtension = Path.GetExtension(attachement).Replace(".", string.Empty);

                if (Constantes.BLOCKED_EXT.Contains(fileExtension))
                {
                    int attachmentID = Project.Instance.MessageAttachments.IndexOf(attachement);

                    m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.DANGER, "The attachment #" + attachmentID + " is a " + fileExtension + " file, this file extension is blocked by most mailbox providers. Your message will be flagged as spam."));
                }
            }

            if (Project.Instance.ExtrasBomberAmount > 1)
            {
                m_checkupInspections.Add(new Inspection(Inspection.VERBOSE.DANGER, "The amount of email per recipient is greater than one, never use the bomber feature with your primary mailbox as it might blacklist your server."));
            }

            m_checkupInspections = m_checkupInspections.OrderByDescending(inspection => (int)inspection.Verbose).ToList();
            m_view.CheckupInspectionCount = m_checkupInspections.Count;
        }

        private string EmulateStringCustomization(string input)
        {
            var stringBuilder = new StringBuilder(input);

            var sender = new System.Net.Mail.MailAddress("sender@example.com");

            stringBuilder.Replace("{SENDER_NAME}", sender.DisplayName);
            stringBuilder.Replace("{SENDER_EMAIL}", sender.Address);
            stringBuilder.Replace("{SENDER_USER}", sender.User);
            stringBuilder.Replace("{SENDER_HOST}", sender.Host);

            var from = new System.Net.Mail.MailAddress("from@example.com");

            stringBuilder.Replace("{FROM_NAME}", from.DisplayName);
            stringBuilder.Replace("{FROM_EMAIL}", from.Address);
            stringBuilder.Replace("{FROM_USER}", from.User);
            stringBuilder.Replace("{FROM_HOST}", from.Host);

            var to = new System.Net.Mail.MailAddress("to@example.com");

            stringBuilder.Replace("{TO_EMAIL}", to.Address);

            if (Utils.TryBase64Encode(to.Address, out string toEmail))
            {
                stringBuilder.Replace("{TO_EMAIL_BASE64}", toEmail);
            }

            stringBuilder.Replace("{TO_USER}", to.User);
            stringBuilder.Replace("{TO_HOST}", to.Host);

            for (int i = 0; i < Project.Instance.RecipientFieldCount; ++i)
            {
                stringBuilder.Replace("{TO_FIELD_" + (i + 1) + "}", "lorem");
            }

            var random = new Random();

            int index = -1;

            while ((index = stringBuilder.ToString().IndexOf("{RAND_STR}")) != -1)
            {
                string randomStr = new string(Enumerable.Repeat(Constantes.CHARS, random.Next(10, 20)).Select(s => s[random.Next(s.Length)]).ToArray());
                stringBuilder.Replace("{RAND_STR}", randomStr, index, 10);
            }

            while ((index = stringBuilder.ToString().IndexOf("{RAND_INT}")) != -1)
            {
                int randomInt = random.Next(int.MinValue, int.MaxValue);
                stringBuilder.Replace("{RAND_INT}", randomInt.ToString(), index, 10);
            }

            while ((index = stringBuilder.ToString().IndexOf("{RAND_UINT}")) != -1)
            {
                int randomUint = random.Next(0, int.MaxValue);
                stringBuilder.Replace("{RAND_UINT}", randomUint.ToString(), index, 11);
            }

            while ((index = stringBuilder.ToString().IndexOf("{RAND_IPV4}")) != -1)
            {
                var data = new byte[4];
                random.NextBytes(data);

                IPAddress ipv4 = new IPAddress(data);
                stringBuilder.Replace("{RAND_IPV4}", ipv4.ToString(), index, 11);
            }

            while ((index = stringBuilder.ToString().IndexOf("{RAND_IPV6}")) != -1)
            {
                var data = new byte[16];
                random.NextBytes(data);

                IPAddress ipv6 = new IPAddress(data);
                stringBuilder.Replace("{RAND_IPV6}", ipv6.ToString(), index, 11);
            }

            stringBuilder.Replace("{GUID}", Guid.NewGuid().ToString());

            DateTime dateTime = DateTime.Now;

            stringBuilder.Replace("{DATE}", dateTime.ToShortDateString());
            stringBuilder.Replace("{TIME}", dateTime.ToShortTimeString());
            stringBuilder.Replace("{DATETIME}", dateTime.ToString());

            return Spinner.Spin(stringBuilder.ToString());
        }

        private bool ImportServers(string filePath, char separator, out IList<Server> servers)
        {
            servers = new List<Server>();

            try
            {
                using (var streamReader = new StreamReader(filePath))
                {
                    string line = string.Empty;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string[] data = line.Split(separator);
                        if (data.Length == 0) continue;

                        Server server = new Server
                        {
                            ConnectionGeneralHost = data.Length > 0 ? data[0] : string.Empty,
                            ConnectionGeneralPort = data.Length > 1 && int.TryParse(data[1], out int value) && value > 0 && value < 65535 ? value : 25,
                            ConnectionAuthentication = (data.Length > 2 && !string.IsNullOrWhiteSpace(data[2])) && (data.Length > 3 && !string.IsNullOrWhiteSpace(data[3])),
                           
                            ConnectionAuthenticationUsername = data.Length > 2 ? data[2] : string.Empty,
                            ConnectionAuthenticationPassword = data.Length > 3 ? data[3] : string.Empty,

                            SettingsSenderEmail = data.Length > 4 && Utils.IsValidEmail(data[4]) ? data[4] : (data.Length > 2 && Utils.IsValidEmail(data[2]) ? data[2] : string.Empty),
                            SettingsSenderName = data.Length > 5 ? data[5] : string.Empty

                        };

                        servers.Add(server);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("An error occurred while importing servers from file " + Path.GetFileName(filePath) + " : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ExportServers(string filePath, char separator)
        {
            var stringBuilder = new StringBuilder();

            foreach (Server server in Project.Instance.Servers)
            {
                var tempStringBuilder = new StringBuilder();

                tempStringBuilder.Append(server.ConnectionGeneralHost);
                tempStringBuilder.Append(separator);
                tempStringBuilder.Append(server.ConnectionGeneralPort);
                tempStringBuilder.Append(separator);

                tempStringBuilder.Append(server.ConnectionAuthenticationUsername);
                tempStringBuilder.Append(separator);
                tempStringBuilder.Append(server.ConnectionAuthenticationPassword);
                tempStringBuilder.Append(separator);

                tempStringBuilder.Append(server.SettingsSenderEmail);
                tempStringBuilder.Append(separator);
                tempStringBuilder.Append(server.SettingsSenderName);

                stringBuilder.AppendLine(tempStringBuilder.ToString());
                tempStringBuilder.Clear();
            }

            try
            {
                File.WriteAllText(filePath, stringBuilder.ToString());
            }
            catch (Exception exception)
            {
                MessageBox.Show("An error occured while exporting servers to file " + Path.GetFileName(filePath) + " : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ImportRecipients(string filePath, char separator, out IList<Recipient> recipients)
        {
            recipients = new List<Recipient>();

            try
            {
                using (var streamReader = File.OpenText(filePath))
                {
                    string line = string.Empty;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string[] data = line.Split(separator);
                        if (data.Length == 0) continue;

                        if (Utils.IsValidEmail(data[0]))
                        {
                            var recipient = new Recipient(data[0], Project.Instance.RecipientFieldCount);

                            for (int i = 0; i < Project.Instance.RecipientFieldCount; ++i)
                            {
                                if (i + 1 >= data.Length) break;
                                recipient.Fields[i] = data[i + 1];
                            }

                            recipients.Add(recipient);
                        }
                    }

                    streamReader.Close();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("An error occurred while importing recipients from file " + Path.GetFileName(filePath) + " : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ExportRecipients(string filePath, char separator)
        {
            var stringBuilder = new StringBuilder();

            foreach (Recipient recipient in Project.Instance.Recipients)
            {
                var tempStringBuilder = new StringBuilder(recipient.Email);

                if (recipient.Fields.Count > 0)
                {
                    tempStringBuilder.Append(separator);
                    tempStringBuilder.Append(string.Join(separator.ToString(), recipient.Fields));
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
                MessageBox.Show("An error occured while exporting recipients to file " + Path.GetFileName(filePath) + " : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ImportProxies(string filePath, char separator, out IList<Proxy> proxies)
        {
            proxies = new List<Proxy>();

            try
            {
                using (var streamReader = File.OpenText(filePath))
                {
                    string line;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string[] data = line.Split(separator);
                        if (data.Length == 0) continue;

                        Proxy proxy = new Proxy
                        {
                            GeneralType = data.Length > 0 && int.TryParse(data[0], out int type) && type < 4 ? (Proxy.TYPE)type : 0,
                            GeneralHost = data.Length > 1 ? data[1] : string.Empty,
                            GeneralPort = data.Length > 2 && int.TryParse(data[2], out int port) ? port : 8080,
                            Authentication = data.Length > 3,
                            AuthenticationUsername = data.Length > 3 ? data[3] : string.Empty,
                            AuthenticationPassword = data.Length > 4 ? data[4] : string.Empty
                        };

                        proxies.Add(proxy);
                    }

                    streamReader.Close();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("An error occurred while importing proxies from file " + Path.GetFileName(filePath) + " : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ExportProxies(string filePath, char separator)
        {
            var stringBuilder = new StringBuilder();

            foreach (Proxy proxy in Project.Instance.Proxies)
            {
                var tempStringBuilder = new StringBuilder();

                tempStringBuilder.Append((int)proxy.GeneralType);
                tempStringBuilder.Append(separator);
                tempStringBuilder.Append(proxy.GeneralHost);
                tempStringBuilder.Append(separator);
                tempStringBuilder.Append(proxy.GeneralPort);

                if (proxy.Authentication)
                {
                    tempStringBuilder.Append(separator);
                    tempStringBuilder.Append(proxy.AuthenticationUsername);
                    tempStringBuilder.Append(separator);
                    tempStringBuilder.Append(proxy.AuthenticationPassword);
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
                MessageBox.Show("An error occured while exporting proxies to file " + Path.GetFileName(filePath) + " : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ImportSubjects(string filePath, out IList<string> subjects)
        {
            subjects = new List<string>();

            try
            {
                using (var streamReader = File.OpenText(filePath))
                {
                    string line = string.Empty;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            subjects.Add(line);
                        }
                    }

                    streamReader.Close();
                }

            }
            catch (Exception exception)
            {
                MessageBox.Show("An error occurred while importing subjects from file " + Path.GetFileName(filePath) + " : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ExportSubjects(string filePath)
        {
            var stringbuilder = new StringBuilder();
            Project.Instance.Subjects.ForEach(subject => stringbuilder.AppendLine(subject));

            try
            {
                File.WriteAllText(filePath, stringbuilder.ToString());
            }
            catch (Exception exception)
            {
                MessageBox.Show("An error occured while exporting subjects to file " + Path.GetFileName(filePath) + " : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool GetMessageRaw(MimeMessage message, out string messageRaw)
        {
            messageRaw = string.Empty;

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    message.WriteTo(memoryStream);
                    memoryStream.Position = 0;

                    using (var streamReader = new StreamReader(memoryStream))
                    {
                        messageRaw = streamReader.ReadToEnd();
                        streamReader.Close();
                    }

                    memoryStream.Close();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("An error occurred while parsing the raw message : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool GetMessageSize(MimeMessage message, out double messageSize)
        {
            messageSize = 0;

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    message.WriteTo(memoryStream);
                    messageSize = memoryStream.Length;
                    memoryStream.Close();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("An error occurred while parsing the message size : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MainController()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    Project.Instance.Dispose();

                    m_previewMailboxAttachments.Clear();
                    m_previewMailboxAttachments.TrimExcess();

                    m_checkupMessageRules.Clear();
                    m_checkupMessageRules.TrimExcess();

                    m_checkupInspections.Clear();
                    m_checkupInspections.TrimExcess();
                }

                IsDisposed = true;
            }
        }
    }
}