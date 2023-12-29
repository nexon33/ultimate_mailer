using System;
using System.Windows.Forms;

using ultimate.mailer.Properties;

namespace ultimate.mailer
{
    public partial class FormOptions : Form
    {
        public FormOptions()
        {
            MaximizeBox = false;
            InitializeComponent();
        }

        private void FormOptions_Load(object sender, EventArgs e)
        {
            checkBoxProjectBackup.Checked = Settings.Default.SettingProjectBackup;

            checkBoxServersCertificate.Checked = Settings.Default.SettingServersCertificate;
            checkBoxServersAuthentication.Checked = Settings.Default.SettingServersAuthentication;

            numericUpDownServersReportsValidity.Value = Settings.Default.SettingServersReportsValidity;
            numericUpDownServersReportsInterval.Value = Settings.Default.SettingServersReportsInterval;

            numericUpDownProxiesValidationTimeout.Value = Settings.Default.SettingProxiesValidationTimeout;

            comboBoxCheckupRecipientsLevel.SelectedIndexChanged -= ComboBoxCheckupRecipientsLevel_SelectedIndexChanged;
            comboBoxCheckupRecipientsLevel.SelectedIndex = Settings.Default.SettingRecipientsValidationLevel;
            comboBoxCheckupRecipientsLevel.SelectedIndexChanged += ComboBoxCheckupRecipientsLevel_SelectedIndexChanged;

            numericUpDownRecipientsValidationTimeout.Value = Settings.Default.SettingRecipientsValidationTimeout;
            checkBoxCheckupRecipientsLevelRole.Checked = Settings.Default.SettingRecipientsValidationLevelRole;
            checkBoxCheckupRecipientsLevelDisposable.Checked = Settings.Default.SettingRecipientsValidationLevelDisposable;
        }

        private void CheckBoxProjectBackup_CheckedChanged(object sender, EventArgs e)
        {
            buttonOptionsApply.Enabled = OptionsChanged;
        }

        private void CheckBoxInspectionSpam_CheckedChanged(object sender, EventArgs e)
        {
            buttonOptionsApply.Enabled = OptionsChanged;
        }

        private void CheckBoxInspectionThreads_CheckedChanged(object sender, EventArgs e)
        {
            buttonOptionsApply.Enabled = OptionsChanged;
        }

        private void CheckBoxServersCertificate_CheckedChanged(object sender, EventArgs e)
        {
            buttonOptionsApply.Enabled = OptionsChanged;
        }

        private void CheckBoxServersAuthentication_CheckedChanged(object sender, EventArgs e)
        {
            buttonOptionsApply.Enabled = OptionsChanged;
        }

        private void NumericUpDownOptionsReportValidity_ValueChanged(object sender, EventArgs e)
        {
            buttonOptionsApply.Enabled = OptionsChanged;
        }

        private void NumericUpDownOptionsReportDuration_ValueChanged(object sender, EventArgs e)
        {
            buttonOptionsApply.Enabled = OptionsChanged;
        }

        private void NumericUpDownProxiesValidationTimeout_ValueChanged(object sender, EventArgs e)
        {
            buttonOptionsApply.Enabled = OptionsChanged;
        }

        private void ComboBoxCheckupRecipientsLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCheckupRecipientsLevel.SelectedIndex >= 4)
            {
                MessageBox.Show("Warning: For the 'smtp' recipients validation level, you must ensure that your network can open a connection on port 25. Please run this command in cmd window to test your network:\n\ntelnet gmail-smtp-in.l.google.com 25\n\nThe server must respond with the code '220' if everything works, otherwise please contact your ISP and ask for port 25 to be opened.", "Warning - Ultimate Mailer V3", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            buttonOptionsApply.Enabled = OptionsChanged;
        }

        private void NumericUpDownRecipientsValidationTimeout_ValueChanged(object sender, EventArgs e)
        {
            buttonOptionsApply.Enabled = OptionsChanged;
        }

        private void CheckBoxCheckupRecipientsLevelRole_CheckedChanged(object sender, EventArgs e)
        {
            buttonOptionsApply.Enabled = OptionsChanged;
        }

        private void CheckBoxCheckupRecipientsLevelDisposable_CheckedChanged(object sender, EventArgs e)
        {
            buttonOptionsApply.Enabled = OptionsChanged;
        }

        private void ButtonOptionsDefault_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Are you sur you want to reset default options ?", "Ultimate Mailer", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                checkBoxProjectBackup.Checked = true;

                checkBoxServersCertificate.Checked = true;
                checkBoxServersAuthentication.Checked = true;

                numericUpDownServersReportsValidity.Value = 1;
                numericUpDownServersReportsInterval.Value = 10;

                numericUpDownProxiesValidationTimeout.Value = 60000;

                comboBoxCheckupRecipientsLevel.SelectedIndex = 2;
                numericUpDownRecipientsValidationTimeout.Value = 60000;
                checkBoxCheckupRecipientsLevelRole.Checked = true;
                checkBoxCheckupRecipientsLevelDisposable.Checked = true;
            }
        }

        private void ButtonOptionsClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ButtonOptionsOk_Click(object sender, EventArgs e)
        {
            if (buttonOptionsApply.Enabled)
            {
                buttonOptionsApply.PerformClick();
            }

            Close();
        }

        private void ButtonOptionsApply_Click(object sender, EventArgs e)
        {
            Settings.Default.SettingProjectBackup = checkBoxProjectBackup.Checked;
            Settings.Default.SettingServersCertificate = checkBoxServersCertificate.Checked;
            Settings.Default.SettingServersAuthentication = checkBoxServersAuthentication.Checked;
            Settings.Default.SettingServersReportsValidity = (int) numericUpDownServersReportsValidity.Value;
            Settings.Default.SettingServersReportsInterval = (int) numericUpDownServersReportsInterval.Value;
            Settings.Default.SettingProxiesValidationTimeout = (int) numericUpDownProxiesValidationTimeout.Value;
            Settings.Default.SettingRecipientsValidationLevel = comboBoxCheckupRecipientsLevel.SelectedIndex;
            Settings.Default.SettingRecipientsValidationTimeout = (int) numericUpDownRecipientsValidationTimeout.Value;
            Settings.Default.SettingRecipientsValidationLevelRole = checkBoxCheckupRecipientsLevelRole.Checked;
            Settings.Default.SettingRecipientsValidationLevelDisposable = checkBoxCheckupRecipientsLevelDisposable.Checked;

            Settings.Default.Save();

            buttonOptionsApply.Enabled = false;
        }

        private bool OptionsChanged
        {
            get
            {
                return checkBoxProjectBackup.Checked != Settings.Default.SettingProjectBackup ||
                    checkBoxServersCertificate.Checked != Settings.Default.SettingServersCertificate ||
                    checkBoxServersAuthentication.Checked != Settings.Default.SettingServersAuthentication ||
                    numericUpDownServersReportsValidity.Value != Settings.Default.SettingServersReportsValidity ||
                    numericUpDownServersReportsInterval.Value != Settings.Default.SettingServersReportsInterval ||
                    numericUpDownProxiesValidationTimeout.Value != Settings.Default.SettingProxiesValidationTimeout ||
                    comboBoxCheckupRecipientsLevel.SelectedIndex != Settings.Default.SettingRecipientsValidationLevel ||
                    numericUpDownRecipientsValidationTimeout.Value != Settings.Default.SettingRecipientsValidationTimeout ||
                    checkBoxCheckupRecipientsLevelRole.Checked != Settings.Default.SettingRecipientsValidationLevelRole ||
                    checkBoxCheckupRecipientsLevelDisposable.Checked != Settings.Default.SettingRecipientsValidationLevelDisposable;
            }
        }
    }
}
