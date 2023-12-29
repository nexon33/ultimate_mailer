using ultimate.mailer.Controllers;

namespace ultimate.mailer.Views
{
    public interface ISplashView
    {
        void SetController(SplashController controller);

        string LoadInformation { set; }

        string SoftwareCopyright { set; }

        string SoftwareVersion { set; }
    }
}
