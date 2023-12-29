using System.Xml;

namespace ultimate.mailer.Models
{
    interface ISerializer
    {
        void Serialize(XmlDocument document, XmlNode parentNode);

        void Deserialize(XmlDocument document, XmlNode selfNode);
    }
}
