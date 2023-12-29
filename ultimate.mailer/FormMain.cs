using MimeKit;

using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Windows.Forms;

using ultimate.mailer.Controllers;
using ultimate.mailer.Models;
using ultimate.mailer.Views;
using static System.Windows.Forms.ListViewItem;

namespace ultimate.mailer
{
    public partial class FormMain : Form, IMainView
    {
        private const int TIMER_UPDATE_INTERVAL = 1000;

        private MainController m_controller;
        private readonly bool m_rowScopeCommit;

        public string Title { set => Text = value + " - Ultimate Mailer V3"; }

        public int ServersCount
        {
            set
            {
                listViewServers.VirtualListSize = value;

                int textWidth = TextRenderer.MeasureText(value.ToString(), new Font("Microsoft Sans Serif", 9)).Width;
                listViewServers.Columns[0].Width = value > 999 ? textWidth : 28;

                toolStripButtonServersClear.Enabled = value != 0;
                toolStripButtonServersExport.Enabled = value != 0;
                toolStripButtonServersValidation.Enabled = value != 0;

                buttonCheckupMessageControlServerFirst.Enabled = value != 0;
                buttonCheckupMessageControlServerPrevious.Enabled = value != 0;
                buttonCheckupMessageControlServerNext.Enabled = value != 0;
                buttonCheckupMessageControlServerLast.Enabled = value != 0;
            }
        }

        public string ServerConnectionGeneralHost { set => textBoxServerConnectionGeneralHost.Text = value; }

        public int ServerConnectionGeneralPort { set => numericUpDownServerConnectionGeneralPort.Value = value; }

        public SslProtocols ServerConnectionGeneralProtocol { set => comboBoxServerConnectionGeneralProtocol.SelectedIndex = Array.IndexOf(Enum.GetValues(typeof(SslProtocols)), value); }

        public bool ServerConnectionAuthentication { set => checkBoxServerConnectionAuthentication.Checked = value; }

        public string ServerConnectionAuthenticationUsername { set => textBoxServerConnectionAuthenticationUsername.Text = value; }

        public string ServerConnectionAuthenticationPassword { set => textBoxServerConnectionAuthenticationPassword.Text = value; }

        public int ServerSettingsGeneralTimeout { set => numericUpDownServerSettingsGeneralTimeout.Value = value; }

        public int ServerSettingsGeneralDelay { set => numericUpDownServerSettingsGeneralDelay.Value = value; }

        public string ServerSettingsSenderName { set => comboBoxServerSettingsSenderName.Text = value; }

        public string ServerSettingsSenderEmail { set => textBoxServerSettingsSenderEmail.Text = value; }

        public bool ServerSettingsLimitSession { set => checkBoxServerSettingsLimitSession.Checked = value; }

        public int ServerSettingsLimitSessionValue { set => numericUpDownServerSettingsLimitSession.Value = value; }

        public bool ServerSettingsLimitHourly { set => checkBoxServerSettingsLimitHourly.Checked = value; }

        public int ServerSettingsLimitHourlyValue { set => numericUpDownServerSettingsLimitHourly.Value = value; }

        public bool ServerSettingsLimitDaily { set => checkBoxServerSettingsLimitDaily.Checked = value; }

        public int ServerSettingsLimitDailyValue { set => numericUpDownServerSettingsLimitDaily.Value = value; }

        public bool ServerSettingsLimitMonthly { set => checkBoxServerSettingsLimitMonthly.Checked = value; }

        public int ServerSettingsLimitMonthlyValue { set => numericUpDownServerSettingsLimitMonthly.Value = value; }

        public int ServerSettingsAdvancedSessionDelay { set => numericUpDownServerSettingsAdvancedSessionDelay.Value = value; }

        public bool ServerSettingsAdvancedPing { set => checkBoxServerSettingsAdvancedPing.Checked = value; }

        public int ServerSettingsAdvancedPingValue { set => numericUpDownServerSettingsAdvancedPing.Value = value; }

        public bool ServerSettingsAdvancedProxy { set => checkBoxServerSettingsAdvancedProxy.Checked = value; }

        public bool ServerReportsAvailable
        {
            set
            {
                labelServerReportsAvailableValue.Text = value ? "✔" : "✘";
                labelServerReportsAvailableValue.ForeColor = value ? Color.Green : Color.Red;
            }
        }

        public DateTime ServerReportsAvailability { set => labelServerReportsAvailabilityValue.Text = value != DateTime.MinValue ? value.ToString() : "-"; }

        public int ServerReportsPerformedCount { set => labelServerReportsPerformedCount.Text = value.ToString() + " message(s)."; }

        public int ServerReportsSentCount { set => labelServerReportsSentCount.Text = value.ToString() + " message(s)."; }

        public int ServerReportsErrorCount { set => labelServerReportsErrorCount.Text = value.ToString() + " message(s)."; }

        public int ServerReportsHourlyCount { set => labelServerReportsHourlyCount.Text = value.ToString() + " message(s)."; }

        public int ServerReportsDailyCount { set => labelServerReportsDailyCount.Text = value.ToString() + " message(s)."; }

        public int ServerReportsMonthlyCount { set => labelServerReportsMonthlyCount.Text = value.ToString() + " message(s)."; }

        public int ServerReportsCapabilities { set => labelServerReportsCapabilitiesValue.Text = (value == -1 ? "Unlimited" : value.ToString()) + " message(s)."; }

        public int ProxiesCount
        {
            set
            {
                listViewProxies.VirtualListSize = value;

                int textWidth = TextRenderer.MeasureText(value.ToString(), new Font("Microsoft Sans Serif", 9)).Width;
                listViewProxies.Columns[0].Width = value > 999 ? textWidth : 28;

                toolStripButtonProxiesClear.Enabled = value != 0;
                toolStripButtonProxiesExport.Enabled = value != 0;
                toolStripButtonProxiesValidation.Enabled = value != 0;
            }
        }

        public Proxy.TYPE ProxyGeneralType { set => comboBoxProxyGeneralType.SelectedIndex = (int)value; }

        public string ProxyGeneralHost { set => textBoxProxyGeneralHost.Text = value; }

        public int ProxyGeneralPort { set => numericUpDownProxyGeneralPort.Value = value; }

        public bool ProxyAuthentication { set => checkBoxProxyAuthentication.Checked = value; }

        public string ProxyAuthenticationUsername { set => textBoxProxyAuthenticationUsername.Text = value; }

        public string ProxyAuthenticationPassword { set => textBoxProxyAuthenticationPassword.Text = value; }

        public bool ProxyTest { set => buttonProxyTest.Enabled = value; }

        public int RecipientsCount
        {
            set
            {
                dataGridViewRecipients.RowCount = value;

                toolStripButtonRecipientsClear.Enabled = value != 0;
                toolStripButtonRecipientsExport.Enabled = value != 0;
                toolStripButtonRecipientsSort.Enabled = value != 0;
                toolStripButtonRecipientsRemoveDuplicates.Enabled = value != 0;
                toolStripButtonRecipientsValidation.Enabled = value != 0;

                buttonPreviewControlRecipientFirst.Enabled = value != 0;
                buttonPreviewControlRecipientPrevious.Enabled = value != 0;
                buttonPreviewControlRecipientNext.Enabled = value != 0;
                buttonPreviewControlRecipientLast.Enabled = value != 0;
            }
        }

        public int RecipientsFieldsCount
        {
            set
            {
                dataGridViewRecipients.ColumnCount = value + 2;

                toolStripButtonRecipientsFieldsRemove.Enabled = value != 0;
                toolStripButtonRecipientsFieldsClear.Enabled = value != 0;
            }
        }

        public int SubjectsCount
        {
            set
            {
                listViewSubjects.VirtualListSize = value;

                int textWidth = TextRenderer.MeasureText(value.ToString(), new Font("Microsoft Sans Serif", 9)).Width;
                listViewSubjects.Columns[0].Width = value > 999 ? textWidth : 28;

                toolStripButtonSubjectsClear.Enabled = value != 0;
                toolStripButtonSubjectsExport.Enabled = value != 0;
                toolStripButtonSubjectsSort.Enabled = value != 0;
                toolStripButtonSubjectsDuplicate.Enabled = value != 0;
            }
        }

        public string HeaderGeneralIdentifier { set => comboBoxHeaderGeneralIdentifier.Text = value; }

        public bool HeaderGeneralDate { set => dateTimePickerHeaderGeneralDate.Checked = value; }

        public DateTime HeaderGeneralDateValue { set => dateTimePickerHeaderGeneralDate.Value = value; }

        public bool HeaderFrom { set => checkBoxHeaderFrom.Checked = value; }

        public string HeaderFromName { set => comboBoxHeaderFromName.Text = value; }

        public string HeaderFromEmail { set => comboBoxHeaderFromEmail.Text = value; }

        public bool HeaderReplyTo { set => checkBoxHeaderReplyTo.Checked = value; }

