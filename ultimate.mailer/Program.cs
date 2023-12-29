using System;
using System.Reflection;
using System.Windows.Forms;
using ultimate.mailer.Controllers;

[assembly: Obfuscation(Feature = "Apply to type : apply to member when method or constructor: virtualization", Exclude = false)]
[assembly: Obfuscation(Feature = "string encryption", Exclude = false)]
[assembly: Obfuscation(Feature = "encrypt resources", Exclude = false)]
[assembly: Obfuscation(Feature = "code control flow obfuscation", Exclude = false)]

namespace ultimate.mailer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var formSplash = new FormSplash();
            new SplashController(formSplash);

            Application.Run(new SplashApplicationContext(formSplash));
        }

        private class SplashApplicationContext : ApplicationContext
        {
            public SplashApplicationContext(FormSplash formSplash) : base(formSplash)
            {}

            protected override void OnMainFormClosed(object sender, EventArgs e)
            {
                if (sender is FormSplash formSplash && formSplash.DialogResult == DialogResult.Yes)
                {
                    var formMain = new FormMain();

                    using (new MainController(formMain))
                    {
                        MainForm = formMain;
                        MainForm.Show();
                    }
                }
                else
                {
                    base.OnMainFormClosed(sender, e);
                }
            }
        }
    }
}