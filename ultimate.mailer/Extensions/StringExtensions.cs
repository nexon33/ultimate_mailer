using System;
using System.Linq;
using System.Text;

namespace ultimate.mailer.Extensions
{
    static public class StringExtensions
    {
        public static string[] Words(this string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.Length == 0)
            {
                return new string[0];
            }

            char[] array = input.Where(c => !char.IsPunctuation(c) || char.IsWhiteSpace(c)).ToArray();
            input = new string(array);

            return input.Trim().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
