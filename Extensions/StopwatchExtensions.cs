namespace LancachePrefill.Common.Extensions
{
    public static class StopwatchExtensions
    {
        //TODO document
        public static string FormatElapsedString(this Stopwatch stopwatch)
        {
            var elapsed = stopwatch.Elapsed;
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
