using MailKit;

using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Xml;

namespace ultimate.mailer.Models
{
    public class Server : ISerializer
    {
        public enum LOCK { NONE, SESSION, HOURLY, DAILY, MONTHLY }

        public bool IsTested { get; set; }

        public bool? IsValidated { get; set; }

        public int Index { get { return Project.Instance.Servers.IndexOf(this); } }

        public string Name { get { return (!string.IsNullOrEmpty(ConnectionGeneralHost) ? ConnectionGeneralHost : "unknow host") + ":" + ConnectionGeneralPort; } }

        public Guid Identifier { get; private set; }

        public LOCK Lock { get; set; }

        public Guid SessionIdentifier { get; set; }

        public DateTime LastDisconnection { get; set; }

        public string ConnectionGeneralHost { get; set; }

        public int ConnectionGeneralPort { get; set; }

        public SslProtocols ConnectionGeneralProtocol { get; set; }

        public bool ConnectionAuthentication { get; set; }

        public string ConnectionAuthenticationUsername { get; set; }

        public string ConnectionAuthenticationPassword { get; set; }

        public int SettingsGeneralTimeout { get; set; }

        public int SettingsGeneralDelay { get; set; }

        public string SettingsSenderName { get; set; }

        public string SettingsSenderEmail { get; set; }

        public bool SettingsLimitSession { get; set; }

        public int SettingsLimitSessionValue { get; set; }

        public bool SettingsLimitHourly { get; set; }

        public int SettingsLimitHourlyValue { get; set; }

        public bool SettingsLimitDaily { get; set; }

        public int SettingsLimitDailyValue { get; set; }

        public bool SettingsLimitMonthly { get; set; }

        public int SettingsLimitMonthlyValue { get; set; }

        public int SettingsAdvancedSessionDelay { get; set; }

        public bool SettingsAdvancedPing { get; set; }

        public int SettingsAdvancedPingValue { get; set; }

        public bool SettingsAdvancedProxy { get; set; }

        public IList<Report> Reports { get; set; }

        public Server()
        {
            IsTested = false;
            IsValidated = null;

            Identifier = Guid.NewGuid();

            Lock = LOCK.NONE;

            SessionIdentifier = Guid.NewGuid();
            LastDisconnection = DateTime.MinValue;

            ConnectionGeneralHost = string.Empty;
            ConnectionGeneralPort = 25;
            ConnectionGeneralProtocol = SslProtocols.Tls12;

            ConnectionAuthentication = true;
            ConnectionAuthenticationUsername = string.Empty;
            ConnectionAuthenticationPassword = string.Empty;

            SettingsGeneralTimeout = 60000;
            SettingsGeneralDelay = 0;

            SettingsSenderName = string.Empty;
            SettingsSenderEmail = string.Empty;

            SettingsLimitSession = false;
            SettingsLimitSessionValue = 100;
            SettingsLimitHourly = false;
            SettingsLimitHourlyValue = 100;
            SettingsLimitDaily = false;
            SettingsLimitDailyValue = 500;
            SettingsLimitMonthly = false;
            SettingsLimitMonthlyValue = 2000;

            SettingsAdvancedSessionDelay = 130;
            SettingsAdvancedPing = true;
            SettingsAdvancedPingValue = 300;
            SettingsAdvancedProxy = true;

            Reports = new List<Report>();
        }

        public void SmtpClient_Connected(object sender, ConnectedEventArgs eventArgs)
        {
            if (DateTime.Now >= LastDisconnection.AddSeconds(SettingsAdvancedSessionDelay))
            {
                SessionIdentifier = Guid.NewGuid();
            }
        }

        public void SmtpClient_Disconnected(object sender, DisconnectedEventArgs eventArgs)
        {
            LastDisconnection = DateTime.Now;
        }

        public override string ToString()
        {
            return "[#" + (Index + 1).ToString() + " " + Name + "]";
        }

