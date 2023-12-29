using ultimate.mailer.Controllers;
using ultimate.mailer.Models;

namespace ultimate.mailer.Views
{
    public interface IMailingView
    {
        void SetController(MailingController controller);

        string Title { set; }

        void WriteLog(Log log);
    }
}
