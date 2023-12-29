using System.Xml;

namespace ultimate.mailer.Models
{
    public class Proxy : ISerializer
    {
        public enum TYPE { HTTP, SOCKS4, SOCKS4a, SOCKS5 }

        public bool IsTested { get; set; }

        public bool? IsValidated { get; set; }

        public int Index { get { return Project.Instance.Proxies.IndexOf(this); } }

        public string Name { get { return (!string.IsNullOrEmpty(GeneralHost) ? GeneralHost : "unknow host") + ":" + GeneralPort; } }

        public TYPE GeneralType { set; get; }

        public string GeneralHost { set; get; }

        public int GeneralPort { set; get; }

        public bool Authentication { set; get; }

        public string AuthenticationUsername { set; get; }

        public string AuthenticationPassword { set; get; }

        public Proxy()
        {
            IsTested = false;
            IsValidated = null;

            GeneralType = TYPE.HTTP;
            GeneralHost = string.Empty;
            GeneralPort = 8080;

            Authentication = false;
            AuthenticationUsername = string.Empty;
            AuthenticationPassword = string.Empty;
        }

        public override string ToString()
        {
            return "[#" + (Index + 1).ToString() + " " + Name + "]";
        }

        public void Serialize(XmlDocument document, XmlNode parentNode)
        {
            //Structure

            XmlNode node = document.CreateElement("proxy");

            XmlNode nodeGeneral = document.CreateElement("general");

            XmlAttribute attributeGeneralType = document.CreateAttribute("type");
            XmlAttribute attributeGeneralHost = document.CreateAttribute("host");
            XmlAttribute attributeGeneralPort = document.CreateAttribute("port");

            XmlNode nodeAuthentication = document.CreateElement("authentication");

            XmlAttribute attributeAuthenticationEnable = document.CreateAttribute("enable");
            XmlAttribute attributeAuthenticationUsername = document.CreateAttribute("username");
            XmlAttribute attributeAuthenticationPassword = document.CreateAttribute("password");

            nodeGeneral.Attributes.Append(attributeGeneralType);
            nodeGeneral.Attributes.Append(attributeGeneralHost);
            nodeGeneral.Attributes.Append(attributeGeneralPort);

            nodeAuthentication.Attributes.Append(attributeAuthenticationEnable);
            nodeAuthentication.Attributes.Append(attributeAuthenticationUsername);
            nodeAuthentication.Attributes.Append(attributeAuthenticationPassword);

            node.AppendChild(nodeGeneral);
            node.AppendChild(nodeAuthentication);

            parentNode.AppendChild(node);

            //Value

            attributeGeneralType.Value = ((int)GeneralType).ToString();
            attributeGeneralHost.Value = GeneralHost;
            attributeGeneralPort.Value = GeneralPort.ToString();

            attributeAuthenticationEnable.Value = Authentication.ToString();
            attributeAuthenticationUsername.Value = AuthenticationUsername;
            attributeAuthenticationPassword.Value = AuthenticationPassword;
        }

        public void Deserialize(XmlDocument document, XmlNode selfNode)
        {
            //Structure

            XmlNode nodeGeneral = selfNode.SelectSingleNode("general");
            XmlNode nodeAuthentication = selfNode.SelectSingleNode("authentication");

            //Value

            GeneralType = (TYPE)(int.TryParse(nodeGeneral.Attributes["type"].Value, out int type) ? type : 0);
            GeneralHost = nodeGeneral.Attributes["host"].Value;
            GeneralPort = int.TryParse(nodeGeneral.Attributes["port"].Value, out int port) ? port : 8080;

            Authentication = bool.TryParse(nodeAuthentication.Attributes["enable"].Value, out bool authentication) && authentication;
            AuthenticationUsername = nodeAuthentication.Attributes["username"].Value;
            AuthenticationPassword = nodeAuthentication.Attributes["password"].Value;
        }
    }
}
