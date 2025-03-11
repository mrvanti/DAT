
using System.Text.RegularExpressions;


namespace DAT
{
    internal static class Utility
    {
        private static string timePattern = @"^(\d?):(\d{2})$"; // Match "m:ss" or ":ss"


        internal static (bool, int) CheckAndConvertTime(string input)
        {
            Match match = Regex.Match(input, timePattern);
            var isOkTimeFormat = match.Success;
            var time = 120;
            if (isOkTimeFormat)
            {
                int minutes = string.IsNullOrEmpty(match.Groups[1].Value) ? 0 : int.Parse(match.Groups[1].Value);
                int seconds = int.Parse(match.Groups[2].Value);
                time = (minutes * 60) + seconds;
            }
            return (isOkTimeFormat, time);
        }

    }
}
