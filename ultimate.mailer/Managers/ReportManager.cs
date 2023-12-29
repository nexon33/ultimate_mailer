using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ultimate.mailer.Models
{
    public class ReportManager
    {
        public enum NOTIFICATION { PERFORMED, SENT, ERROR }

        private readonly Server m_server;

        public ReadOnlyCollection<Report> Reports { get { return new ReadOnlyCollection<Report>(m_server.Reports); } }

        public int PerformedCount { get { return m_server.Reports.Sum(report => report.Performed); } }

        public int SentCount { get { return m_server.Reports.Sum(report => report.Sent); } }

        public int ErrorCount { get { return m_server.Reports.Sum(report => report.Error); } }

        public int SessionCount { get { return m_server.Reports.Where(report => report.SessionIdentifier == m_server.SessionIdentifier).Sum(report => report.Sent); } }

        public int HourlyCount { get { return m_server.Reports.Where(report => report.End >= DateTime.Now.AddHours(-1)).Sum(report => report.Sent); } }

        public int DailyCount { get { return m_server.Reports.Where(report => report.End >= DateTime.Now.AddDays(-1)).Sum(report => report.Sent); } }

        public int MonthlyCount { get { return m_server.Reports.Where(report => report.End >= DateTime.Now.AddMonths(-1)).Sum(report => report.Sent); } }

        public ReportManager(Server server)
        {
            m_server = server;
        }

        private DateTime GetLastSentDate()
        {
            if (m_server.Reports.Count != 0)
            {
                return m_server.Reports.Last(report => report.Sent > 0).End;
            }

            return DateTime.Now;
        }

        public bool IsAvailable()
        {
            if (m_server.Lock == Server.LOCK.MONTHLY)
            {
                if (GetLastSentDate().AddMonths(1) <= DateTime.Now)
                {
                    m_server.Lock = Server.LOCK.NONE;
                }
            }
            else if (m_server.Lock == Server.LOCK.DAILY)
            {
                if (GetLastSentDate().AddDays(1) <= DateTime.Now)
                {
                    m_server.Lock = Server.LOCK.NONE;
                }
            }
            else if (m_server.Lock == Server.LOCK.HOURLY)
            {
                if (GetLastSentDate().AddHours(1) <= DateTime.Now)
                {
                    m_server.Lock = Server.LOCK.NONE;
                }
            }
            else if (m_server.Lock == Server.LOCK.SESSION)
            {
                if (GetLastSentDate().AddSeconds(m_server.SettingsAdvancedSessionDelay) <= DateTime.Now)
                {
                    m_server.Lock = Server.LOCK.NONE;
                }
            }

            return m_server.Lock == Server.LOCK.NONE;
        }

        public DateTime GetAvailability()
        {
            if (m_server.Lock == Server.LOCK.MONTHLY)
            {
                return GetLastSentDate().AddMonths(1);
            }
            else if (m_server.Lock == Server.LOCK.DAILY)
            {
                return GetLastSentDate().AddDays(1);
            }
            else if (m_server.Lock == Server.LOCK.HOURLY)
            {
                return GetLastSentDate().AddHours(1);
            }
            else if (m_server.Lock == Server.LOCK.SESSION)
            {
                return GetLastSentDate().AddSeconds(m_server.SettingsAdvancedSessionDelay);
            }

            return DateTime.MinValue;
        }

        public int GetCapabilities()
        {
            if (IsAvailable())
            {
                if (m_server.SettingsLimitHourly)
                {
                    return m_server.SettingsLimitHourlyValue - HourlyCount;
                }
                else if (m_server.SettingsLimitDaily)
                {
                    return m_server.SettingsLimitDailyValue - DailyCount;
                }
                else if (m_server.SettingsLimitMonthly)
                {
                    return m_server.SettingsLimitMonthlyValue - MonthlyCount;
                }
                else
                {
                    return -1;
                }
            }

            return 0;
        }

        public void Notify(NOTIFICATION notification)
        {
            if (m_server.Reports.Count == 0 || m_server.Reports.Last().End < DateTime.Now || !m_server.Reports.Last().SessionIdentifier.Equals(m_server.SessionIdentifier))
            {
                if (m_server.Reports.Count != 0 && m_server.Reports.Last().End > DateTime.Now)
                {
                    m_server.Reports.Last().Close();
                }

                m_server.Reports.Add(new Report(m_server.SessionIdentifier));
            }

            m_server.Reports.Last().Notify(notification);

            if (notification == NOTIFICATION.SENT)
            {
                if (m_server.SettingsLimitSession && m_server.SettingsLimitSessionValue - SessionCount == 0)
                {
                    m_server.Lock = Server.LOCK.SESSION;
                }
                else if (m_server.SettingsLimitHourly && m_server.SettingsLimitHourlyValue - HourlyCount == 0)
                {
                    m_server.Lock = Server.LOCK.HOURLY;
                }
                else if (m_server.SettingsLimitDaily && m_server.SettingsLimitDailyValue - DailyCount == 0)
                {
                    m_server.Lock = Server.LOCK.DAILY;
                }
                else if (m_server.SettingsLimitMonthly && m_server.SettingsLimitMonthlyValue - MonthlyCount == 0)
                {
                    m_server.Lock = Server.LOCK.MONTHLY;
                }

                if (m_server.Lock != Server.LOCK.NONE)
                {
                    m_server.Reports.Last().Close();
                }
            }
        }

        public void Update()
        {
            m_server.Reports = m_server.Reports.Where(report => report.Expiration > DateTime.Now).ToList();
        }
    }
}