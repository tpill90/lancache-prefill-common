namespace LancachePrefill.Common.Extensions
{
    public static class CliFxExtensions
    {
        private static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// Adds ".exe" to the example usage in help text CliFx displays
        /// </summary>
        public static CliApplicationBuilder SetExecutableNamePlatformAware(this CliApplicationBuilder builder, string appName)
        {
            if (IsWindows())
            {
                appName += ".exe";
            }
            builder.SetExecutableName(appName);
            return builder;
        }
    }
}
