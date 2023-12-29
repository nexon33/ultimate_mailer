using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using ultimate.mailer.Views;
using System.Threading.Tasks;

using Application = System.Windows.Forms.Application;

namespace ultimate.mailer.Controllers
{
    public class SplashController
    {
        private readonly ISplashView m_view;

        public SplashController(ISplashView view)
        {
            m_view = view;
            m_view.SetController(this);

            LoadView();
        }

        private void LoadView()
        {
            m_view.LoadInformation = "Loading";
            m_view.SoftwareCopyright = "Copyright © Ultimate Mailer " + DateTime.Now.Year;

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            m_view.SoftwareVersion = "Version " + fileVersionInfo.ProductVersion;
        }

        public async Task<bool> AssertNetworkConnectionAsync()
        {
            m_view.LoadInformation = "Checking internet connection...";

            while (!await Utils.IsNetworkAvailable())
            {
                DialogResult dialogResult = MessageBox.Show("No internet connection, please check your network.", "Error - Ultimate Mailer V3", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                if (dialogResult == DialogResult.Cancel)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> AssertSoftwareVersionAsync()
        {
            m_view.LoadInformation = "Searching for update...";

            HttpResponseMessage response = null;

            using (var httpClient = new HttpClient())
            {
                do
                {
                    try
                    {
                        response = await httpClient.GetAsync(Constantes.URL_SERVICE_VERSION);

                        if (response == null || response.StatusCode != HttpStatusCode.OK)
                        {
                            DialogResult dialogResult = MessageBox.Show("Unable to reach webservice, if the error persist please contact an administrator.", "Error - Ultimate Mailer V3", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                            if (dialogResult == DialogResult.Cancel)
                            {
                                return false;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        DialogResult dialogResult = MessageBox.Show("An error occurred while searching for update : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                        if (dialogResult == DialogResult.Cancel)
                        {
                            return false;
                        }
                    }
                }
                while (response == null || response.StatusCode != HttpStatusCode.OK);
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            string currentSoftwareVersion = fileVersionInfo.ProductVersion;
            string availableSoftwareVersion = await response.Content.ReadAsStringAsync();

            if (!currentSoftwareVersion.Equals(availableSoftwareVersion))
            {
                DialogResult dialogResult = MessageBox.Show("An update is available, please update the software to continue.", "Information - Ultimate Mailer V3", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (dialogResult == DialogResult.No)
                {
                    return false;
                }

                m_view.LoadInformation = "Update in progress...";

                bool successfullyMoved = false;

                do
                {
                    try
                    {
                        File.Delete(Application.ExecutablePath.Replace(".exe", ".old"));
                        File.Move(Application.ExecutablePath, Application.ExecutablePath.Replace(".exe", ".old"));
                        successfullyMoved = true;
                    }
                    catch (Exception exception)
                    {
                        dialogResult = MessageBox.Show("An error occurred while trying to rename current executable : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                        if (dialogResult == DialogResult.Cancel)
                        {
                            return false;
                        }
                    }
                }
                while (!successfullyMoved);

                using (var webClient = new WebClient())
                {
                    webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;

                    bool successfullyDownloaded = false;

                    do
                    {
                        try
                        {
                            var uri = new Uri(IntPtr.Size == 4 ? Constantes.URL_DOWNLOAD_X86 : Constantes.URL_DOWNLOAD_X64);
                            await webClient.DownloadFileTaskAsync(uri, Path.GetFileName(Application.ExecutablePath));

                            successfullyDownloaded = true;
                        }
                        catch (Exception exception)
                        {
                            dialogResult = MessageBox.Show("An error occurred while downloading the update : " + exception.Message, "Error - Ultimate Mailer V3", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

                            if (dialogResult == DialogResult.Cancel)
                            {
                                return false;
                            }
                        }
                    }
                    while (!successfullyDownloaded);
                }

                dialogResult = MessageBox.Show("The software has been successfully updated ! To continue you must restart the application.", "Information - Ultimate Mailer V3", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (dialogResult == DialogResult.Yes)
                {
                    Process.Start(Application.ExecutablePath);
                }

                return false;
            }

            return true;
        }

        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            m_view.LoadInformation = "Update in progress " + e.ProgressPercentage + "% " + ((e.BytesReceived / 1024)) + "/" + ((e.TotalBytesToReceive / 1024)) + " kb";
        }
    }
}