using System;
using System.Xml;

namespace ultimate.mailer.Models
{
    public class Report : ISerializer
    {
        public Guid SessionIdentifier { get; private set; }

        public DateTime Begin { get; private set; }

        public DateTime End { get; private set; }

        public int Performed { get; private set; }

        public int Sent { get; private set; }

        public int Error { get; private set; }

        public DateTime Expiration { get { return End.AddMonths(Properties.Settings.Default.SettingServersReportsValidity); } }

        public Report() { }

        public Report(Guid sessionIdentifier)
        {
            SessionIdentifier = sessionIdentifier;

            Begin = DateTime.Now;
            End = Begin.AddMinutes(Properties.Settings.Default.SettingServersReportsInterval);

            Performed = 0;
            Sent = 0;
            Error = 0;
        }

        public void Notify(ReportManager.NOTIFICATION notify)
        {
            switch (notify)
            {
                case ReportManager.NOTIFICATION.PERFORMED:
                    ++Performed;
                    break;
                case ReportManager.NOTIFICATION.SENT:
                    ++Sent;
                    break;
                case ReportManager.NOTIFICATION.ERROR:
                    ++Error;
                    break;
            }
        }

        public void Close()
        {
            End = DateTime.Now;
        }

        public void Serialize(XmlDocument document, XmlNode parentNode)
        {
            //Structure

            XmlNode nodeReport = document.CreateElement("report");

            XmlAttribute attributeReportSession = document.CreateAttribute("session");
            XmlAttribute attributeReportBegin = document.CreateAttribute("begin");
            XmlAttribute attributeReportEnd = document.CreateAttribute("end");
            XmlAttribute attributeReportPerformed = document.CreateAttribute("performed");
            XmlAttribute attributeReportSent = document.CreateAttribute("sent");
            XmlAttribute attributeReportError = document.CreateAttribute("error");

            nodeReport.Attributes.Append(attributeReportSession);
            nodeReport.Attributes.Append(attributeReportBegin);
            nodeReport.Attributes.Append(attributeReportEnd);
            nodeReport.Attributes.Append(attributeReportPerformed);
            nodeReport.Attributes.Append(attributeReportSent);
            nodeReport.Attributes.Append(attributeReportError);

            parentNode.AppendChild(nodeReport);

            //Value

            attributeReportSession.Value = SessionIdentifier.ToString();
            attributeReportBegin.Value = Begin.ToString();
            attributeReportEnd.Value = End.ToString();
            attributeReportPerformed.Value = Performed.ToString();
            attributeReportSent.Value = Sent.ToString();
            attributeReportError.Value = Error.ToString();
        }

        public void Deserialize(XmlDocument document, XmlNode selfNode)
        {
            SessionIdentifier = Guid.TryParse(selfNode.Attributes["session"].Value, out Guid guid) ? guid : Guid.NewGuid();

            Begin = DateTime.TryParse(selfNode.Attributes["begin"].Value, out DateTime begin) ? begin : DateTime.MinValue;
            End = DateTime.TryParse(selfNode.Attributes["end"].Value, out DateTime end) ? end : DateTime.Now;

            Performed = int.TryParse(selfNode.Attributes["performed"].Value, out int performed) ? performed : 0;
            Sent = int.TryParse(selfNode.Attributes["sent"].Value, out int sent) ? sent : 0;
            Error = int.TryParse(selfNode.Attributes["error"].Value, out int error) ? error : 0;
        }
    }
}
