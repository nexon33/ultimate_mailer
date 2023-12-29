using Newtonsoft.Json;

using System.Collections.Generic;

namespace ultimate.mailer.Models
{
    public class PostmarkResponse
    {
        public class Rule
        {
            public double Score { get; }

            public string Description { get; }

            public Rule([JsonProperty("score")] double score, [JsonProperty("description")] string description)
            {
                Score = score;
                Description = description;
            }
        }

        public enum RESULT { NONE, EXCELENT, GOOD, SPAM }

        private readonly List<Rule> m_rules;

        public bool Success { get; }

        public double Score { get; }

        public RESULT Result { get; }

        public IReadOnlyCollection<Rule> Rules { get => m_rules.AsReadOnly(); }

        public string Report { get; }

        public string Message { get; }

        [JsonConstructor]
        public PostmarkResponse([JsonProperty("success")] bool success, [JsonProperty("score")] double score, [JsonProperty("rules")] List<Rule> rules, [JsonProperty("report")] string report, [JsonProperty("message")] string message)
        {
            Success = success;

            Score = score;
            Result = Score <= 0 ? RESULT.EXCELENT : (Score < 5 ? RESULT.GOOD : RESULT.SPAM);

            m_rules = rules;
            Report = report;
            Message = message;
        }
    }
}