        public string HeaderReplyToName { set => comboBoxHeaderReplyToName.Text = value; }

        public string HeaderReplyToEmail { set => comboBoxHeaderReplyToEmail.Text = value; }

        public bool HeaderListUnsubscribe { set => checkBoxHeaderListUnsubscribe.Checked = value; }

        public string HeaderListUnsubscribeEmail { set => comboBoxHeaderListUnsubscribeEmail.Text = value; }

        public string HeaderListUnsubscribeUrl { set => textBoxHeaderListUnsubscribeUrl.Text = value; }

        public string HeaderAdvancedReturnPath { set => comboBoxHeaderAdvancedReturnPath.Text = value; }

        public MessagePriority HeaderAdvancedPriority { set => comboBoxHeaderAdvancedPriority.SelectedIndex = (int)value; }

        public MessageImportance HeaderAdvancedImportance { set => comboBoxHeaderAdvancedImportance.SelectedIndex = (int)value; }

        public bool MessageBodyHtml { set { toolStripCheckboxMessageBodyHtml.Checked = value; } }

        public string MessageBody { set => richTextBoxMessageBody.Text = value; }

        public int MessageAttachmentsCount
        {
            set
            {
                listViewMessageAttachments.VirtualListSize = value;
                toolStripButtonMessageAttachmentsClear.Enabled = value != 0;
            }
        }

        public string PreviewControlRecipient { set => labelPreviewControlRecipientValue.Text = value; }

        public string PreviewMailboxHeaderIdentifier { set => labelPreviewMailboxHeaderIdentifierValue.Text = value; }

        public string PreviewMailboxHeaderSubject { set => labelPreviewMailboxHeaderSubjectValue.Text = value; }

        public string PreviewMailboxHeaderSender { set => labelPreviewMailboxHeaderSenderValue.Text = value; }

        public string PreviewMailboxHeaderFrom { set => labelPreviewMailboxHeaderFromValue.Text = value; }

        public string PreviewMailboxHeaderTo { set => labelPreviewMailboxHeaderToValue.Text = value; }

        public string PreviewMailboxBodyText { set => textBoxPreviewMailboxBodyText.Text = value; }

        public string PreviewMailboxBodyHtml { set { htmlPanelPreviewMailboxBodyHtml.Text = value; } }

        public DateTime PreviewMailboxHeaderDate { set => labelPreviewMailboxHeaderDate.Text = value.ToString(); }

        public double PreviewMailboxMessageSize { set => labelPreviewMailboxMessageSize.Text = Math.Round(value, 2).ToString() + " Mb"; }

        public int PreviewMailboxAttachmentsCount { set => listViewPreviewMailboxAttachments.VirtualListSize = value; }

        public string PreviewOriginal { set => textBoxPreviewOriginal.Text = value; }

        public string CheckupMessageControlServer { set => labelCheckupMessageControlServerValue.Text = value; }

        public double CheckupMessageScore { set => labelCheckupMessageScoreValue.Text = value.ToString().Replace(",", "."); }

        public PostmarkResponse.RESULT CheckupMessageScoreResult
        {
            set
            {
                labelCheckupMessageScoreResult.Text = value == PostmarkResponse.RESULT.EXCELENT ? "Excelent" : (value == PostmarkResponse.RESULT.GOOD ? "Good" : (value == PostmarkResponse.RESULT.SPAM ? "Spam" : "-"));
                labelCheckupMessageScoreResult.ForeColor = value == PostmarkResponse.RESULT.EXCELENT ? Color.Green : (value == PostmarkResponse.RESULT.GOOD ? Color.Orange : (value == PostmarkResponse.RESULT.SPAM ? Color.Red : SystemColors.ControlText));
            }
        }

        public int CheckupMessageRulesCount { set => listViewCheckupMessageRules.VirtualListSize = value; }

        public string CheckupMessageReport { set => richTextBoxCheckupMessageReport.Text = value; }

        public int CheckupInspectionCount { set => listViewCheckupInspection.VirtualListSize = value; }

        public int ExtrasThreadsAmount { set => numericUpDownExtrasThreadsAmount.Value = value; }

        public Project.SELECTION ExtrasServersSelection { set => comboBoxExtrasServersSelection.SelectedIndex = (int)value; }

        public int ExtrasServersRotation { set => numericUpDownExtrasServersRotation.Value = value; }

        public int ExtrasServersDelay { set => numericUpDownExtrasServersDelay.Value = value; }

        public Project.SELECTION ExtrasProxiesSelection { set => comboBoxExtrasProxiesSelection.SelectedIndex = (int)value; }

        public int ExtrasProxiesRotation { set => numericUpDownExtrasProxiesRotation.Value = value; }

        public int ExtrasProxiesDelay { set => numericUpDownExtrasProxiesDelay.Value = value; }

        public int ExtrasBomberAmount { set => numericUpDownExtrasBomberAmount.Value = value; }


        public FormMain() //OK
        {
            m_rowScopeCommit = false;
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e) //OK
        {
            if (!SystemInformation.TerminalServerSession)
            {
                listViewServers.DoubleBuffering(true);
                listViewProxies.DoubleBuffering(true);
                dataGridViewRecipients.DoubleBuffering(true);
                listViewSubjects.DoubleBuffering(true);
                listViewCheckupInspection.DoubleBuffering(true);
            }

            comboBoxHeaderGeneralIdentifier.SelectedIndex = 0;
            comboBoxExtrasServersSelection.SelectedIndex = 0;
            comboBoxExtrasProxiesSelection.SelectedIndex = 0;
            textBoxPreviewMailboxBodyText.GotFocus += TextBoxPreviewMailboxBody_GotFocus;

            ListViewServers_Resize(listViewServers, new EventArgs());
            ListViewProxies_Resize(listViewProxies, new EventArgs());
            ListViewSubjects_Resize(listViewSubjects, new EventArgs());
            ListViewMessageAttachments_Resize(listViewMessageAttachments, new EventArgs());
            ListViewPreviewMailboxAttachments_Resize(listViewPreviewMailboxAttachments, new EventArgs());
            ListViewCheckupMessageRules_Resize(listViewCheckupMessageRules, new EventArgs());
            ListViewCheckupInspection_Resize(listViewCheckupInspection, new EventArgs());
        }

        public void SetController(MainController controller)
        {
            m_controller = controller;
        }

        private void ButtonMainNew_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to create a new project ?\n\nAll unsaved data in the current project will be lost.", "Information - Ultimate Mailer V3", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dialogResult == DialogResult.Yes)
            {
                listViewServers.SelectedIndices.Clear();
                listViewProxies.SelectedIndices.Clear();
                dataGridViewRecipients.ClearSelection();
                listViewSubjects.SelectedIndices.Clear();
                listViewMessageAttachments.SelectedIndices.Clear();

                m_controller.ProjectNew();
            }
        }

        private void ButtonMainOpen_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Open project - Ultimate Mailer V3";
                openFileDialog.DefaultExt = "umproj";
                openFileDialog.Filter = "Ultimate Mailer Project (.umproj)|*.umproj";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    listViewServers.SelectedIndices.Clear();
                    dataGridViewRecipients.ClearSelection();
                    listViewProxies.SelectedIndices.Clear();
                    listViewSubjects.SelectedIndices.Clear();
                    listViewMessageAttachments.SelectedIndices.Clear();

