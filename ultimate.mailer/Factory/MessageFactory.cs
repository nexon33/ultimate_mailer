using MimeKit;

namespace ultimate.mailer.Factory
{
    public abstract class MessageFactory
    {
        public abstract MimeMessage GetMimeMessage();
    }
}