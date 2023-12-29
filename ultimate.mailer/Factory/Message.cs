using MimeKit;
using MimeKit.Utils;

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;

using ultimate.mailer.Models;

namespace ultimate.mailer.Factory
{
    public class Message : MimeMessage
    {
        private readonly Random m_random = new Random();

        private readonly Server m_server;
        private readonly Recipient m_recipient;

        public Message(Server server, Recipient recipient, bool attachments)
        {
            m_server = server;
            m_recipient = recipient;

            if (m_server == null && Project.Instance.Servers.Count > 0)
            {
                m_server = Project.Instance.Servers[m_random.Next(Project.Instance.Servers.Count)];
            }

            if (m_recipient != null && Utils.IsValidEmail(m_recipient.Email))
            {
                To.Add(MailboxAddress.Parse(m_recipient.Email));
            }

            if (m_server != null && Utils.IsValidEmail(m_server.SettingsSenderEmail))
            {
                string senderName = GetCustomizedSenderName(m_server.SettingsSenderName);
                Sender = new MailboxAddress(senderName, m_server.SettingsSenderEmail);
            }

            

            if (Project.Instance.HeaderFrom)
            {
                string fromEmail = GetCustomizedFromEmail(Project.Instance.HeaderFromEmail);

                if (Utils.IsValidEmail(fromEmail))
                {
                    string fromName = GetCustomizedFromName(Project.Instance.HeaderFromName);
                    From.Add(new MailboxAddress(fromName, fromEmail));
                }
            }
            else if (m_server != null && Utils.IsValidEmail(m_server.SettingsSenderEmail))
            {
                string senderName = GetCustomizedSenderName(m_server.SettingsSenderName);
                From.Add(new MailboxAddress(senderName, m_server.SettingsSenderEmail));
            }

            string messageIdentifier = GetCustomizedMessageIdentifier(Project.Instance.HeaderGeneralIdentifier);

            if (!string.IsNullOrWhiteSpace(messageIdentifier) && Utils.IsValidIdentifier(messageIdentifier))
            {
                MessageId = MimeUtils.GenerateMessageId(messageIdentifier);
            }
            else
            {
                MessageId = MimeUtils.GenerateMessageId();
            }

            if (Project.Instance.Subjects.Count != 0)
            {
                string subject = Project.Instance.Subjects[m_random.Next(Project.Instance.Subjects.Count)];
                Subject = GetCustomizedString(subject);
            }

            if (Project.Instance.HeaderGeneralDate)
            {
                Date = Project.Instance.HeaderGeneralDateValue;
            }

            if (Project.Instance.HeaderReplyTo)
            {
                string replyToEmail = GetCustomizedReplyToEmail(Project.Instance.HeaderReplyToEmail);

                if (Utils.IsValidEmail(replyToEmail))
                {
                    string replyToName = GetCustomizedReplyToName(Project.Instance.HeaderReplyToName);
                    ReplyTo.Add(new MailboxAddress(replyToName, replyToEmail));
                }
            }

            if (Project.Instance.HeaderListUnsubscribe)
            {
                var stringBuilder = new StringBuilder();

                string listUnsubscribeEmail = GetCustomizedListUnsubscribeEmail(Project.Instance.HeaderListUnsubscribeEmail);

                if (Utils.IsValidEmail(listUnsubscribeEmail))
                {
                    stringBuilder.Append("<mailto:" + listUnsubscribeEmail + "?subject=unsubscribe>");
                }

                string listUnsubscribeUrl = Project.Instance.HeaderListUnsubscribeUrl;

                if (Utils.IsValidUrl(listUnsubscribeUrl))
                {
                    if (stringBuilder.Length > 0) stringBuilder.Append(", ");
                    stringBuilder.Append("<" + listUnsubscribeUrl + ">");
                }

                Headers.Add("List-Unsubscribe", stringBuilder.ToString());
                stringBuilder.Clear();
            }

            if (!string.IsNullOrWhiteSpace(Project.Instance.HeaderAdvancedReturnPath))
            {
                string returnPathEmail = GetCustomizedReturnPathEmail(Project.Instance.HeaderAdvancedReturnPath);

                if (Utils.IsValidEmail(returnPathEmail))
                {
                    Headers.Add("Return-Path", returnPathEmail);
                }
            }

            Priority = Project.Instance.HeaderAdvancedPriority;
            Importance = Project.Instance.HeaderAdvancedImportance;

            var bodyBuilder = new BodyBuilder();

            if (!string.IsNullOrWhiteSpace(Project.Instance.MessageBody))
            {
                if (!Project.Instance.MessageBodyHtml)
                {
                    bodyBuilder.TextBody = GetCustomizedString(Project.Instance.MessageBody);
                }
                else
                {
                    string messageBodyHtml = GetCustomizedString(Project.Instance.MessageBody);
                    bodyBuilder.TextBody = Utils.HtmlToText(messageBodyHtml);
                    bodyBuilder.HtmlBody = messageBodyHtml;
                }
            }

            if (attachments)
            {
                foreach (string attachment in Project.Instance.MessageAttachments)
                {
                    if (File.Exists(attachment))
                    {
                        bodyBuilder.Attachments.Add(attachment);
                    }
                }
            }

            Body = bodyBuilder.ToMessageBody();
        }

