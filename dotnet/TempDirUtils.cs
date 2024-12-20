namespace LancachePrefill.Common
{
    public static class TempDirUtils
    {
        /// <summary>
        /// Gets the base directories for the temp folder, determined by which operating system the app is currently running on.
        /// </summary>
        /// <param name="appName">The name of the current application.  Ex. "SteamPrefill"</param>
        /// <param name="tempDirVersion">Used to version the temp directory, in case there are breaking changes.  Increment the value to when there are breaking changes.  Ex. "v1"</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string GetTempDirBaseDirectories(string appName, string tempDirVersion)
        {
            if (OperatingSystem.IsWindows())
            {
                string pathAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(pathAppData, appName, tempDirVersion);
            }
            if (OperatingSystem.IsLinux())
            {
                // Gets base directories for the XDG Base Directory specification (Linux)
                string pathHome = Environment.GetEnvironmentVariable("HOME");
                if (pathHome == null)
                {
                    throw new Exception("Could not determine HOME directory");
                }

                string pathXdgCacheHome = Environment.GetEnvironmentVariable("XDG_CACHE_HOME");
                if (pathXdgCacheHome == null)
                {
                    pathXdgCacheHome = Path.Combine(pathHome, ".cache");
                }

                return Path.Combine(pathXdgCacheHome, appName, tempDirVersion);
            }
            if (OperatingSystem.IsMacOS())
            {
                string pathHome = Environment.GetEnvironmentVariable("HOME");
                return Path.Combine(pathHome, "Library", "Caches", appName, tempDirVersion);
            }

            throw new NotSupportedException($"Unknown platform {RuntimeInformation.OSDescription}");
        }
    }
}