        public void Serialize(XmlDocument document, XmlNode parentNode)
        {
            XmlNode node = document.CreateElement("server");

            XmlAttribute attributeLock = document.CreateAttribute("lock");
            XmlAttribute attributeSession = document.CreateAttribute("session");
            XmlAttribute attributeDisconnection = document.CreateAttribute("disconnection");

            XmlNode nodeConnection = document.CreateElement("connection");

            XmlNode nodeConnectionGeneral = document.CreateElement("general");

            XmlAttribute attributeConnectionGeneralHost = document.CreateAttribute("host");
            XmlAttribute attributeConnectionGeneralPort = document.CreateAttribute("port");
            XmlAttribute attributeConnectionGeneralProtocol = document.CreateAttribute("protocol");

            XmlNode nodeConnectionAuthentication = document.CreateElement("authentication");

            XmlAttribute attributeConnectionAuthenticationEnable = document.CreateAttribute("enable");
            XmlAttribute attributeConnectionAuthenticationUsername = document.CreateAttribute("username");
            XmlAttribute attributeConnectionAuthenticationPassword = document.CreateAttribute("password");

            XmlNode nodeSettings = document.CreateElement("settings");

            XmlNode nodeSettingsGeneral = document.CreateElement("general");

            XmlAttribute attributeSettingsGeneralTimeout = document.CreateAttribute("timeout");
            XmlAttribute attributeSettingsGeneralDelay = document.CreateAttribute("delay");

            XmlNode nodeSettingsSender = document.CreateElement("sender");

            XmlAttribute attributeSettingsSenderName = document.CreateAttribute("name");
            XmlAttribute attributeSettingsSenderEmail = document.CreateAttribute("email");

            XmlNode nodeSettingsLimit = document.CreateElement("limit");

            XmlNode nodeSettingsLimitSession = document.CreateElement("session");

            XmlAttribute attributeSettingsLimitSessionEnable = document.CreateAttribute("enable");
            XmlAttribute attributeSettingsLimitSessionValue = document.CreateAttribute("value");

            XmlNode nodeSettingsLimitHourly = document.CreateElement("hourly");

            XmlAttribute attributeSettingsLimitHourlyEnable = document.CreateAttribute("enable");
            XmlAttribute attributeSettingsLimitHourlyValue = document.CreateAttribute("value");

            XmlNode nodeSettingsLimitDaily = document.CreateElement("daily");

            XmlAttribute attributeSettingsLimitDailyEnable = document.CreateAttribute("enable");
            XmlAttribute attributeSettingsLimitDailyValue = document.CreateAttribute("value");

            XmlNode nodeSettingsLimitMonthly = document.CreateElement("monthly");

            XmlAttribute attributeSettingsLimitMonthlyEnable = document.CreateAttribute("enable");
            XmlAttribute attributeSettingsLimitMonthlyValue = document.CreateAttribute("value");

            XmlNode nodeSettingsAdvanced = document.CreateElement("advanced");

            XmlAttribute attributeSettingsAdvancedSessionDelay = document.CreateAttribute("delay");
            XmlAttribute attributeSettingsAdvancedProxy = document.CreateAttribute("proxy");

            XmlNode nodeSettingsAdvancedPing = document.CreateElement("ping");

            XmlAttribute attributeSettingsAdvancedPingEnable = document.CreateAttribute("enable");
            XmlAttribute attributeSettingsAdvancedPingValue = document.CreateAttribute("value");

            XmlNode nodeReports = document.CreateElement("reports");

            node.Attributes.Append(attributeLock);
            node.Attributes.Append(attributeSession);
            node.Attributes.Append(attributeDisconnection);

            nodeConnectionGeneral.Attributes.Append(attributeConnectionGeneralHost);
            nodeConnectionGeneral.Attributes.Append(attributeConnectionGeneralPort);
            nodeConnectionGeneral.Attributes.Append(attributeConnectionGeneralProtocol);

            nodeConnectionAuthentication.Attributes.Append(attributeConnectionAuthenticationEnable);
            nodeConnectionAuthentication.Attributes.Append(attributeConnectionAuthenticationUsername);
            nodeConnectionAuthentication.Attributes.Append(attributeConnectionAuthenticationPassword);

            nodeSettingsGeneral.Attributes.Append(attributeSettingsGeneralTimeout);
            nodeSettingsGeneral.Attributes.Append(attributeSettingsGeneralDelay);

            nodeSettingsSender.Attributes.Append(attributeSettingsSenderName);
            nodeSettingsSender.Attributes.Append(attributeSettingsSenderEmail);

            nodeSettingsLimitSession.Attributes.Append(attributeSettingsLimitSessionEnable);
            nodeSettingsLimitSession.Attributes.Append(attributeSettingsLimitSessionValue);

            nodeSettingsLimitHourly.Attributes.Append(attributeSettingsLimitHourlyEnable);
            nodeSettingsLimitHourly.Attributes.Append(attributeSettingsLimitHourlyValue);

            nodeSettingsLimitDaily.Attributes.Append(attributeSettingsLimitDailyEnable);
            nodeSettingsLimitDaily.Attributes.Append(attributeSettingsLimitDailyValue);

            nodeSettingsLimitMonthly.Attributes.Append(attributeSettingsLimitMonthlyEnable);
            nodeSettingsLimitMonthly.Attributes.Append(attributeSettingsLimitMonthlyValue);

            nodeSettingsAdvanced.Attributes.Append(attributeSettingsAdvancedSessionDelay);
            nodeSettingsAdvanced.Attributes.Append(attributeSettingsAdvancedProxy);

            nodeSettingsAdvancedPing.Attributes.Append(attributeSettingsAdvancedPingEnable);
            nodeSettingsAdvancedPing.Attributes.Append(attributeSettingsAdvancedPingValue);

            nodeConnection.AppendChild(nodeConnectionGeneral);
            nodeConnection.AppendChild(nodeConnectionAuthentication);

            nodeSettingsLimit.AppendChild(nodeSettingsLimitSession);
            nodeSettingsLimit.AppendChild(nodeSettingsLimitHourly);
            nodeSettingsLimit.AppendChild(nodeSettingsLimitDaily);
            nodeSettingsLimit.AppendChild(nodeSettingsLimitMonthly);

            nodeSettingsAdvanced.AppendChild(nodeSettingsAdvancedPing);

            nodeSettings.AppendChild(nodeSettingsGeneral);
            nodeSettings.AppendChild(nodeSettingsSender);
            nodeSettings.AppendChild(nodeSettingsLimit);
            nodeSettings.AppendChild(nodeSettingsAdvanced);

            node.AppendChild(nodeConnection);
            node.AppendChild(nodeSettings);
            node.AppendChild(nodeReports);

            parentNode.AppendChild(node);

            //Value

            attributeLock.Value = ((int)Lock).ToString();
            attributeSession.Value = SessionIdentifier.ToString();
            attributeDisconnection.Value = LastDisconnection.ToString();

            attributeConnectionGeneralHost.Value = ConnectionGeneralHost;
            attributeConnectionGeneralPort.Value = ConnectionGeneralPort.ToString();
            attributeConnectionGeneralProtocol.Value = ((int)ConnectionGeneralProtocol).ToString();

            attributeConnectionAuthenticationEnable.Value = ConnectionAuthentication.ToString();
            attributeConnectionAuthenticationUsername.Value = ConnectionAuthenticationUsername;
            attributeConnectionAuthenticationPassword.Value = ConnectionAuthenticationPassword;

            attributeSettingsGeneralTimeout.Value = SettingsGeneralTimeout.ToString();
            attributeSettingsGeneralDelay.Value = SettingsGeneralDelay.ToString();

            attributeSettingsSenderName.Value = SettingsSenderName;
            attributeSettingsSenderEmail.Value = SettingsSenderEmail;

            attributeSettingsLimitSessionEnable.Value = SettingsLimitSession.ToString();
            attributeSettingsLimitSessionValue.Value = SettingsLimitSessionValue.ToString();
            attributeSettingsLimitHourlyEnable.Value = SettingsLimitHourly.ToString();
            attributeSettingsLimitHourlyValue.Value = SettingsLimitHourlyValue.ToString();
            attributeSettingsLimitDailyEnable.Value = SettingsLimitDaily.ToString();
            attributeSettingsLimitDailyValue.Value = SettingsLimitDailyValue.ToString();
            attributeSettingsLimitMonthlyEnable.Value = SettingsLimitMonthly.ToString();
            attributeSettingsLimitMonthlyValue.Value = SettingsLimitMonthlyValue.ToString();
            attributeSettingsAdvancedSessionDelay.Value = SettingsAdvancedSessionDelay.ToString();
            attributeSettingsAdvancedProxy.Value = SettingsAdvancedProxy.ToString();

            attributeSettingsAdvancedPingEnable.Value = SettingsAdvancedPing.ToString();
            attributeSettingsAdvancedPingValue.Value = SettingsAdvancedPingValue.ToString();

            foreach (Report report in Reports)
            {
                report.Serialize(document, nodeReports);
            }
        }

