using System;
using System.Net;
using System.Text;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

using MimeKit;
using MimeKit.Utils;


namespace ultimate.mailer
{
    class Utils
    {
        private static readonly Regex REGEX_HTML_TAG = new Regex(Constantes.REGEX_PATTERN_HTML_TAG, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex REGEX_EMPTY_LINE = new Regex(Constantes.REGEX_PATTERN_EMPTY_LINE, RegexOptions.Multiline | RegexOptions.Compiled);

        public static async Task<bool> IsNetworkAvailable()
        {
            try
            {
                using (var client = new WebClient())
                {
                    await client.OpenReadTaskAsync(new Uri("http://clients3.google.com/generate_204"));
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var defaultAddress = new MailAddress(email);
                var mailkitAddress = MailboxAddress.Parse(email);

                return mailkitAddress.Address == defaultAddress.Address && defaultAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidIdentifier(string identifier)
        {
            string messageId = MimeUtils.GenerateMessageId(identifier);
            return IsValidEmail(messageId);
        }

        public static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public static string HtmlToText(string htmlString)
        {
            htmlString = WebUtility.HtmlDecode(htmlString);

            htmlString = REGEX_HTML_TAG.Replace(htmlString, string.Empty);
            htmlString = REGEX_EMPTY_LINE.Replace(htmlString, "\r\n");

            htmlString = htmlString.TrimStart(' ', '\r', '\n');
            htmlString = htmlString.TrimEnd(' ', '\r', '\n');

            IEnumerable<string> lines = htmlString.Split('\n').Select(line => line.Trim());
            htmlString = string.Join("\n", lines);

            return htmlString;
        }

        public static string HashSHA256(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                var stringBuilder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    stringBuilder.Append(bytes[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }

        public static bool TryBase64Encode(string input, out string output)
        {
            output = string.Empty;

            try
            {
                var textBytes = Encoding.UTF8.GetBytes(input);
                output = Convert.ToBase64String(textBytes);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}