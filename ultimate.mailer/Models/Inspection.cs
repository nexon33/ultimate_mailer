namespace ultimate.mailer.Models
{
    public class Inspection
    {
        public enum VERBOSE { INFO, WARNING, DANGER }

        public VERBOSE Verbose { get; private set; }

        public string Message { get; private set; }

        public Inspection(VERBOSE verbose, string message)
        {
            Verbose = verbose;
            Message = message;
        }
    }
}