        public void Deserialize(XmlDocument document, XmlNode selfNode)
        {
            //Structure

            XmlNode nodeConnectionGeneral = selfNode.SelectSingleNode("connection/general");
            XmlNode nodeConnectionAuthentication = selfNode.SelectSingleNode("connection/authentication");

            XmlNode nodeSettingsGeneral = selfNode.SelectSingleNode("settings/general");
            XmlNode nodeSettingsSender = selfNode.SelectSingleNode("settings/sender");

            XmlNode nodeSettingsLimitSession = selfNode.SelectSingleNode("settings/limit/session");
            XmlNode nodeSettingsLimitHourly = selfNode.SelectSingleNode("settings/limit/hourly");
            XmlNode nodeSettingsLimitDaily = selfNode.SelectSingleNode("settings/limit/daily");
            XmlNode nodeSettingsLimitMonthly = selfNode.SelectSingleNode("settings/limit/monthly");

            XmlNode nodeSettingsAvanced = selfNode.SelectSingleNode("settings/advanced");
            XmlNode nodeSettingsAvancedPing = selfNode.SelectSingleNode("settings/advanced/ping");

            XmlNodeList nodeReports = selfNode.SelectNodes("reports/report");

            //Value

            Lock = (LOCK)(int.TryParse(selfNode.Attributes["lock"].Value, out int @lock) ? @lock : 0);
            SessionIdentifier = Guid.TryParse(selfNode.Attributes["session"].Value, out Guid guid) ? guid : Guid.NewGuid();
            LastDisconnection = DateTime.TryParse(selfNode.Attributes["disconnection"].Value, out DateTime date) ? date : DateTime.Now;

            ConnectionGeneralHost = nodeConnectionGeneral.Attributes["host"].Value;
            ConnectionGeneralPort = int.TryParse(nodeConnectionGeneral.Attributes["port"].Value, out int port) ? port : 25;
            ConnectionGeneralProtocol = (SslProtocols)(int.TryParse(nodeConnectionGeneral.Attributes["protocol"].Value, out int protocol) ? protocol : 3072);

            ConnectionAuthentication = !bool.TryParse(nodeConnectionAuthentication.Attributes["enable"].Value, out bool authentication) || authentication;
            ConnectionAuthenticationUsername = nodeConnectionAuthentication.Attributes["username"].Value;
            ConnectionAuthenticationPassword = nodeConnectionAuthentication.Attributes["password"].Value;

            SettingsGeneralTimeout = int.TryParse(nodeSettingsGeneral.Attributes["timeout"].Value, out int timeout) ? timeout : 60000;
            SettingsGeneralDelay = int.TryParse(nodeSettingsGeneral.Attributes["delay"].Value, out int delay) ? delay : 0;

            SettingsSenderName = nodeSettingsSender.Attributes["name"].Value;
            SettingsSenderEmail = nodeSettingsSender.Attributes["email"].Value;

            SettingsLimitSession = bool.TryParse(nodeSettingsLimitSession.Attributes["enable"].Value, out bool sessionLimit) && sessionLimit;
            SettingsLimitSessionValue = int.TryParse(nodeSettingsLimitSession.Attributes["value"].Value, out int sessionLimitValue) ? sessionLimitValue : 0;
            SettingsLimitHourly = bool.TryParse(nodeSettingsLimitHourly.Attributes["enable"].Value, out bool hourlyLimit) && hourlyLimit;
            SettingsLimitHourlyValue = int.TryParse(nodeSettingsLimitHourly.Attributes["value"].Value, out int hourlyLimitValue) ? hourlyLimitValue : 0;
            SettingsLimitDaily = bool.TryParse(nodeSettingsLimitDaily.Attributes["enable"].Value, out bool dailyLimit) && dailyLimit;
            SettingsLimitDailyValue = int.TryParse(nodeSettingsLimitDaily.Attributes["value"].Value, out int dailyLimitValue) ? dailyLimitValue : 0;
            SettingsLimitMonthly = bool.TryParse(nodeSettingsLimitMonthly.Attributes["enable"].Value, out bool monthlyLimit) && monthlyLimit;
            SettingsLimitMonthlyValue = int.TryParse(nodeSettingsLimitMonthly.Attributes["value"].Value, out int monthlyLimitValue) ? monthlyLimitValue : 0;

            SettingsAdvancedSessionDelay = int.TryParse(nodeSettingsAvanced.Attributes["delay"].Value, out int sessionDelay) ? sessionDelay : 130;
            SettingsAdvancedPing = !bool.TryParse(nodeSettingsAvancedPing.Attributes["enable"].Value, out bool ping) || ping;
            SettingsAdvancedPingValue = int.TryParse(nodeSettingsAvancedPing.Attributes["value"].Value, out int pingValue) ? pingValue : 300;
            SettingsAdvancedProxy = !bool.TryParse(nodeSettingsAvanced.Attributes["proxy"].Value, out bool proxy) || proxy;

            foreach (XmlNode nodeReport in nodeReports)
            {
                var report = new Report();
                report.Deserialize(document, nodeReport);

                Reports.Add(report);
            }
        }
    }
}
