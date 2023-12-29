using System;
using System.Windows.Forms;

using ultimate.mailer.Controllers;
using ultimate.mailer.Views;

namespace ultimate.mailer
{
    public partial class FormSplash : Form, ISplashView
    {
        private SplashController m_controller;

        public string LoadInformation { set => labelLoadInformation.Text = value; }

        public string SoftwareCopyright { set => labelCopyright.Text = value; }

        public string SoftwareVersion { set => labelVersion.Text = value; }

        public FormSplash()
        {
            DialogResult = DialogResult.No;
            InitializeComponent();
        }

        public void SetController(SplashController controller)
        {
            m_controller = controller;
        }

        private async void FormSplash_Shown(object sender, EventArgs e)
        {
            if (!await m_controller.AssertNetworkConnectionAsync())
            {
                Close();
                return;
            }

            if (!await m_controller.AssertSoftwareVersionAsync())
            {
                Close();
                return;
            }

            DialogResult = DialogResult.Yes;

            Close();
        }
    }
}