        private string GetCustomizedString(string input)
        {
            var stringBuilder = new StringBuilder(input);

            if (Sender != null)
            {
                var mailAddress = new MailAddress(Sender.ToString());

                stringBuilder.Replace("{SENDER_NAME}", mailAddress.DisplayName);
                stringBuilder.Replace("{SENDER_EMAIL}", mailAddress.Address);
                stringBuilder.Replace("{SENDER_USER}", mailAddress.User);
                stringBuilder.Replace("{SENDER_HOST}", mailAddress.Host);
            }

            if (From.Count > 0)
            {
                var mailAddress = new MailAddress(From[0].ToString());

                stringBuilder.Replace("{FROM_NAME}", mailAddress.DisplayName);
                stringBuilder.Replace("{FROM_EMAIL}", mailAddress.Address);
                stringBuilder.Replace("{FROM_USER}", mailAddress.User);
                stringBuilder.Replace("{FROM_HOST}", mailAddress.Host);
            }

            if (To.Count > 0)
            {
                var mailAddress = new MailAddress(To[0].ToString());

                stringBuilder.Replace("{TO_EMAIL}", mailAddress.Address);

                if (Utils.TryBase64Encode(mailAddress.Address, out string toEmail))
                {
                    stringBuilder.Replace("{TO_EMAIL_BASE64}", toEmail);
                }

                stringBuilder.Replace("{TO_USER}", mailAddress.User);
                stringBuilder.Replace("{TO_HOST}", mailAddress.Host);

                for (int i = 0; i < m_recipient.Fields.Count; ++i)
                {
                    stringBuilder.Replace("{TO_FIELD_" + (i + 1) + "}", m_recipient.Fields[i]);
                }
            }

            int index = -1;

            while ((index = stringBuilder.ToString().IndexOf("{RAND_STR}")) != -1)
            {
                string randomStr = new string(Enumerable.Repeat(Constantes.CHARS, m_random.Next(10, 20)).Select(s => s[m_random.Next(s.Length)]).ToArray());
                stringBuilder.Replace("{RAND_STR}", randomStr, index, 10);
            }

            while ((index = stringBuilder.ToString().IndexOf("{RAND_INT}")) != -1)
            {
                int randomInt = m_random.Next(int.MinValue, int.MaxValue);
                stringBuilder.Replace("{RAND_INT}", randomInt.ToString(), index, 10);
            }

            while ((index = stringBuilder.ToString().IndexOf("{RAND_UINT}")) != -1)
            {
                int randomUint = m_random.Next(0, int.MaxValue);
                stringBuilder.Replace("{RAND_UINT}", randomUint.ToString(), index, 11);
            }

            while ((index = stringBuilder.ToString().IndexOf("{RAND_IPV4}")) != -1)
            {
                var data = new byte[4];
                m_random.NextBytes(data);

                IPAddress ipv4 = new IPAddress(data);
                stringBuilder.Replace("{RAND_IPV4}", ipv4.ToString(), index, 11);
            }

            while ((index = stringBuilder.ToString().IndexOf("{RAND_IPV6}")) != -1)
            {
                var data = new byte[16];
                m_random.NextBytes(data);

                IPAddress ipv6 = new IPAddress(data);
                stringBuilder.Replace("{RAND_IPV6}", ipv6.ToString(), index, 11);
            }

            stringBuilder.Replace("{GUID}", Guid.NewGuid().ToString());

            DateTime dateTime = DateTime.Now;

            stringBuilder.Replace("{DATE}", dateTime.ToShortDateString());
            stringBuilder.Replace("{TIME}", dateTime.ToShortTimeString());
            stringBuilder.Replace("{DATETIME}", dateTime.ToString());

            return Spinner.Spin(stringBuilder.ToString());
        }

