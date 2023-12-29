using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ultimate.mailer.Controllers;
using ultimate.mailer.Models;
using ultimate.mailer.Views;

namespace ultimate.mailer
{
    public partial class FormMailing : Form, IMailingView
    {
        private MailingController m_controller;

        private const int TIMER_UPDATE_INTERVAL = 1000;

        private bool m_cancellationRequested;
        private readonly int m_projectCapabilities;
        private readonly Timer m_timerUpdate;

        public string Title { set => Text = value + " - Ultimate Mailer V3"; }

        public FormMailing()
        {
            m_cancellationRequested = false;
            m_projectCapabilities = Project.Instance.Recipients.Count * Project.Instance.ExtrasBomberAmount;

            m_timerUpdate = new Timer
            {
                Interval = TIMER_UPDATE_INTERVAL
            };

            m_timerUpdate.Tick += new EventHandler(TimerEventProcessor);

            InitializeComponent();
        }

        private void FormMailing_Load(object sender, EventArgs e)
        {
            comboBoxTrackersExport.SelectedIndex = 0;

            circularProgressBarTrackersPerformed.Maximum = m_projectCapabilities;
            circularProgressBarTrackersSent.Maximum = m_projectCapabilities;
            circularProgressBarTrackersError.Maximum = m_projectCapabilities;

            circularProgressBarTrackersPerformed.Value = 0;
            circularProgressBarTrackersSent.Value = 0;
            circularProgressBarTrackersError.Value = 0;

            labelTrackersErrorValue.Text = "0 / " + m_projectCapabilities;
            labelTrackersPerformedValue.Text = "0 / " + m_projectCapabilities;
            labelTrackersSentValue.Text = "0 / " + m_projectCapabilities;
        }

        private void FormMailing_Shown(object sender, EventArgs e)
        {
            m_controller.StartExecution();
            m_timerUpdate.Start();
            buttonMaillingStop.Enabled = true;
        }

        private void FormMailing_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_cancellationRequested)
            {
                e.Cancel = true;

                Task.Run(() =>
                {
                    m_controller.StopExecution();
                    m_timerUpdate.Stop();

                    m_cancellationRequested = true;

                    BeginInvoke((MethodInvoker)delegate
                    {
                       Close();
                    });
                });
            }
        }

        public void SetController(MailingController controller)
        {
            m_controller = controller;
        }

        private void ButtonMaillingStart_Click(object sender, EventArgs e)
        {
            buttonMaillingStart.Enabled = false;
            m_controller.StartExecution();
            m_timerUpdate.Start();
            buttonMaillingStop.Enabled = true;
        }

        private void ButtonMaillingStop_Click(object sender, EventArgs e)
        {
            buttonMaillingStop.Enabled = false;
            m_controller.StopExecution();
            m_timerUpdate.Stop();
            buttonMaillingStart.Enabled = true;
        }

        private async void ButtonMailingTrackersExport_Click(object sender, EventArgs e)
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Title = "Export trackers - Ultimate Mailer V3";
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|Comma-separated values files (*.csv)|*.csv";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    comboBoxTrackersExport.Visible = false;
                    buttonMailingTrackersExport.Visible = false;
                    pictureBoxTrackersLoading.Visible = true;

                    int exportMode = comboBoxTrackersExport.SelectedIndex;
                    await m_controller.TrackersExportAsync(saveFileDialog.FileName, exportMode);

                    pictureBoxTrackersLoading.Visible = false;
                    buttonMailingTrackersExport.Visible = true;
                    comboBoxTrackersExport.Visible = true;
                }
            }
        }

        private void RichTextBoxLogs_TextChanged(object sender, EventArgs e)
        {
            richTextBoxLogs.AppendText(Environment.NewLine);
        }

        public void WriteLog(Log log)
        {
            Action appendText = () =>
            {
                richTextBoxLogs.AppendText(log.ToString());
            };

            if (richTextBoxLogs.InvokeRequired)
            {
                richTextBoxLogs.BeginInvoke(appendText);
            }
            else
            {
                appendText();
            }
        }

        private void TimerEventProcessor(object sender, EventArgs eventArgs)
        {
            tableLayoutPanelTrackers.SuspendLayout();

            int performedCount = m_controller.Trackers.Where(tracker => tracker.Performed).Count();
            int performedPercent = performedCount * 100 / m_projectCapabilities;

            circularProgressBarTrackersPerformed.Value = performedCount;
            circularProgressBarTrackersPerformed.Text = performedPercent + "%";
            labelTrackersPerformedValue.Text = performedCount + " / " + m_projectCapabilities;

            int sentCount = m_controller.Trackers.Where(tracker => tracker.Sent).Count();
            int sentPercent = sentCount * 100 / m_projectCapabilities;

            circularProgressBarTrackersSent.Value = sentCount;
            circularProgressBarTrackersSent.Text = sentPercent + "%";

            labelTrackersSentValue.Text = sentCount + " / " + m_projectCapabilities;

            int errorCount = m_controller.Trackers.Where(tracker => tracker.Error).Count();
            int errorPercent = errorCount * 100 / m_projectCapabilities;

            circularProgressBarTrackersError.Value = errorCount;
            circularProgressBarTrackersError.Text = errorPercent + "%";

            labelTrackersErrorValue.Text = errorCount + " / " + m_projectCapabilities;

            tableLayoutPanelTrackers.ResumeLayout();
        }
    }
}
