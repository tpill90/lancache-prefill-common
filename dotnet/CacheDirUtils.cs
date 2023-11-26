namespace LancachePrefill.Common
{
    public static class CacheDirUtils
    {
        /// <summary>
        /// Gets the base directories for the cache folder, determined by which Operating System the app is currently running on.
        /// </summary>
        /// <param name="appName">The name of the current application.  Ex. "SteamPrefill"</param>
        /// <param name="cacheDirVersion">Used to version the cache directory, in case there are breaking changes.  Increment the value to when there are breaking changes.  Ex. "v1"</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string GetCacheDirBaseDirectories(string appName, string cacheDirVersion)
        {
            if (OperatingSystem.IsWindows())
            {
                string pathAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(pathAppData, appName, "Cache", cacheDirVersion);
            }
            if (OperatingSystem.IsLinux())
            {
                // Gets base directories for the XDG Base Directory specification (Linux)
                string pathHome = Environment.GetEnvironmentVariable("HOME")
                                  ?? throw new Exception("Could not determine HOME directory");

                string pathXdgCacheHome = Environment.GetEnvironmentVariable("XDG_CACHE_HOME")
                                          ?? Path.Combine(pathHome, ".cache");

                return Path.Combine(pathXdgCacheHome, appName, cacheDirVersion);
            }
            if (OperatingSystem.IsMacOS())
            {
                string pathLibraryCaches = Path.GetFullPath("~/Library/Caches");
                return Path.Combine(pathLibraryCaches, appName, cacheDirVersion);
            }

            throw new NotSupportedException($"Unknown platform {RuntimeInformation.OSDescription}");
        }
    }
}
