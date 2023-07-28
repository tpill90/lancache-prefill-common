namespace LancachePrefill.Common.Extensions
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return value.Substring(0, Math.Min(value.Length, maxLength));
        }

        /// <summary>
        /// Pads string with whitespace, taking the width of Unicode characters (2 wide) into account
        /// </summary>
        public static string PadRightUnicode(this string value, int totalWidth)
        {
            var unicodeWidth = value.Sum(t => UnicodeCalculator.GetWidth(t));

            // Adjusts the total padding by the additional width of the unicode characters
            return value.PadRight(totalWidth - (unicodeWidth - value.Length));
        }
    }
}
