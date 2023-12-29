using System;

namespace ultimate.mailer.Models
{
    public class Log
    {
        public enum VERBOSE { SUCCESS, INFO, WARNING, ERROR, DEBUG }

        public DateTime Date { get; }

        public VERBOSE Verbose { get; }

        public string Message { get; }

        public Log(VERBOSE verbose, string message)
        {
            Date = DateTime.Now;
            Verbose = verbose;
            Message = message;
        }

        public override string ToString()
        {
            return Date.ToString() + " " + Verbose.ToString().PadRight(7) + " " + Message;
        }
    }
}