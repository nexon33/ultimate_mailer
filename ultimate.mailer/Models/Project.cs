using MimeKit;

using System;
using System.Collections.Generic;
using System.Xml;

namespace ultimate.mailer.Models
{
    public class Project : IDisposable, ISerializer
    {
        public enum SELECTION { ASYNCHRONICALLY, SYNCHRONICALLY, RANDOMLY }

        public enum IDENTIFIER { SERVER, SENDER, FROM, TO }

        private bool IsDisposed { get; set; }

        private static Project instance = null;
        private static readonly object padlock = new object();

        public static Project Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Project();
                    }

                    return instance;
                }
            }
        }

        public string Path { get; set; }

        public List<Server> Servers { get; set; }

        public List<Proxy> Proxies { get; set; }

        public List<Recipient> Recipients { get; set; }

        public int RecipientFieldCount { get; set; }

        public List<string> Subjects { get; set; }

        public string HeaderGeneralIdentifier { get; set; }

        public bool HeaderGeneralDate { get; set; }

        public DateTime HeaderGeneralDateValue { get; set; }

        public bool HeaderFrom { get; set; }

        public string HeaderFromName { get; set; }

        public string HeaderFromEmail { get; set; }

        public bool HeaderReplyTo { get; set; }

        public string HeaderReplyToName { get; set; }

        public string HeaderReplyToEmail { get; set; }

        public bool HeaderListUnsubscribe { get; set; }

        public string HeaderListUnsubscribeEmail { get; set; }

        public string HeaderListUnsubscribeUrl { get; set; }

        public string HeaderAdvancedReturnPath { get; set; }

        public MessagePriority HeaderAdvancedPriority { get; set; }

        public MessageImportance HeaderAdvancedImportance { get; set; }

        public bool MessageBodyHtml { get; set; }

        public string MessageBody { get; set; }

        public List<string> MessageAttachments { get; set; }

        public int ExtrasThreadsAmount { get; set; }

        public SELECTION ExtrasServersSelection { get; set; }

        public int ExtrasServersRotation { get; set; }

        public int ExtrasServersDelay { get; set; }

        public SELECTION ExtrasProxiesSelection { get; set; }

        public int ExtrasProxiesRotation { get; set; }

        public int ExtrasProxiesDelay { get; set; }

        public int ExtrasBomberAmount { get; set; }

        private Project()
        {
            Path = "untilted.umproj";

            Servers = new List<Server>();

            Proxies = new List<Proxy>();

            Recipients = new List<Recipient>();
            RecipientFieldCount = 0;

            Subjects = new List<string>();

            HeaderGeneralIdentifier = "{SERVER_HOST}";
            HeaderGeneralDate = false;
            HeaderGeneralDateValue = DateTime.Now;

            HeaderFrom = false;
            HeaderFromName = string.Empty;
            HeaderFromEmail = string.Empty;

            HeaderReplyTo = false;
            HeaderReplyToName = string.Empty;
            HeaderReplyToEmail = string.Empty;

            HeaderAdvancedReturnPath = string.Empty;
            HeaderAdvancedPriority = MessagePriority.Normal;
            HeaderAdvancedImportance = MessageImportance.Normal;

            HeaderListUnsubscribe = false;
            HeaderListUnsubscribeEmail = string.Empty;
            HeaderListUnsubscribeUrl = string.Empty;

            MessageBodyHtml = false;
            MessageBody = string.Empty;

            MessageAttachments = new List<string>();

            ExtrasThreadsAmount = 1;

            ExtrasServersSelection = SELECTION.ASYNCHRONICALLY;
            ExtrasServersRotation = 1000;
            ExtrasServersDelay = 100;

            ExtrasProxiesSelection = SELECTION.ASYNCHRONICALLY;
            ExtrasProxiesRotation = 1000;
            ExtrasProxiesDelay = 100;

            ExtrasBomberAmount = 1;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Project()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    Servers.Clear();
                    Servers.TrimExcess();

                    Proxies.Clear();
                    Proxies.TrimExcess();

                    Recipients.Clear();
                    Recipients.TrimExcess();

                    Subjects.Clear();
                    Subjects.TrimExcess();

                    MessageAttachments.Clear();
                    MessageAttachments.TrimExcess();
                }

                IsDisposed = true;
            }
        }

        public void Serialize(XmlDocument document, XmlNode parentNode)
        {
            XmlAttribute attributeVersion = document.CreateAttribute("version");

            XmlNode nodeServers = document.CreateElement("servers");
            XmlNode nodeProxies = document.CreateElement("proxies");
            XmlNode nodeRecipients = document.CreateElement("recipients");
            XmlAttribute attributeRecipientsFields = document.CreateAttribute("fields");

            XmlNode nodeSubjects = document.CreateElement("subjects");

            XmlNode nodeHeader = document.CreateElement("header");

            XmlNode nodeHeaderGeneral = document.CreateElement("general");
            XmlAttribute attributeHeaderGeneralIdentifier = document.CreateAttribute("identifier");

            XmlNode nodeHeaderGeneralDate = document.CreateElement("date");
            XmlAttribute attributeHeaderGeneralDateEnable = document.CreateAttribute("enable");
            XmlAttribute attributeGeneralDateValue = document.CreateAttribute("value");

            XmlNode nodeHeaderFrom = document.CreateElement("from");

            XmlAttribute attributeFromEnable = document.CreateAttribute("enable");
            XmlAttribute attributeFromName = document.CreateAttribute("name");
            XmlAttribute attributeFromEmail = document.CreateAttribute("email");

            XmlNode nodeReplyTo = document.CreateElement("reply-to");

            XmlAttribute attributeHeaderReplyToEnable = document.CreateAttribute("enable");
            XmlAttribute attributeHeaderReplyToName = document.CreateAttribute("name");
            XmlAttribute attributeHeaderReplyToEmail = document.CreateAttribute("email");

            XmlNode nodeHeaderUnsubscribe = document.CreateElement("unsubscribe");

            XmlAttribute attributeHeaderUnsubscribeEnable = document.CreateAttribute("enable");
            XmlAttribute attributeHeaderUnsubscribeEmail = document.CreateAttribute("email");
            XmlAttribute attributeHeaderUnsubscribeUrl = document.CreateAttribute("url");

            XmlNode nodeHeaderAdvanced = document.CreateElement("advanced");

            XmlAttribute attributeHeaderAdvancedReturnPath = document.CreateAttribute("return-path");
            XmlAttribute attributeHeaderAdvancedPriority = document.CreateAttribute("priority");
            XmlAttribute attributeHeaderAdvancedImportance = document.CreateAttribute("importance");

            XmlNode nodeMessage = document.CreateElement("message");

            XmlNode nodeMessageBody = document.CreateElement("body");
            XmlAttribute attributeMessageBodyHtml = document.CreateAttribute("html");

            XmlNode nodeMessageAttachments = document.CreateElement("attachments");

            XmlNode nodeExtras = document.CreateElement("extras");

            XmlNode nodeExtrasThreads = document.CreateElement("threads");
            XmlAttribute nodeExtrasThreadsAmount = document.CreateAttribute("amount");

            XmlNode nodeExtrasServers = document.CreateElement("servers");

            XmlAttribute attributeExtrasServersSelection = document.CreateAttribute("selection");
            XmlAttribute attributeExtrasServersRotation = document.CreateAttribute("rotation");
            XmlAttribute attributeExtrasServersDelay = document.CreateAttribute("delay");

            XmlNode nodeExtrasProxies = document.CreateElement("proxies");

            XmlAttribute attributeExtrasProxiesSelection = document.CreateAttribute("selection");
            XmlAttribute attributeExtrasProxiesRotation = document.CreateAttribute("rotation");
            XmlAttribute attributeExtrasProxiesDelay = document.CreateAttribute("delay");

            XmlNode nodeExtrasBomber = document.CreateElement("bomber");

            XmlAttribute attributeExtrasBomberAmount = document.CreateAttribute("amount");

            parentNode.Attributes.Append(attributeVersion);

            nodeRecipients.Attributes.Append(attributeRecipientsFields);

            nodeHeaderGeneral.Attributes.Append(attributeHeaderGeneralIdentifier);

            nodeHeaderGeneralDate.Attributes.Append(attributeHeaderGeneralDateEnable);
            nodeHeaderGeneralDate.Attributes.Append(attributeGeneralDateValue);

            nodeHeaderFrom.Attributes.Append(attributeFromEnable);
            nodeHeaderFrom.Attributes.Append(attributeFromName);
            nodeHeaderFrom.Attributes.Append(attributeFromEmail);

            nodeReplyTo.Attributes.Append(attributeHeaderReplyToEnable);
            nodeReplyTo.Attributes.Append(attributeHeaderReplyToName);
            nodeReplyTo.Attributes.Append(attributeHeaderReplyToEmail);

            nodeHeaderUnsubscribe.Attributes.Append(attributeHeaderUnsubscribeEnable);
            nodeHeaderUnsubscribe.Attributes.Append(attributeHeaderUnsubscribeEmail);
            nodeHeaderUnsubscribe.Attributes.Append(attributeHeaderUnsubscribeUrl);

            nodeHeaderAdvanced.Attributes.Append(attributeHeaderAdvancedReturnPath);
            nodeHeaderAdvanced.Attributes.Append(attributeHeaderAdvancedPriority);
            nodeHeaderAdvanced.Attributes.Append(attributeHeaderAdvancedImportance);

            nodeMessageBody.Attributes.Append(attributeMessageBodyHtml);

            nodeExtrasThreads.Attributes.Append(nodeExtrasThreadsAmount);

            nodeExtrasServers.Attributes.Append(attributeExtrasServersSelection);
            nodeExtrasServers.Attributes.Append(attributeExtrasServersRotation);
            nodeExtrasServers.Attributes.Append(attributeExtrasServersDelay);

            nodeExtrasProxies.Attributes.Append(attributeExtrasProxiesSelection);
            nodeExtrasProxies.Attributes.Append(attributeExtrasProxiesRotation);
            nodeExtrasProxies.Attributes.Append(attributeExtrasProxiesDelay);

            nodeExtrasBomber.Attributes.Append(attributeExtrasBomberAmount);

            nodeHeaderGeneral.AppendChild(nodeHeaderGeneralDate);

            nodeHeader.AppendChild(nodeHeaderGeneral);
            nodeHeader.AppendChild(nodeHeaderFrom);
            nodeHeader.AppendChild(nodeReplyTo);
            nodeHeader.AppendChild(nodeHeaderUnsubscribe);
            nodeHeader.AppendChild(nodeHeaderAdvanced);

            nodeMessage.AppendChild(nodeMessageBody);
            nodeMessage.AppendChild(nodeMessageAttachments);

            nodeExtras.AppendChild(nodeExtrasThreads);
            nodeExtras.AppendChild(nodeExtrasServers);
            nodeExtras.AppendChild(nodeExtrasProxies);
            nodeExtras.AppendChild(nodeExtrasBomber);

            parentNode.AppendChild(nodeServers);
            parentNode.AppendChild(nodeProxies);
            parentNode.AppendChild(nodeRecipients);
            parentNode.AppendChild(nodeSubjects);
            parentNode.AppendChild(nodeHeader);
            parentNode.AppendChild(nodeMessage);
            parentNode.AppendChild(nodeExtras);

            //Value

            attributeVersion.Value = Constantes.BACKUP_VERSION.ToString();

            attributeHeaderGeneralIdentifier.Value = HeaderGeneralIdentifier;

            foreach (Server server in Servers)
            {
                server.Serialize(document, nodeServers);
            }

            foreach (Proxy proxy in Proxies)
            {
                proxy.Serialize(document, nodeProxies);
            }

            attributeRecipientsFields.Value = RecipientFieldCount.ToString();

            foreach (Recipient recipient in Recipients)
            {
                recipient.Serialize(document, nodeRecipients);
            }

            foreach (string subject in Subjects)
            {
                XmlNode nodeSubject = document.CreateElement("subject");
                nodeSubject.InnerText = subject;
                nodeSubjects.AppendChild(nodeSubject);
            }

            attributeHeaderGeneralIdentifier.Value = HeaderGeneralIdentifier;
            attributeHeaderGeneralDateEnable.Value = HeaderGeneralDate.ToString();
            attributeGeneralDateValue.Value = HeaderGeneralDateValue.ToString();

            attributeFromEnable.Value = HeaderFrom.ToString();
            attributeFromName.Value = HeaderFromName;
            attributeFromEmail.Value = HeaderFromEmail;

            attributeHeaderReplyToEnable.Value = HeaderReplyTo.ToString();
            attributeHeaderReplyToName.Value = HeaderReplyToName;
            attributeHeaderReplyToEmail.Value = HeaderReplyToEmail;

            attributeHeaderUnsubscribeEnable.Value = HeaderListUnsubscribe.ToString();
            attributeHeaderUnsubscribeEmail.Value = HeaderListUnsubscribeEmail;
            attributeHeaderUnsubscribeUrl.Value = HeaderListUnsubscribeUrl;

            attributeHeaderAdvancedReturnPath.Value = HeaderAdvancedReturnPath;
            attributeHeaderAdvancedPriority.Value = ((int)HeaderAdvancedPriority).ToString();
            attributeHeaderAdvancedImportance.Value = ((int)HeaderAdvancedImportance).ToString();

            attributeMessageBodyHtml.Value = MessageBodyHtml.ToString();
            nodeMessageBody.InnerText = document.CreateCDataSection(MessageBody).Value;

            foreach (string attachment in MessageAttachments)
            {
                XmlNode messageAttachment = document.CreateElement("attachment");
                messageAttachment.InnerText = attachment;
                nodeMessageAttachments.AppendChild(messageAttachment);
            }

            nodeExtrasThreadsAmount.Value = ExtrasThreadsAmount.ToString();

            attributeExtrasServersSelection.Value = Convert.ToInt32(ExtrasServersSelection).ToString();
            attributeExtrasServersRotation.Value = ExtrasServersRotation.ToString();
            attributeExtrasServersDelay.Value = ExtrasServersDelay.ToString();

            attributeExtrasProxiesSelection.Value = Convert.ToInt32(ExtrasProxiesSelection).ToString();
            attributeExtrasProxiesRotation.Value = ExtrasProxiesRotation.ToString();
            attributeExtrasProxiesDelay.Value = ExtrasProxiesDelay.ToString();

            attributeExtrasBomberAmount.Value = ExtrasBomberAmount.ToString();
        }

        public void Deserialize(XmlDocument document, XmlNode selfNode)
        {
            XmlNodeList nodeServers = document.DocumentElement.SelectNodes("servers/server");
            XmlNodeList nodeProxies = document.DocumentElement.SelectNodes("proxies/proxy");
            XmlNodeList nodeRecipientsList = document.DocumentElement.SelectNodes("recipients/recipient");
            XmlNode nodeRecipients = document.DocumentElement.SelectSingleNode("recipients");
            XmlNodeList nodeSubjects = document.DocumentElement.SelectNodes("subjects/subject");

            XmlNode nodeHeaderGeneral = document.DocumentElement.SelectSingleNode("header/general");
            XmlNode nodeHeaderGeneralDate = nodeHeaderGeneral.SelectSingleNode("date");
            XmlNode nodeHeaderFrom = document.DocumentElement.SelectSingleNode("header/from");
            XmlNode nodeHeaderReplyTo = document.DocumentElement.SelectSingleNode("header/reply-to");
            XmlNode nodeHeaderUnsubscribe = document.DocumentElement.SelectSingleNode("header/unsubscribe");
            XmlNode nodeHeaderAdvanced = document.DocumentElement.SelectSingleNode("header/advanced");

            XmlNode nodeMessageBody = document.DocumentElement.SelectSingleNode("message/body");
            XmlNodeList messageAttachmentNodes = document.DocumentElement.SelectNodes("message/attachments/attachment");

            XmlNode nodeExtrasThreads = document.DocumentElement.SelectSingleNode("extras/threads");
            XmlNode nodeExtrasServers = document.DocumentElement.SelectSingleNode("extras/servers");
            XmlNode nodeExtrasProxies = document.DocumentElement.SelectSingleNode("extras/proxies");
            XmlNode nodeExtrasBomber = document.DocumentElement.SelectSingleNode("extras/bomber");

            int backupVersion = int.TryParse(selfNode.Attributes["version"].Value, out int version) ? version : Constantes.BACKUP_VERSION;

            Servers.Clear();
            Servers.TrimExcess();

            foreach (XmlNode nodeServer in nodeServers)
            {
                var server = new Server();
                server.Deserialize(document, nodeServer);
                Servers.Add(server);
            }

            Proxies.Clear();
            Proxies.TrimExcess();

            foreach (XmlNode nodeProxy in nodeProxies)
            {
                var proxy = new Proxy();
                proxy.Deserialize(document, nodeProxy);
                Proxies.Add(proxy);
            }

            Recipients.Clear();
            Recipients.TrimExcess();

            RecipientFieldCount = Convert.ToInt32(nodeRecipients.Attributes["fields"].Value);

            foreach (XmlNode nodeRecipient in nodeRecipientsList)
            {
                var recipient = new Recipient();
                recipient.Deserialize(document, nodeRecipient);
                Recipients.Add(recipient);
            }

            Subjects.Clear();
            Subjects.TrimExcess();

            foreach (XmlNode nodeSubject in nodeSubjects)
            {
                Subjects.Add(nodeSubject.InnerText);
            }

            HeaderGeneralIdentifier = nodeHeaderGeneral.Attributes["identifier"].Value;
            HeaderGeneralDate = bool.TryParse(nodeHeaderGeneralDate.Attributes["enable"].Value, out bool date) && date;
            HeaderGeneralDateValue = DateTime.TryParse(nodeHeaderGeneralDate.Attributes["value"].Value, out DateTime dateTime) ? dateTime : DateTime.Now;

            HeaderFrom = bool.TryParse(nodeHeaderFrom.Attributes["enable"].Value, out bool from) && from;
            HeaderFromName = nodeHeaderFrom.Attributes["name"].Value;
            HeaderFromEmail = nodeHeaderFrom.Attributes["email"].Value;

            HeaderReplyTo = bool.TryParse(nodeHeaderReplyTo.Attributes["enable"].Value, out bool replyTo) && replyTo;
            HeaderReplyToName = nodeHeaderReplyTo.Attributes["name"].Value;
            HeaderReplyToEmail = nodeHeaderReplyTo.Attributes["email"].Value;

            HeaderListUnsubscribe = bool.TryParse(nodeHeaderUnsubscribe.Attributes["enable"].Value, out bool unsubscribe) && unsubscribe;
            HeaderListUnsubscribeEmail = nodeHeaderUnsubscribe.Attributes["email"].Value;
            HeaderListUnsubscribeUrl = nodeHeaderUnsubscribe.Attributes["url"].Value;

            HeaderAdvancedReturnPath = nodeHeaderAdvanced.Attributes["return-path"].Value;
            HeaderAdvancedPriority = (MessagePriority)(int.TryParse(nodeHeaderAdvanced.Attributes["priority"].Value, out int priority) ? priority : 1);
            HeaderAdvancedImportance = (MessageImportance)(int.TryParse(nodeHeaderAdvanced.Attributes["importance"].Value, out int importance) ? importance : 1);

            MessageBodyHtml = bool.TryParse(nodeMessageBody.Attributes["html"].Value, out bool html) && html;
            MessageBody = nodeMessageBody.InnerText;

            foreach (XmlNode messageAttachmentNode in messageAttachmentNodes)
            {
                MessageAttachments.Add(messageAttachmentNode.InnerText);
            }

            ExtrasThreadsAmount = int.TryParse(nodeExtrasThreads.Attributes["amount"].Value, out int threads) ? threads : 1;

            ExtrasServersSelection = (SELECTION)(int.TryParse(nodeExtrasServers.Attributes["selection"].Value, out int serversSelection) ? serversSelection : 0);
            ExtrasServersRotation = int.TryParse(nodeExtrasServers.Attributes["rotation"].Value, out int serverRotation) ? serverRotation : 1000;
            ExtrasServersDelay = int.TryParse(nodeExtrasServers.Attributes["delay"].Value, out int serverDelay) ? serverDelay : 100;

            ExtrasProxiesSelection = (SELECTION)(int.TryParse(nodeExtrasProxies.Attributes["selection"].Value, out int proxySelection) ? proxySelection : 0);
            ExtrasProxiesRotation = int.TryParse(nodeExtrasProxies.Attributes["rotation"].Value, out int proxyRotation) ? proxyRotation : 1000;
            ExtrasProxiesDelay = int.TryParse(nodeExtrasProxies.Attributes["delay"].Value, out int proxyDelay) ? proxyDelay : 100;

            ExtrasBomberAmount = int.TryParse(nodeExtrasBomber.Attributes["amount"].Value, out int bomber) ? bomber : 1;
        }
    }
}
