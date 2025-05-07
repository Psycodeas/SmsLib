using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SmsLib
{
    internal class Utils
    {
        public static bool ValidateToken(string responseContent)
        {
            Regex titleRegex = new Regex(@"<title>Login</title>", RegexOptions.IgnoreCase);
            Match match = titleRegex.Match(responseContent);

            return !match.Success;
        }

        public static string NormalaizeNumber(string number)
        {
            if (number.StartsWith("0") && number.Length == 11)
            {
                return "+98" + number.Substring(1);
            }
            else if (number.StartsWith("+98") && number.Length == 13)
            {
                return number;
            }
            else
            {
                throw new ArgumentException($"The number '{number}' is not valid.");
            }
        }

        public static string Time()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        }

        public static string HexToString(string hex)
        {
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return new string(System.Text.Encoding.UTF8.GetString(bytes).Where(c => c != '\0').ToArray()); ;
        }
    }
}
