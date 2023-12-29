using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace ultimate.mailer.Models
{
    public class Recipient : ISerializer
    {
        public bool IsTested { get; set; }

        public bool? IsValidated { get; set; }

        public int Index { get { return Project.Instance.Recipients.IndexOf(this); } }

        public string Email { get; private set; }

        public IList<string> Fields { get; set; }

        public Recipient()
        {
            IsTested = false;
            IsValidated = null;

            Email = string.Empty;
            Fields = new List<string>();
        }

        public Recipient(string email, int fieldCount)
        {
            IsTested = false;
            IsValidated = null;

            Email = email;
            Fields = Enumerable.Repeat(string.Empty, fieldCount).ToList();
        }

        public override string ToString()
        {
            return "[#" + (Index + 1) + " " + Email + "]";
        }

        public void Serialize(XmlDocument document, XmlNode parentNode)
        {
            XmlNode node = document.CreateElement("recipient");
            XmlAttribute attributeEmail = document.CreateAttribute("email");

            XmlNode nodeFields = document.CreateElement("fields");

            node.Attributes.Append(attributeEmail);

            node.AppendChild(nodeFields);

            parentNode.AppendChild(node);

            attributeEmail.Value = Email;

            foreach (string field in Fields)
            {
                XmlNode nodeField = document.CreateElement("field");
                nodeField.InnerText = field;
                nodeFields.AppendChild(nodeField);
            }
        }

        public void Deserialize(XmlDocument document, XmlNode selfNode)
        {
            XmlNodeList nodeFields = selfNode.SelectNodes("fields/field");

            Email = selfNode.Attributes["email"].Value;

            for (int i = 0; i < nodeFields.Count; ++i)
            {
                Fields.Add(nodeFields[i].InnerText);
            }
        }
    }
}
