using MimeKit;
using System;
using System.Security.Authentication;
using ultimate.mailer.Controllers;
using ultimate.mailer.Models;

namespace ultimate.mailer.Views
{
    public interface IMainView
    {
        void SetController(MainController controller);

        string Title { set; }

        //Servers Panel

        int ServersCount { set; }

        string ServerConnectionGeneralHost { set; }

        int ServerConnectionGeneralPort { set; }

        SslProtocols ServerConnectionGeneralProtocol { set; }

        bool ServerConnectionAuthentication { set; }

        string ServerConnectionAuthenticationUsername { set; }

        string ServerConnectionAuthenticationPassword { set; }

        int ServerSettingsGeneralTimeout { set; }

        int ServerSettingsGeneralDelay { set; }

        string ServerSettingsSenderName { set; }

        string ServerSettingsSenderEmail { set; }

        bool ServerSettingsLimitSession { set; }

        int ServerSettingsLimitSessionValue { set; }

        bool ServerSettingsLimitHourly { set; }

        int ServerSettingsLimitHourlyValue { set; }

        bool ServerSettingsLimitDaily { set; }

        int ServerSettingsLimitDailyValue { set; }

        bool ServerSettingsLimitMonthly { set; }

        int ServerSettingsLimitMonthlyValue { set; }

        int ServerSettingsAdvancedSessionDelay { set; }

        bool ServerSettingsAdvancedPing { set; }

        int ServerSettingsAdvancedPingValue { set; }

        bool ServerSettingsAdvancedProxy { set; }

        bool ServerReportsAvailable { set; }

        DateTime ServerReportsAvailability { set; }

        int ServerReportsPerformedCount { set; }

        int ServerReportsSentCount { set; }

        int ServerReportsErrorCount { set; }

        int ServerReportsHourlyCount { set; }

        int ServerReportsDailyCount { set; }

        int ServerReportsMonthlyCount { set; }

        int ServerReportsCapabilities { set; }

        //Proxies panel

        int ProxiesCount { set; }

        Proxy.TYPE ProxyGeneralType { set; }

        string ProxyGeneralHost { set; }

        int ProxyGeneralPort { set; }

        bool ProxyAuthentication { set; }

        string ProxyAuthenticationUsername { set; }

        string ProxyAuthenticationPassword { set; }

        bool ProxyTest { set; }

        //Recipients panel

        int RecipientsCount { set; }

        int RecipientsFieldsCount { set; }

        //Subjects panel

        int SubjectsCount { set; }

        //Header panel

        string HeaderGeneralIdentifier { set; }

        bool HeaderGeneralDate { set; }

        DateTime HeaderGeneralDateValue { set; }

        bool HeaderFrom { set; }

        string HeaderFromName { set; }

        string HeaderFromEmail { set; }

        bool HeaderReplyTo { set; }

        string HeaderReplyToName { set; }

        string HeaderReplyToEmail { set; }

        bool HeaderListUnsubscribe { set; }

        string HeaderListUnsubscribeEmail { set; }

        string HeaderListUnsubscribeUrl { set; }

        string HeaderAdvancedReturnPath { set; }

        MessagePriority HeaderAdvancedPriority { set; }

        MessageImportance HeaderAdvancedImportance { set; }

        //Message Panel

        bool MessageBodyHtml { set; }

        string MessageBody { set; }

        int MessageAttachmentsCount { set; }

        //Preview Panel

        string PreviewControlRecipient { set; }

        string PreviewMailboxHeaderIdentifier { set; }

        string PreviewMailboxHeaderSubject { set; }

        string PreviewMailboxHeaderSender { set; }

        string PreviewMailboxHeaderFrom { set; }

        string PreviewMailboxHeaderTo { set; }

        string PreviewMailboxBodyText { set; }

        string PreviewMailboxBodyHtml { set; }

        DateTime PreviewMailboxHeaderDate { set; }

        double PreviewMailboxMessageSize { set; }

        int PreviewMailboxAttachmentsCount { set; }

        string PreviewOriginal { set; }

        //Checkup Panel

        string CheckupMessageControlServer { set; }

        double CheckupMessageScore { set; }

        PostmarkResponse.RESULT CheckupMessageScoreResult { set; }

        int CheckupMessageRulesCount { set; }

        string CheckupMessageReport { set; }

        int CheckupInspectionCount { set; }

        //Tasks Panel

        int ExtrasThreadsAmount { set; }

        Project.SELECTION ExtrasServersSelection { set; }

        int ExtrasServersRotation { set; }

        int ExtrasServersDelay { set; }

        Project.SELECTION ExtrasProxiesSelection { set; }

        int ExtrasProxiesRotation { set; }

        int ExtrasProxiesDelay { set; }

        int ExtrasBomberAmount { set; }
    }
}