                    m_controller.ProjectOpen(openFileDialog.FileName);
                }
            }
        }

        private void ButtonMainSave_Click(object sender, EventArgs e) //OK
        {
            if (File.Exists(Project.Instance.Path))
            {
                m_controller.ProjectSave(Project.Instance.Path);
            }
            else
            {
                buttonMainSaveAs.PerformClick();
            }
        }

        private void ButtonMainSaveAs_Click(object sender, EventArgs e) //OK
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Save project as - Ultimate Mailer V3";
                saveFileDialog.DefaultExt = "umproj";
                saveFileDialog.Filter = "Ultimate Mailer Project (.umproj)|*.umproj";
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.FileName = Path.GetFileName(Project.Instance.Path);

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    m_controller.ProjectSave(saveFileDialog.FileName);
                }
            }
        }

        private void ButtonMainStart_Click(object sender, EventArgs e) //OK
        {
            Enabled = false;

            if (m_controller.InputValidation())
            {
                using (var formMailing = new FormMailing())
                using (new MailingController(formMailing))
                {
                    WindowState = FormWindowState.Minimized;
                    formMailing.ShowDialog();
                    WindowState = FormWindowState.Normal;
                }

                if (Properties.Settings.Default.SettingProjectBackup)
                {
                    buttonMainSave.PerformClick();
                }
            }

            Enabled = true;
        }

        private void ButtonMenu_Click(object sender, EventArgs e) //OK
        {
            buttonMenuServers.BackColor = Color.CornflowerBlue;
            buttonMenuServers.ForeColor = SystemColors.ControlLightLight;
            buttonMenuProxies.BackColor = Color.CornflowerBlue;
            buttonMenuProxies.ForeColor = SystemColors.ControlLightLight;
            buttonMenuRecipients.BackColor = Color.CornflowerBlue;
            buttonMenuRecipients.ForeColor = SystemColors.ControlLightLight;
            buttonMenuSubjects.BackColor = Color.CornflowerBlue;
            buttonMenuSubjects.ForeColor = SystemColors.ControlLightLight;
            buttonMenuHeader.BackColor = Color.CornflowerBlue;
            buttonMenuHeader.ForeColor = SystemColors.ControlLightLight;
            buttonMenuMessage.BackColor = Color.CornflowerBlue;
            buttonMenuMessage.ForeColor = SystemColors.ControlLightLight;
            buttonMenuPreview.BackColor = Color.CornflowerBlue;
            buttonMenuPreview.ForeColor = SystemColors.ControlLightLight;
            buttonMenuCheckup.BackColor = Color.CornflowerBlue;
            buttonMenuCheckup.ForeColor = SystemColors.ControlLightLight;
            buttonMenuExtras.BackColor = Color.CornflowerBlue;
            buttonMenuExtras.ForeColor = SystemColors.ControlLightLight;

            ((Button)sender).BackColor = SystemColors.Control;
            ((Button)sender).ForeColor = Color.CornflowerBlue;

            panelServers.Visible = sender == buttonMenuServers;
            panelProxies.Visible = sender == buttonMenuProxies;
            panelRecipients.Visible = sender == buttonMenuRecipients;
            panelSubjects.Visible = sender == buttonMenuSubjects;
            panelHeader.Visible = sender == buttonMenuHeader;
            panelMessage.Visible = sender == buttonMenuMessage;
            panelPreview.Visible = sender == buttonMenuPreview;
            panelCheckup.Visible = sender == buttonMenuCheckup;
            panelExtras.Visible = sender == buttonMenuExtras;
        }

        private void ButtonMenuOptions_Click(object sender, EventArgs e) //OK
        {
            using (var formOptions = new FormOptions())
            {
                formOptions.ShowDialog();
            }
        }

        private void ButtonMenuHelp_Click(object sender, EventArgs e) //OK
        {
            Process.Start(Constantes.URL_DOWNLOAD_EBOOK);
        }

        private void ToolStripButtonServersAdd_Click(object sender, EventArgs e) //OK
        {
            m_controller.ServersAdd();
        }

        private void ToolStripButtonServersRemove_Click(object sender, EventArgs e)
        {
            m_controller.ServersRemove();
            ListViewServers_SelectedIndexChanged(listViewServers, new EventArgs());

            if (listViewServers.SelectedIndices.Count != 0)
            {
                var eventArgs = new ListViewItemSelectionChangedEventArgs(null, listViewServers.SelectedIndices[0], true);
                ListViewServers_ItemSelectionChanged(listViewServers, eventArgs);
            }
        }

        private void ToolStripButtonServersClear_Click(object sender, EventArgs e) //OK
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to clear all servers ?", "Information - Ultimate Mailer V3", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dialogResult == DialogResult.Yes)
            {
                m_controller.ServersClear();
                ListViewServers_SelectedIndexChanged(listViewServers, new EventArgs());
            }
        }

        private async void ToolStripButtonServersImport_ClickAsync(object sender, EventArgs e) //OK
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Import servers - Ultimate Mailer V3";
                openFileDialog.DefaultExt = "txt";
                openFileDialog.Filter = "Text files (*.txt)|*.txt|Comma-separated values files (*.csv)|*.csv";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    await m_controller.ServersImportAsync(openFileDialog.FileName);
                }
            }
        }

        private async void ToolStripButtonServersExport_ClickAsync(object sender, EventArgs e) //OK
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Export servers - Ultimate Mailer V3";
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|Comma-separated values files (*.csv)|*.csv";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    await m_controller.ServersExportAsync(saveFileDialog.FileName);
                }
            }
        }

        private void TimerServersUI_Tick(object sender, EventArgs e) //OK
        {
            int count = Project.Instance.Servers.Count;
            int validatedCount = Project.Instance.Servers.Count(server => server.IsValidated != null);
            int validatedPercent = 100 * validatedCount / (count != 0 ? count : 1);

            toolStripLabelServersLoading.Text = validatedCount.ToString() + '/' + count.ToString() + " - " + validatedPercent.ToString() + '%';
        }

        private async void ToolStripButtonServersValidation_ClickAsync(object sender, EventArgs e) //OK
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to verify all servers ?\n\nDefective and misconfigured servers will be permanently deleted.", "Information - Ultimate Mailer V3", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dialogResult == DialogResult.Yes)
            {
                listViewServers.SelectedIndexChanged -= ListViewServers_SelectedIndexChanged;
                listViewServers.ItemSelectionChanged -= ListViewServers_ItemSelectionChanged;

                panelMain.Enabled = false;
                toolStripServers.Enabled = false;

                toolStripLabelServersLoading.Visible = true;
                splitContainerServers.Panel2Collapsed = true;

                using (var timerServersUI = new Timer())
                {
                    timerServersUI.Interval = TIMER_UPDATE_INTERVAL;
                    timerServersUI.Tick += new EventHandler(TimerServersUI_Tick);

                    timerServersUI.Start();
                    await m_controller.ServersValidationAsync();
                    timerServersUI.Stop();
                }

                splitContainerServers.Panel2Collapsed = false;
                toolStripLabelServersLoading.Visible = false;

                toolStripServers.Enabled = true;
                panelMain.Enabled = true;

                listViewServers.SelectedIndexChanged += ListViewServers_SelectedIndexChanged;
                listViewServers.ItemSelectionChanged += ListViewServers_ItemSelectionChanged;

                ListViewServers_SelectedIndexChanged(listViewServers, new EventArgs());
            }
        }

        private void ListViewServers_Resize(object sender, EventArgs e) //OK
        {
            int availableWidth = listViewServers.Width - listViewServers.Columns[0].Width - 4 - SystemInformation.VerticalScrollBarWidth;
            listViewServers.Columns[1].Width = availableWidth;
        }

        private void ListViewServers_SelectedIndexChanged(object sender, EventArgs e) //OK
        {
            toolStripButtonServersRemove.Enabled = listViewServers.SelectedIndices.Count != 0;
            panelServerUnselected.Visible = listViewServers.SelectedIndices.Count == 0;
            panelServerControl.Visible = listViewServers.SelectedIndices.Count != 0;

            if (listViewServers.SelectedIndices.Count == 0)
            {
                m_controller.ServersSelectionChanged(-1);
            }
        }

        private void ListViewServers_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) //OK
        {
            if (e.IsSelected)
            {
                m_controller.ServersSelectionChanged(e.ItemIndex);
            }
        }

        private void ListViewServers_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) //OK
        {
            Server server = Project.Instance.Servers[e.ItemIndex];

            var listViewItem = new ListViewItem((e.ItemIndex + 1).ToString())
            {
                UseItemStyleForSubItems = true
            };

            if (server.IsValidated != null)
            {
                listViewItem.BackColor = server.IsValidated == true ? Color.LightGreen : Color.Tomato;
            }

            listViewItem.SubItems.Add(new ListViewSubItem(listViewItem, server.Name));

            e.Item = listViewItem;
        }

        private void TextBoxServerConnectionGeneralHost_TextChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                listViewServers.BeginUpdate();
                m_controller.SelectedServer.ConnectionGeneralHost = textBoxServerConnectionGeneralHost.Text;
                listViewServers.EndUpdate();
            }
        }

        private void NumericUpDownServerConnectionGeneralPort_ValueChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                listViewServers.BeginUpdate();
                m_controller.SelectedServer.ConnectionGeneralPort = (int)numericUpDownServerConnectionGeneralPort.Value;
                listViewServers.EndUpdate();
            }
        }

        private void ComboBoxServerConnectionGeneralProtocol_SelectedIndexChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.ConnectionGeneralProtocol = Enum.GetValues(typeof(SslProtocols)).Cast<SslProtocols>().ToList()[comboBoxServerConnectionGeneralProtocol.SelectedIndex];
            }
        }

        private void CheckBoxServerConnectionAuthentication_CheckedChanged(object sender, EventArgs e) //OK
        {
            bool @checked = checkBoxServerConnectionAuthentication.Checked;
            checkBoxServerConnectionAuthentication.ForeColor = @checked ? Color.Green : Color.Red;
            panelServerConnectionAuthentication.Enabled = @checked;

            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.ConnectionAuthentication = @checked;
            }
        }

        private void TextBoxServerConnectionAuthenticationUsername_TextChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.ConnectionAuthenticationUsername = textBoxServerConnectionAuthenticationUsername.Text;
            }
        }

        private void TextBoxServerConnectionAuthenticationPassword_TextChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.ConnectionAuthenticationPassword = textBoxServerConnectionAuthenticationPassword.Text;
            }
        }

        private void CheckBoxServersConnectionAuthenticationPassword_CheckedChanged(object sender, EventArgs e) //OK
        {
            bool @checked = checkBoxServersConnectionAuthenticationPassword.Checked;
            textBoxServerConnectionAuthenticationPassword.UseSystemPasswordChar = !@checked;
        }

        private async void ButtonServerConnectionTest_ClickAsync(object sender, EventArgs e) //OK
        {
            buttonServerConnectionTest.Enabled = false;
            await m_controller.ServerConnectionTestAsync();
            buttonServerConnectionTest.Enabled = true;
        }

        private void NumericUpDownServerSettingsGeneralTimeout_ValueChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.SettingsGeneralTimeout = (int)numericUpDownServerSettingsGeneralTimeout.Value;
            }
        }

        private void NumericUpDownServerSettingsGeneralDelay_ValueChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.SettingsGeneralDelay = (int)numericUpDownServerSettingsGeneralDelay.Value;
            }
        }

        private void ComboBoxServerSettingsSenderName_TextChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.SettingsSenderName = comboBoxServerSettingsSenderName.Text;
            }
        }

        private void TextBoxServerSettingsSenderEmail_TextChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.SettingsSenderEmail = textBoxServerSettingsSenderEmail.Text;
            }
        }

        private void CheckBoxServerSettingsLimitSession_CheckedChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.SettingsLimitSession = checkBoxServerSettingsLimitSession.Checked;
            }
        }

        private void NumericUpDownServerSettingsLimitSession_ValueChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.SettingsLimitSessionValue = (int)numericUpDownServerSettingsLimitSession.Value;
            }
        }

        private void CheckBoxServerSettingsLimitHourly_CheckedChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.SettingsLimitHourly = checkBoxServerSettingsLimitHourly.Checked;
            }
        }

        private void NumericUpDownServerSettingsLimitHourly_ValueChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.SettingsLimitHourlyValue = (int)numericUpDownServerSettingsLimitHourly.Value;
            }
        }

        private void CheckBoxServerSettingsLimitDaily_CheckedChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.SettingsLimitDaily = checkBoxServerSettingsLimitDaily.Checked;
            }
        }

        private void NumericUpDownServerSettingsLimitDaily_ValueChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.SettingsLimitDailyValue = (int)numericUpDownServerSettingsLimitDaily.Value;
            }
        }

        private void CheckBoxServerSettingsLimitMonthly_CheckedChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.SettingsLimitMonthly = checkBoxServerSettingsLimitMonthly.Checked;
            }
        }

        private void NumericUpDownServerSettingsLimitMonthly_ValueChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.SettingsLimitMonthlyValue = (int)numericUpDownServerSettingsLimitMonthly.Value;
            }
        }

        private void NumericUpDownServerSettingsAdvancedSessionDelay_ValueChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.SettingsAdvancedSessionDelay = (int)numericUpDownServerSettingsAdvancedSessionDelay.Value;
            }
        }

        private void CheckBoxServerSettingsAdvancedPing_CheckedChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.SettingsAdvancedPing = checkBoxServerSettingsAdvancedPing.Checked;
            }
        }

        private void NumericUpDownServerSettingsAdvancedPing_ValueChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.SettingsAdvancedPingValue = (int)numericUpDownServerSettingsAdvancedPing.Value;
            }
        }

        private void CheckBoxServerSettingsAdvancedProxy_CheckedChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedServer.SettingsAdvancedProxy = checkBoxServerSettingsAdvancedProxy.Checked;
            }
        }

        private void ButtonServerReportsRefresh_Click(object sender, EventArgs e) //OK
        {
            buttonServerReportsRefresh.Enabled = false;
            m_controller.ServerReportsRefresh();
            buttonServerReportsRefresh.Enabled = true;
        }

        private void ToolStripButtonProxiesAdd_Click(object sender, EventArgs e) //OK
        {
            m_controller.ProxiesAdd();
        }

        private void ToolStripButtonProxiesRemove_Click(object sender, EventArgs e) //OK
        {
            m_controller.ProxiesRemove();
            ListViewProxies_SelectedIndexChanged(listViewProxies, new EventArgs());

            if (listViewProxies.SelectedIndices.Count != 0)
            {
                var eventArgs = new ListViewItemSelectionChangedEventArgs(null, listViewProxies.SelectedIndices[0], true);
                ListViewProxies_ItemSelectionChanged(listViewProxies, eventArgs);
            }
        }

        private void ToolStripButtonProxiesClear_Click(object sender, EventArgs e) //OK
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to clear all proxies ?", "Information - Ultimate Mailer V3", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dialogResult == DialogResult.Yes)
            {
                m_controller.ProxiesClear();
                ListViewProxies_SelectedIndexChanged(listViewProxies, new EventArgs());
            }
        }

        private async void ToolStripButtonProxiesImport_ClickAsync(object sender, EventArgs e) //OK
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Import proxies - Ultimate Mailer V3";
                openFileDialog.DefaultExt = "txt";
                openFileDialog.Filter = "Text files (*.txt)|*.txt|Comma-separated values files (*.csv)|*.csv";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    await m_controller.ProxiesImportAsync(openFileDialog.FileName);
                }
            }
        }

        private async void ToolStripButtonProxiesExport_ClickAsync(object sender, EventArgs e) //OK
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Export proxies - Ultimate Mailer V3";
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|Comma-separated values files (*.csv)|*.csv";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    await m_controller.ProxiesExportAsync(saveFileDialog.FileName);
                }
            }
        }

        private void TimerProxiesUI_Tick(object sender, EventArgs e) //OK
        {
            listViewProxies.Refresh();

            int count = Project.Instance.Proxies.Count;
            int validatedCount = Project.Instance.Proxies.Count(proxy => proxy.IsValidated != null);
            int validatedPercent = 100 * validatedCount / (count != 0 ? count : 1);

            toolStripLabelProxiesLoading.Text = validatedCount.ToString() + '/' + count.ToString() + " - " + validatedPercent.ToString() + '%';
        }

        private async void ToolStripButtonProxiesValidation_ClickAsync(object sender, EventArgs e) //OK
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to verify all proxies ?\n\nDefective and misconfigured proxies will be permanently deleted.", "Information - Ultimate Mailer V3", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dialogResult == DialogResult.Yes)
            {
                listViewProxies.SelectedIndexChanged -= ListViewProxies_SelectedIndexChanged;
                listViewProxies.ItemSelectionChanged -= ListViewProxies_ItemSelectionChanged;

                panelMain.Enabled = false;
                toolStripProxies.Enabled = false;

                toolStripLabelProxiesLoading.Visible = true;
                splitContainerProxies.Panel2Collapsed = true;

                using (var timerProxiesUI = new Timer())
                {
                    timerProxiesUI.Interval = TIMER_UPDATE_INTERVAL;
                    timerProxiesUI.Tick += new EventHandler(TimerProxiesUI_Tick);

                    timerProxiesUI.Start();
                    await m_controller.ProxiesValidationAsync();
                    timerProxiesUI.Stop();
                }

                splitContainerProxies.Panel2Collapsed = false;
                toolStripLabelProxiesLoading.Visible = false;

                toolStripProxies.Enabled = true;
                panelMain.Enabled = true;

                listViewProxies.SelectedIndexChanged += ListViewProxies_SelectedIndexChanged;
                listViewProxies.ItemSelectionChanged += ListViewProxies_ItemSelectionChanged;

                ListViewProxies_SelectedIndexChanged(listViewProxies, new EventArgs());
            }
        }

        private void ListViewProxies_Resize(object sender, EventArgs e) //OK
        {
            int availableWidth = listViewProxies.Width - listViewProxies.Columns[0].Width - 4 - SystemInformation.VerticalScrollBarWidth;
            listViewProxies.Columns[1].Width = availableWidth;
        }

        private void ListViewProxies_SelectedIndexChanged(object sender, EventArgs e) //OK
        {
            toolStripButtonProxiesRemove.Enabled = listViewProxies.SelectedIndices.Count != 0;
            panelProxyUnselected.Visible = listViewProxies.SelectedIndices.Count == 0;
            panelProxyControl.Visible = listViewProxies.SelectedIndices.Count != 0;

            if (listViewProxies.SelectedIndices.Count == 0)
            {
                m_controller.ProxiesSelectionChanged(-1);
            }
        }

        private void ListViewProxies_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) //OK
        {
            if (e.IsSelected)
            {
                m_controller.ProxiesSelectionChanged(e.ItemIndex);
            }
        }

        private void ListViewProxies_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) //OK
        {
            Proxy proxy = Project.Instance.Proxies[e.ItemIndex];

            var listViewItem = new ListViewItem((e.ItemIndex + 1).ToString())
            {
                UseItemStyleForSubItems = true
            };

            if (proxy.IsValidated != null)
            {
                listViewItem.BackColor = proxy.IsValidated == true ? Color.LightGreen : Color.Tomato;
            }

            listViewItem.SubItems.Add(new ListViewSubItem(listViewItem, proxy.Name));

            e.Item = listViewItem;
        }

        private void ComboBoxProxyGeneralType_SelectedIndexChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedProxy.GeneralType = (Proxy.TYPE)comboBoxProxyGeneralType.SelectedIndex;
            }
        }

        private void TextBoxProxyGeneralHost_TextChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                listViewProxies.BeginUpdate();
                m_controller.SelectedProxy.GeneralHost = textBoxProxyGeneralHost.Text;
                listViewProxies.EndUpdate();
            }
        }

        private void NumericUpDownProxyGeneralPort_ValueChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                listViewProxies.BeginUpdate();
                m_controller.SelectedProxy.GeneralPort = (int)numericUpDownProxyGeneralPort.Value;
                listViewProxies.EndUpdate();
            }
        }

        private void CheckBoxProxyAuthentication_CheckedChanged(object sender, EventArgs e) //OK
        {
            bool @checked = checkBoxProxyAuthentication.Checked;
            checkBoxProxyAuthentication.ForeColor = @checked ? Color.Green : Color.Red;
            panelProxyAuthentication.Enabled = @checked;

            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedProxy.Authentication = @checked;
            }
        }

        private void TextBoxProxyAuthenticationUsername_TextChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedProxy.AuthenticationUsername = textBoxProxyAuthenticationUsername.Text;
            }
        }

        private void TextBoxProxyAuthenticationPassword_TextChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                m_controller.SelectedProxy.AuthenticationPassword = textBoxProxyAuthenticationPassword.Text;
            }
        }

        private void CheckBoxProxyAuthenticationPassword_CheckedChanged(object sender, EventArgs e) //OK
        {
            textBoxProxyAuthenticationPassword.UseSystemPasswordChar = !checkBoxProxyAuthenticationPassword.Checked;
        }

        private async void ButtonProxyTest_ClickAsync(object sender, EventArgs e) //OK
        {
            buttonProxyTest.Enabled = false;
            await m_controller.ProxyConnectionTestAsync();
            buttonProxyTest.Enabled = true;
        }

        private void ToolStripTextBoxRecipients_TextChanged(object sender, EventArgs e) //OK
        {
            toolStripButtonRecipientsAdd.Enabled = Utils.IsValidEmail(toolStripTextBoxRecipientsEmail.Text);
        }

        private void ToolStripTextBoxRecipientsEmail_KeyPress(object sender, KeyPressEventArgs e) //OK
        {
            if (e.KeyChar == (char)13 && Utils.IsValidEmail(toolStripTextBoxRecipientsEmail.Text))
            {
                e.Handled = true;
                toolStripButtonRecipientsAdd.PerformClick();
            }
        }

        private void ToolStripButtonRecipientsAdd_Click(object sender, EventArgs e) //OK
        {
            m_controller.RecipientsAdd(toolStripTextBoxRecipientsEmail.Text);
            toolStripTextBoxRecipientsEmail.Text = string.Empty;
            toolStripTextBoxRecipientsEmail.Focus();
        }

        private void ToolStripButtonRecipientsRemove_Click(object sender, EventArgs e) //OK
        {
            m_controller.RecipientsRemove(dataGridViewRecipients.SelectedRows);
            dataGridViewRecipients.ClearSelection();
            dataGridViewRecipients.Refresh();
        }

        private void ToolStripButtonRecipientsClear_Click(object sender, EventArgs e) //OK
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to clear all recipients ?", "Information - Ultimate Mailer V3", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dialogResult == DialogResult.Yes)
            {
                m_controller.RecipientsClear();
            }
        }

        private async void ToolStripButtonRecipientsImport_ClickAsync(object sender, EventArgs e) //OK
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Import recipients - Ultimate Mailer V3";
                openFileDialog.DefaultExt = "txt";
                openFileDialog.Filter = "Text files (*.txt)|*.txt|Comma-separated values files (*.csv)|*.csv";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    toolStripButtonRecipientsImport.Enabled = false;
                    await m_controller.RecipientsImportAsync(openFileDialog.FileName);
                    toolStripButtonRecipientsImport.Enabled = true;
                }
            }
        }

        private async void ToolStripButtonRecipientsExport_ClickAsync(object sender, EventArgs e) //OK
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Export recipients - Ultimate Mailer V3";
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|Comma-separated values files (*.csv)|*.csv";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    toolStripButtonRecipientsExport.Enabled = false;
                    await m_controller.RecipientsExportAsync(saveFileDialog.FileName);
                    toolStripButtonRecipientsExport.Enabled = true;
                }
            }
        }

        private async void ToolStripButtonRecipientsExtract_ClickAsync(object sender, EventArgs e) //OK
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Extract emails - Ultimate Mailer V3";
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    toolStripButtonRecipientsExtract.Enabled = false;
                    await m_controller.RecipientsExtractAsync(openFileDialog.FileName);
                    toolStripButtonRecipientsExtract.Enabled = true;
                }
            }
        }

        private async void ToolStripButtonRecipientsSort_ClickAsync(object sender, EventArgs e) //OK
        {
            toolStripButtonRecipientsSort.Enabled = false;

            await m_controller.RecipientsSortAsync();
            dataGridViewRecipients.Refresh();
            toolStripButtonRecipientsSort.Enabled = true;
        }

        private async void ToolStripButtonRecipientsRemoveDuplicates_ClickAsync(object sender, EventArgs e) //OK
        {
            toolStripButtonRecipientsRemoveDuplicates.Enabled = false;
            await m_controller.RecipientsDuplicateAsync();
            dataGridViewRecipients.Refresh();
            toolStripButtonRecipientsRemoveDuplicates.Enabled = true;
        }

        private void TimerRecipientsUI_Tick(object sender, EventArgs e) //OK
        {
            dataGridViewRecipients.Refresh();

            int count = Project.Instance.Recipients.Count;
            int validatedCount = Project.Instance.Recipients.Count(recipient => recipient.IsValidated != null);
            int validatedPercent = 100 * validatedCount / (count != 0 ? count : 1);

            toolStripLabelRecipientsLoading.Text = validatedCount.ToString() + '/' + count.ToString() + " - " + validatedPercent.ToString() + '%';
        }

        private async void ToolStripButtonRecipientsValidation_ClickAsync(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to verify all recipients ?\n\nMalformed, non-existing and unwanted email will be permanently deleted.", "Information - Ultimate Mailer V3", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dialogResult == DialogResult.Yes)
            {
                toolStripTextBoxRecipientsEmail.KeyPress -= ToolStripTextBoxRecipientsEmail_KeyPress;
                toolStripTextBoxRecipientsEmail.TextChanged -= ToolStripTextBoxRecipients_TextChanged;
                dataGridViewRecipients.SelectionChanged -= DataGridViewRecipients_SelectionChanged;

                panelMain.Enabled = false;
                toolStripRecipients.Enabled = false;
                toolStripLabelRecipientsLoading.Visible = true;

                using (var timerRecipientsUI = new Timer())
                {
                    timerRecipientsUI.Interval = TIMER_UPDATE_INTERVAL;
                    timerRecipientsUI.Tick += new EventHandler(TimerRecipientsUI_Tick);

                    timerRecipientsUI.Start();
                    await m_controller.RecipientsValidationAsync();
                    timerRecipientsUI.Stop();
                }

                toolStripLabelRecipientsLoading.Visible = false;
                toolStripRecipients.Enabled = true;
                panelMain.Enabled = true;

                dataGridViewRecipients.SelectionChanged += DataGridViewRecipients_SelectionChanged;
                DataGridViewRecipients_SelectionChanged(dataGridViewRecipients, new EventArgs());

                toolStripTextBoxRecipientsEmail.KeyPress += ToolStripTextBoxRecipientsEmail_KeyPress;
                toolStripTextBoxRecipientsEmail.TextChanged += ToolStripTextBoxRecipients_TextChanged;
            }
        }

        private void DataGridViewRecipients_SelectionChanged(object sender, EventArgs e) //OK
        {
            toolStripButtonRecipientsRemove.Enabled = dataGridViewRecipients.SelectedRows.Count > 0;

            if (dataGridViewRecipients.SelectedRows.Count > 0)
            {
                m_controller.SelectedRecipientChange(dataGridViewRecipients.SelectedRows[0].Index);
            }
        }

        private void DataGridViewRecipients_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) //OK
        {
            if (Project.Instance.Recipients.Count == 0) return;

            Recipient recipient = Project.Instance.Recipients[e.RowIndex];

            if (recipient.IsValidated != null)
            {
                DataGridViewCellStyle cellStyle = dataGridViewRecipients.Rows[e.RowIndex].DefaultCellStyle;
                cellStyle.BackColor = recipient.IsValidated == true ? Color.LightGreen : Color.Tomato;
            }

            if (e.ColumnIndex == 0)
            {
                e.Value = recipient.Index + 1;
            }
            else if (e.ColumnIndex == 1)
            {
                e.Value = recipient.Email;
            }
            else
            {
                e.Value = recipient.Fields[e.ColumnIndex - 2];
            }
        }

        private void DataGridViewRecipients_CellValuePushed(object sender, DataGridViewCellValueEventArgs e) //OK
        {
            if (m_controller.SelectedRecipient != null)
            {
                m_controller.SelectedRecipient.Fields[e.ColumnIndex - 2] = e.Value != null ? e.Value.ToString() : string.Empty;
            }
        }

        private void DataGridViewRecipients_RowDirtyStateNeeded(object sender, QuestionEventArgs e) //OK
        {
            if (!m_rowScopeCommit)
            {
                e.Response = dataGridViewRecipients.IsCurrentCellDirty;
            }
        }

        private void DataGridViewRecipients_ColumnAdded(object sender, DataGridViewColumnEventArgs e) //OK
        {
            e.Column.HeaderText = "Field n°" + (e.Column.Index - 1).ToString();
            e.Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        private void ToolStripButtonRecipientsFieldAdd_Click(object sender, EventArgs e) //OK
        {
            m_controller.RecipientFieldsAdd();
        }

        private void ToolStripButtonRecipientsFieldsRemove_Click(object sender, EventArgs e) //OK
        {
            m_controller.RecipientFieldsRemove();
        }

        private void ToolStripButtonRecipientsFieldsClear_Click(object sender, EventArgs e) //OK
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to clear all fields ?", "Information - Ultimate Mailer V3", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dialogResult == DialogResult.Yes)
            {
                m_controller.RecipientsFieldsClear();
            }
        }

        private void ToolStripTextBoxSubjects_TextChanged(object sender, EventArgs e) //OK
        {
            toolStripButtonSubjectsAdd.Enabled = !string.IsNullOrWhiteSpace(toolStripTextBoxSubjects.Text);
        }

        private void ToolStripTextBoxSubjects_KeyPress(object sender, KeyPressEventArgs e) //OK
        {
            if (e.KeyChar == (char)13 && !string.IsNullOrWhiteSpace(toolStripTextBoxSubjects.Text))
            {
                e.Handled = true;
                toolStripButtonSubjectsAdd.PerformClick();
            }
        }

        private void ToolStripButtonSubjectsAdd_Click(object sender, EventArgs e) //OK
        {
            m_controller.SubjectsAdd(toolStripTextBoxSubjects.Text);
            toolStripTextBoxSubjects.Text = string.Empty;
            toolStripTextBoxSubjects.Focus();
        }

        private void ToolStripButtonSubjectsRemove_Click(object sender, EventArgs e) //OK
        {
            m_controller.SubjectsRemove(listViewSubjects.SelectedIndices);
            listViewSubjects.SelectedIndices.Clear();
            ListViewSubjects_SelectedIndexChanged(listViewSubjects, new EventArgs());
        }

        private void ToolStripButtonSubjectsClear_Click(object sender, EventArgs e) //OK
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to clear all subjects ?", "Information - Ultimate Mailer V3", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dialogResult == DialogResult.Yes)
            {
                m_controller.SubjectsClear();
                ListViewSubjects_SelectedIndexChanged(listViewSubjects, new EventArgs());
            }
        }

        private async void ToolStripButtonSubjectsImport_ClickAsync(object sender, EventArgs e) //OK
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Import subjects - Ultimate Mailer V3";
                openFileDialog.DefaultExt = "txt";
                openFileDialog.Filter = "Text files (*.txt)|*.txt|Comma-separated values files (*.csv)|*.csv";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    toolStripButtonSubjectsImport.Enabled = false;
                    await m_controller.ImportSubjectsAsync(openFileDialog.FileName);
                    toolStripButtonSubjectsImport.Enabled = true;
                }
            }
        }

        private async void ToolStripButtonSubjectsExport_ClickAsync(object sender, EventArgs e) //OK
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Export subjects - Ultimate Mailer V3";
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|Comma-separated values files (*.csv)|*.csv";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    toolStripButtonSubjectsExport.Enabled = false;
                    await m_controller.ExportSubjectsAsync(saveFileDialog.FileName);
                    toolStripButtonSubjectsExport.Enabled = true;
                }
            }
        }

        private void ListViewSubjects_SelectedIndexChanged(object sender, EventArgs e) //OK
        {
            toolStripButtonSubjectsRemove.Enabled = listViewSubjects.SelectedIndices.Count > 0;
        }

        private void ListViewSubjects_Resize(object sender, EventArgs e) //OK
        {
            int availableWidth = listViewSubjects.Width - listViewSubjects.Columns[0].Width - 4 - SystemInformation.VerticalScrollBarWidth;
            listViewSubjects.Columns[1].Width = availableWidth;
        }

        private void ListViewSubjects_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) //OK
        {
            string subject = Project.Instance.Subjects[e.ItemIndex];
            var listViewItem = new ListViewItem((e.ItemIndex + 1).ToString());
            listViewItem.SubItems.Add(new ListViewSubItem(listViewItem, subject));
            e.Item = listViewItem;
        }

        private void ComboBoxHeaderGeneralIdentifier_TextChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.HeaderGeneralIdentifier = comboBoxHeaderGeneralIdentifier.Text;
            }
        }

        private void DateTimePickerHeaderGeneralDate_ValueChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.HeaderGeneralDate = dateTimePickerHeaderGeneralDate.Checked;
                Project.Instance.HeaderGeneralDateValue = dateTimePickerHeaderGeneralDate.Value;
            }
        }

        private void CheckBoxHeaderFrom_CheckedChanged(object sender, EventArgs e) //OK
        {
            bool @checked = checkBoxHeaderFrom.Checked;
            checkBoxHeaderFrom.ForeColor = @checked ? Color.Green : Color.Red;
            panelHeaderFrom.Enabled = @checked;

            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.HeaderFrom = @checked;
            }
        }

        private void ComboBoxHeaderFromName_TextChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.HeaderFromName = comboBoxHeaderFromName.Text;
            }
        }

        private void ComboBoxHeaderFromEmail_TextChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.HeaderFromEmail = comboBoxHeaderFromEmail.Text;
            }
        }

        private void CheckBoxHeaderReplyTo_CheckedChanged(object sender, EventArgs e) //OK
        {
            bool @checked = checkBoxHeaderReplyTo.Checked;
            checkBoxHeaderReplyTo.ForeColor = @checked ? Color.Green : Color.Red;
            panelHeaderReplyTo.Enabled = @checked;

            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.HeaderReplyTo = checkBoxHeaderReplyTo.Checked;
            }
        }

        private void ComboBoxHeaderReplyToName_TextChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.HeaderReplyToName = comboBoxHeaderReplyToName.Text;
            }
        }

        private void ComboBoxHeaderReplyToEmail_TextChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.HeaderReplyToEmail = comboBoxHeaderReplyToEmail.Text;
            }
        }

        private void CheckBoxHeaderListUnsubscribe_CheckedChanged(object sender, EventArgs e) //OK
        {
            bool @checked = checkBoxHeaderListUnsubscribe.Checked;
            checkBoxHeaderListUnsubscribe.ForeColor = @checked ? Color.Green : Color.Red;
            panelHeaderListUnsubscribe.Enabled = @checked;

            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.HeaderListUnsubscribe = @checked;
            }
        }

        private void ComboBoxHeaderListUnsubscribeEmail_TextChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.HeaderListUnsubscribeEmail = comboBoxHeaderListUnsubscribeEmail.Text;
            }
        }

        private void TextBoxHeaderListUnsubscribeUrl_TextChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.HeaderListUnsubscribeUrl = textBoxHeaderListUnsubscribeUrl.Text;
            }
        }

        private void ComboBoxHeaderAdvancedReturnPath_TextChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.HeaderAdvancedReturnPath = comboBoxHeaderAdvancedReturnPath.Text;
            }
        }

        private void ComboBoxHeaderAdvancedPriority_SelectedIndexChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.HeaderAdvancedPriority = (MessagePriority)comboBoxHeaderAdvancedPriority.SelectedIndex;
            }
        }

        private void ComboBoxHeaderAdvancedImportance_SelectedIndexChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.HeaderAdvancedImportance = (MessageImportance)comboBoxHeaderAdvancedImportance.SelectedIndex;
            }
        }

        private void ToolStripButtonMessageBodyOpen_Click(object sender, EventArgs e) //OK
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Open message body - Ultimate Mailer V3";
                openFileDialog.DefaultExt = "txt";
                openFileDialog.Filter = "Text file (*.txt)|*.txt|Hypertext Markup Language (*.html)|*.html";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    m_controller.MessageBodyOpen(openFileDialog.FileName);
                }
            }
        }

        private void ToolStripButtonMessageBodySave_Click(object sender, EventArgs e) //OK
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Save message body - Ultimate Mailer V3";
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.Filter = "Text file (*.txt)|*.txt|Hypertext Markup Language (*.html)|*.html";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    m_controller.MessageBodySave(saveFileDialog.FileName);
                }
            }
        }

        private void ToolStripButtonMessageBodyUndo_Click(object sender, EventArgs e) //OK
        {
            richTextBoxMessageBody.Undo();
        }

        private void ToolStripButtonMessageBodyRedo_Click(object sender, EventArgs e) //OK
        {
            richTextBoxMessageBody.Redo();
        }

        private void ToolStripButtonMessageBodyOutdent_Click(object sender, EventArgs e) //OK
        {
            richTextBoxMessageBody.Focus();

            int selectionCharIndex = richTextBoxMessageBody.SelectionStart - richTextBoxMessageBody.GetFirstCharIndexOfCurrentLine();
            int selectionStart = richTextBoxMessageBody.SelectionStart;
            int selectionLenght = richTextBoxMessageBody.SelectionLength;

            int lineSelectionStart = richTextBoxMessageBody.GetLineFromCharIndex(selectionStart);
            int lineSelectionEnd = richTextBoxMessageBody.GetLineFromCharIndex(selectionStart + selectionLenght);

            string[] lines = richTextBoxMessageBody.Lines;

            if (lineSelectionStart != lineSelectionEnd)
            {
                for (int i = lineSelectionStart; i <= lineSelectionEnd; ++i)
                {
                    if (richTextBoxMessageBody.Lines[i].StartsWith(" "))
                    {
                        int whiteSpaceCount = lines[i].Length - lines[i].TrimStart(' ').Length;
                        double indentRatio = (whiteSpaceCount / (double)4) - (int)(whiteSpaceCount / (double)4);
                        int removeCharCount = indentRatio.Equals(0) ? 4 : (int)(indentRatio * 4);

                        lines[i] = lines[i].Remove(0, removeCharCount);

                        if (i == lineSelectionStart && selectionCharIndex >= whiteSpaceCount)
                        {
                            selectionStart -= removeCharCount;
                            selectionLenght += removeCharCount;
                        }
                        else if (i == lineSelectionStart && selectionCharIndex > whiteSpaceCount - removeCharCount)
                        {
                            selectionStart -= selectionCharIndex - (whiteSpaceCount - removeCharCount);
                            selectionLenght += selectionCharIndex - (whiteSpaceCount - removeCharCount);
                        }
                        else
                        {
                            selectionLenght -= removeCharCount;
                        }
                    }
                }
            }
            else if (selectionCharIndex != 0 && richTextBoxMessageBody.Lines.Length != 0 && richTextBoxMessageBody.Lines[lineSelectionStart].ToCharArray()[selectionCharIndex - 1] == ' ')
            {
                string lineToSelection = lines[lineSelectionStart].Substring(0, selectionCharIndex);
                int whiteSpaceCount = lineToSelection.Length - lineToSelection.TrimEnd(' ').Length;
                double indentRatio = (selectionCharIndex / (double)4) - (int)(selectionCharIndex / (double)4);
                int removeCharCount = indentRatio.Equals(0) ? 4 : (int)(indentRatio * 4);

                int removeStart = selectionCharIndex - (whiteSpaceCount >= removeCharCount ? removeCharCount : whiteSpaceCount);
                int removeEnd = whiteSpaceCount >= removeCharCount ? removeCharCount : whiteSpaceCount;

                lines[lineSelectionStart] = lines[lineSelectionStart].Remove(removeStart, removeEnd);

                selectionStart -= removeEnd;
            }

            richTextBoxMessageBody.Lines = lines;

            richTextBoxMessageBody.SelectionStart = selectionStart;
            richTextBoxMessageBody.SelectionLength = selectionLenght;
        }

        private void ToolStripButtonMessageBodyIndent_Click(object sender, EventArgs e) //OK
        {
            richTextBoxMessageBody.Focus();

            int currentColumnIndex = richTextBoxMessageBody.SelectionStart - richTextBoxMessageBody.GetFirstCharIndexOfCurrentLine();
            int selectionStart = richTextBoxMessageBody.SelectionStart;
            int selectionLenght = richTextBoxMessageBody.SelectionLength;

            int lineSelectionStart = richTextBoxMessageBody.GetLineFromCharIndex(selectionStart);
            int lineSelectionEnd = richTextBoxMessageBody.GetLineFromCharIndex(selectionStart + selectionLenght);

            string[] lines = richTextBoxMessageBody.Lines;

            if (lineSelectionStart != lineSelectionEnd)
            {
                for (int i = lineSelectionStart; i <= lineSelectionEnd; ++i)
                {
                    lines[i] = new string(' ', 4) + lines[i];
                    selectionLenght += 4;
                }
            }
            else if (richTextBoxMessageBody.Lines.Length != 0)
            {
                string lineToPosition = lines[lineSelectionStart].Substring(0, currentColumnIndex);
                string lineFromPosition = (lines[lineSelectionStart].Length != currentColumnIndex ? lines[lineSelectionStart].Substring(currentColumnIndex, lines[lineSelectionStart].Length - currentColumnIndex) : "");
                double indentRatio = 1 - ((currentColumnIndex / (double)4) - (int)(currentColumnIndex / (double)4));
                int spaceAddCount = (int)(indentRatio * 4);

                lines[lineSelectionStart] = lineToPosition + new string(' ', spaceAddCount) + lineFromPosition;
                selectionStart += spaceAddCount;
            }
            else
            {
                lines = new string[1];
                lines[0] = new string(' ', 4);
                selectionStart += 4;
            }

            richTextBoxMessageBody.Lines = lines;

            richTextBoxMessageBody.SelectionStart = selectionStart;
            richTextBoxMessageBody.SelectionLength = selectionLenght;
        }

        private void ToolStripCheckboxMessageHtml_CheckStateChanged(object sender, EventArgs e) //OK
        {
            var @checked = toolStripCheckboxMessageBodyHtml.Checked;

            toolStripCheckboxMessageBodyHtml.Text = @checked ? "✔" : "✘";
            toolStripCheckboxMessageBodyHtml.ToolTipText = @checked ? "Enabled" : "Disabled";
            toolStripCheckboxMessageBodyHtml.ForeColor = @checked ? Color.Green : Color.Red;

            richTextBoxMessageBody.Font = new Font(@checked ? "Courier New" : "Microsoft Sans Serif", 10);
            richTextBoxMessageBody.WordWrap = !@checked;

            panelPreviewMailboxBodyText.Visible = !@checked;
            htmlPanelPreviewMailboxBodyHtml.Visible = @checked;

            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.MessageBodyHtml = @checked;
            }
        }

        private void RichTextBoxMessageBody_TextChanged(object sender, EventArgs e) //OK
        {
            toolStripButtonMessageBodyUndo.Enabled = richTextBoxMessageBody.CanUndo;
            toolStripButtonMessageBodyRedo.Enabled = richTextBoxMessageBody.CanRedo;

            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.MessageBody = richTextBoxMessageBody.Text;
            }
        }

        private void ToolStripButtonMessageAttachmentsAdd_Click(object sender, EventArgs e) //OK
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Open attachment - Ultimate Mailer V3";
                openFileDialog.Filter = "All files|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    m_controller.MessageAttachmentAdd(openFileDialog.FileName);
                }
            }
        }

        private void ToolStripButtonMessageAttachmentsRemove_Click(object sender, EventArgs e) //OK
        {
            m_controller.MessageAttachmentRemove(listViewMessageAttachments.SelectedIndices);
            ListViewMessageAttachments_SelectedIndexChanged(listViewMessageAttachments, new EventArgs());
        }

        private void ToolStripButtonMessageAttachmentsClear_Click(object sender, EventArgs e) //OK
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to clear all attachments ?", "Information - Ultimate Mailer V3", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dialogResult == DialogResult.Yes)
            {
                m_controller.MessageAttachmentsClear();
                ListViewMessageAttachments_SelectedIndexChanged(listViewMessageAttachments, new EventArgs());
            }
        }

        private void ListViewMessageAttachments_Resize(object sender, EventArgs e) //OK
        {
            int availableWidth = listViewMessageAttachments.Width - listViewMessageAttachments.Columns[0].Width - 4 - SystemInformation.VerticalScrollBarWidth;
            listViewMessageAttachments.Columns[1].Width = availableWidth;
        }

        private void ListViewMessageAttachments_SelectedIndexChanged(object sender, EventArgs e) //OK
        {
            toolStripButtonMessageAttachmentsRemove.Enabled = listViewMessageAttachments.SelectedIndices.Count != 0;
        }

        private void ListViewMessageAttachments_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) //OK
        {
            string attachment = Project.Instance.MessageAttachments[e.ItemIndex];
            var listViewItem = new ListViewItem((e.ItemIndex + 1).ToString());
            listViewItem.SubItems.Add(new ListViewSubItem(listViewItem, attachment));
            e.Item = listViewItem;
        }

        private void ButtonPreviewControlRecipientFirst_Click(object sender, EventArgs e) //OK
        {
            m_controller.PreviewRecipientFirst();
        }

        private void ButtonPreviewControlRecipientPrevious_Click(object sender, EventArgs e) //OK
        {
            m_controller.PreviewRecipientPrevious();
        }

        private void ButtonPreviewControlRecipientNext_Click(object sender, EventArgs e) //OK
        {
            m_controller.PreviewRecipientNext();
        }

        private void ButtonPreviewControlRecipientLast_Click(object sender, EventArgs e) //OK
        {
            m_controller.PreviewRecipientLast();
        }

        private void ButtonPreviewRefresh_Click(object sender, EventArgs e) //OK
        {
            m_controller.PreviewRefresh();
        }

        private void TextBoxPreviewMailboxBody_GotFocus(object sender, EventArgs e) //OK
        {
            NativeMethods.HideCaret(textBoxPreviewMailboxBodyText.Handle);
        }

        private void ListViewPreviewMailboxAttachments_Resize(object sender, EventArgs e) //OK
        {
            int availableWidth = listViewPreviewMailboxAttachments.Width - listViewPreviewMailboxAttachments.Columns[1].Width - 4 - SystemInformation.VerticalScrollBarWidth;
            listViewPreviewMailboxAttachments.Columns[0].Width = availableWidth;
        }

        private void ListViewPreviewMailboxAttachments_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) //OK
        {
            string attachment = m_controller.PreviewMailboxAttachments[e.ItemIndex];

            var listViewItem = new ListViewItem(Path.GetFileName(attachment));

            listViewItem.SubItems.Add(new ListViewSubItem(listViewItem, "Download"));
            listViewItem.UseItemStyleForSubItems = false;

            listViewItem.SubItems[1].ForeColor = Color.RoyalBlue;
            listViewItem.SubItems[1].Font = new Font(listViewItem.Font, FontStyle.Underline);

            e.Item = listViewItem;
        }

        private void ButtonCheckupMessageControlServerFirst_Click(object sender, EventArgs e) //OK
        {
            m_controller.CheckupMessageServerFirst();
        }

        private void ButtonCheckupMessageControlServerPrevious_Click(object sender, EventArgs e) //OK
        {
            m_controller.CheckupMessageServerPrevious();
        }

        private void ButtonCheckupMessageControlServerNext_Click(object sender, EventArgs e) //OK
        {
            m_controller.CheckupMessageServerNext();
        }

        private void ButtonCheckupMessageControlServerLast_Click(object sender, EventArgs e) //OK
        {
            m_controller.CheckupMessageServerLast();
        }

        private async void ButtonCheckupMessageCheck_Click(object sender, EventArgs e) //OK
        {
            buttonCheckupMessageCheck.Visible = false;
            pictureBoxCheckupMessageLoading.Visible = true;

            await m_controller.CheckupMessageCheckAsync();

            pictureBoxCheckupMessageLoading.Visible = false;
            buttonCheckupMessageCheck.Visible = true;
        }


        private void ListViewCheckupMessageRules_Resize(object sender, EventArgs e) //OK
        {
            int availableWidth = listViewCheckupMessageRules.Width - listViewCheckupMessageRules.Columns[0].Width - 4 - SystemInformation.VerticalScrollBarWidth;
            listViewCheckupMessageRules.Columns[1].Width = availableWidth;
        }

        private void ListViewCheckupMessageRules_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) //OK
        {
            PostmarkResponse.Rule rule = m_controller.CheckupMessageRules[e.ItemIndex];

            var listViewItem = new ListViewItem(rule.Score.ToString())
            {
                ForeColor = rule.Score < 0 ? Color.LimeGreen : (rule.Score == 0 ? SystemColors.ControlText : Color.Red)
            };

            listViewItem.SubItems.Add(new ListViewSubItem(listViewItem, rule.Description));

            e.Item = listViewItem;
        }

        private void ButtonCheckupMessageReport_Click(object sender, EventArgs e) //OK
        {
            listViewCheckupMessageRules.Visible = false;
            richTextBoxCheckupMessageReport.Visible = true;

            buttonCheckupMessageRules.BackColor = SystemColors.ControlDarkDark;
            buttonCheckupMessageReport.BackColor = SystemColors.ControlDark;
        }

        private void ButtonCheckupMessageRules_Click(object sender, EventArgs e) //OK
        {
            richTextBoxCheckupMessageReport.Visible = false;
            listViewCheckupMessageRules.Visible = true;

            buttonCheckupMessageReport.BackColor = SystemColors.ControlDarkDark;
            buttonCheckupMessageRules.BackColor = SystemColors.ControlDark;
        }

        private void ButtonCheckupInspectionCheck_Click(object sender, EventArgs e) //OK
        {
            buttonCheckupInspectionCheck.Enabled = false;
            buttonCheckupInspectionCheck.Visible = false;
            pictureBoxCheckupInspectionLoading.Visible = true;

            m_controller.InputInspection();

            pictureBoxCheckupInspectionLoading.Visible = false;
            buttonCheckupInspectionCheck.Visible = true;
            buttonCheckupInspectionCheck.Enabled = true;
        }

        private void ListViewCheckupInspection_Resize(object sender, EventArgs e) //OK
        {
            int availableWidth = listViewCheckupInspection.Width - 4 - SystemInformation.VerticalScrollBarWidth;
            listViewCheckupInspection.Columns[0].Width = availableWidth;
        }

        private void ListViewCheckupInspection_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e) //OK
        {
            Inspection inspection = m_controller.CheckupInspections[e.ItemIndex];

            var listViewItem = new ListViewItem(inspection.Verbose == Inspection.VERBOSE.INFO ? "Info : " :
                inspection.Verbose == Inspection.VERBOSE.WARNING ? "Warning : " :
                inspection.Verbose == Inspection.VERBOSE.DANGER ? "Danger : " : string.Empty);

            listViewItem.Text += inspection.Message;

            listViewItem.ForeColor = inspection.Verbose == Inspection.VERBOSE.INFO ? Color.Blue :
                inspection.Verbose == Inspection.VERBOSE.WARNING ? Color.Orange :
                inspection.Verbose == Inspection.VERBOSE.DANGER ? Color.Red : Color.Black;

            e.Item = listViewItem;
        }

        private void NumericUpDownExtrasThreadsAmount_ValueChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.ExtrasThreadsAmount = (int)numericUpDownExtrasThreadsAmount.Value;
            }
        }

        private void ComboBoxExtrasServersSelection_SelectedIndexChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.ExtrasServersSelection = (Project.SELECTION)comboBoxExtrasServersSelection.SelectedIndex;
            }
        }

        private void NumericUpDownExtrasServersRotation_ValueChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.ExtrasServersRotation = (int)numericUpDownExtrasServersRotation.Value;
            }
        }

        private void NumericUpDownExtrasServersDelay_ValueChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.ExtrasServersDelay = (int)numericUpDownExtrasServersDelay.Value;
            }
        }

        private void ComboBoxExtrasProxiesSelection_SelectedIndexChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.ExtrasProxiesSelection = (Project.SELECTION)comboBoxExtrasProxiesSelection.SelectedIndex;
            }
        }

        private void NumericUpDownExtrasProxiesRotation_ValueChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.ExtrasProxiesRotation = (int)numericUpDownExtrasProxiesRotation.Value;
            }
        }

        private void NumericUpDownExtrasProxiesDelay_ValueChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.ExtrasProxiesDelay = (int)numericUpDownExtrasProxiesDelay.Value;
            }
        }

        private void NumericUpDownExtrasBomberAmount_ValueChanged(object sender, EventArgs e) //OK
        {
            if (m_controller.AllowModelsUpdate)
            {
                Project.Instance.ExtrasBomberAmount = (int)numericUpDownExtrasBomberAmount.Value;
            }
        }

        private void ToolStripDropDownButtonSpintax_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) //OK
        {
            var toolStripItem = (ToolStripItem)sender;

            if (toolStripDropDownButtonMessageBodySpintax.DropDownItems.IndexOf(toolStripItem) != -1)
            {
                richTextBoxMessageBody.SelectedText = e.ClickedItem.Text;
            }
            else if (toolStripDropDownButtonSubjectsSpintax.DropDownItems.IndexOf(toolStripItem) != -1)
            {
                toolStripTextBoxSubjects.SelectedText = e.ClickedItem.Text;
            }
        }
        private void buttonMenuSmtp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Constantes.URL_SMTP_BUILDER); 
        }
    }
}