        public string GetCustomizedMessageIdentifier(string input)
        {
            var stringBuilder = new StringBuilder(input);

            if (m_server != null)
            {
                stringBuilder.Replace("{SERVER_HOST}", m_server.ConnectionGeneralHost);
            }

            if (Sender != null)
            {
                stringBuilder.Replace("{SENDER_HOST}", new MailAddress(Sender.Address.ToString()).Host);
            }

            if (From.Count > 0)
            {
                stringBuilder.Replace("{FROM_HOST}", new MailAddress(From[0].ToString()).Host);
            }

            if (To.Count > 0)
            {
                stringBuilder.Replace("{TO_HOST}", new MailAddress(To[0].ToString()).Host);
            }

            return stringBuilder.ToString();
        }

        public string GetCustomizedSenderName(string input)
        {
            var stringBuilder = new StringBuilder(input);

            if (To.Count > 0)
            {
                var mailAddress = new MailAddress(To[0].ToString());

                stringBuilder.Replace("{TO_EMAIL}", mailAddress.Address);
                stringBuilder.Replace("{TO_USER}", mailAddress.User);
                stringBuilder.Replace("{TO_HOST}", mailAddress.Host);

                for (int i = 0; i < m_recipient.Fields.Count; ++i)
                {
                    stringBuilder.Replace("{TO_FIELD_" + (i + 1) + "}", m_recipient.Fields[i]);
                }
            }

            return stringBuilder.ToString();
        }

        public string GetCustomizedFromName(string input)
        {
            var stringBuilder = new StringBuilder(input);

            if (Sender != null)
            {
                stringBuilder.Replace("{SENDER_NAME}", Sender.Name);
            }

            if (To.Count > 0)
            {
                var mailAddress = new MailAddress(To[0].ToString());

                stringBuilder.Replace("{TO_EMAIL}", mailAddress.Address);
                stringBuilder.Replace("{TO_USER}", mailAddress.User);
                stringBuilder.Replace("{TO_HOST}", mailAddress.Host);

                for (int i = 0; i < m_recipient.Fields.Count; ++i)
                {
                    stringBuilder.Replace("{TO_FIELD_" + (i + 1) + "}", m_recipient.Fields[i]);
                }
            }

            return stringBuilder.ToString();
        }

        public string GetCustomizedFromEmail(string input)
        {
            var stringBuilder = new StringBuilder(input);

            if (Sender != null)
            {
                stringBuilder.Replace("{SENDER_EMAIL}", Sender.Address.ToString());
            }

            if (To.Count > 0)
            {
                stringBuilder.Replace("{TO_EMAIL}", new MailAddress(To[0].ToString()).Address);
            }

            return stringBuilder.ToString();
        }

        public string GetCustomizedReplyToName(string input)
        {
            var stringBuilder = new StringBuilder(input);

            if (Sender != null)
            {
                stringBuilder.Replace("{SENDER_NAME}", Sender.Name);
            }

            if (From.Count > 0)
            {
                var mailAddress = new MailAddress(From[0].ToString());
                stringBuilder.Replace("{FROM_NAME}", mailAddress.DisplayName);
            }

            return stringBuilder.ToString();
        }

        public string GetCustomizedReplyToEmail(string input)
        {
            var stringBuilder = new StringBuilder(input);

            if (Sender != null)
            {
                stringBuilder.Replace("{SENDER_EMAIL}", Sender.Address.ToString());
            }

            if (From.Count > 0)
            {
                var mailAddress = new MailAddress(From[0].ToString());
                stringBuilder.Replace("{FROM_EMAIL}", mailAddress.Address);
            }

            return stringBuilder.ToString();
        }

        public string GetCustomizedListUnsubscribeEmail(string input)
        {
            var stringBuilder = new StringBuilder(input);

            if (Sender != null)
            {
                stringBuilder.Replace("{SENDER_EMAIL}", Sender.Address.ToString());
            }

            return stringBuilder.ToString();
        }

        public string GetCustomizedReturnPathEmail(string input)
        {
            var stringBuilder = new StringBuilder(input);

            if (Sender != null)
            {
                stringBuilder.Replace("{SENDER_EMAIL}", Sender.Address.ToString());
            }

            return stringBuilder.ToString();
        }
    }
}
