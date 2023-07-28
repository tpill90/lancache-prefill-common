namespace LancachePrefill.Common.Extensions
{
    public static class StopwatchExtensions
    {
        public static string FormatElapsedString(this Stopwatch stopwatch)
        {
            return FormatElapsedString(stopwatch.Elapsed);
        }

        /// <summary>
        /// Formats the elapsed time, omitting any leading 0's.
        ///
        /// For example, if the total elapsed time 15 minutes, the result would be "15:00.00
        /// </summary>
        public static string FormatElapsedString(this TimeSpan elapsed)
        {
            if (elapsed.TotalHours > 1)
            {
                return elapsed.ToString(@"h\:mm\:ss\.ff");
            }
            if (elapsed.TotalMinutes > 1)
            {
                return elapsed.ToString(@"mm\:ss\.ff");
            }
            return elapsed.ToString(@"ss\.ffff");
        }
    }
}
