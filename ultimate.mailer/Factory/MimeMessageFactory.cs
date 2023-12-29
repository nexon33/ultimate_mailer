using MimeKit;

using ultimate.mailer.Models;

namespace ultimate.mailer.Factory
{
    class MimeMessageFactory : MessageFactory
    {
        private readonly Server m_server;
        private readonly Recipient m_recipient;
        private readonly bool m_attachments;

        public MimeMessageFactory(Server server = null, Recipient recipient = null, bool attachments = true)
        {
            m_server = server;
            m_recipient = recipient;
            m_attachments = attachments;
        }

        public override MimeMessage GetMimeMessage()
        {
            return new Message(m_server, m_recipient, m_attachments);
        }
    }
}
