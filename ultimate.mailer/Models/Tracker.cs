namespace ultimate.mailer.Models
{
    public class Tracker
    {
        public Recipient Recipient { get; }

        public Server Server { get; set; }

        public string Identifier { get; set; }

        public bool Performed { get; set; }

        public bool Sent { get; set; }

        public bool Error { get; set; }

        public string Response { get; set; }

        public Tracker(Recipient recipient)
        {
            Recipient = recipient;
            Server = null;

            Identifier = string.Empty;
            Performed = false;
            Sent = false;
            Error = false;
            Response = string.Empty;
        }
    }
}
