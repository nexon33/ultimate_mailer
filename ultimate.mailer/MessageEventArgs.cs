using System;

using ultimate.mailer.Models;

namespace ultimate.mailer
{
    public class MessagePerformedEventArgs
    {
        public Tracker Tracker { get; }

        public string Identifier { get; }

        public Server Server { get; }

        public MessagePerformedEventArgs(Tracker tracker, string identifier, Server server)
        {
            Tracker = tracker;
            Identifier = identifier;
            Server = server;
        }
    }

    public class MessageErrorEventArgs
    {
        public Tracker Tracker { get; }

        public Exception Exception { get; }

        public MessageErrorEventArgs(Tracker tracker, Exception exception)
        {
            Tracker = tracker;
            Exception = exception;
        }
    }
}
