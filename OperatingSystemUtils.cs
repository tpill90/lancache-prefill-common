namespace LancachePrefill.Common
{
    public static class OperatingSystemUtils
    {
        /// <summary>
        /// Detects if the user has double clicked the prefill .exe from Windows explorer, and displays a warning
        /// that a CLI app must be correctly run from a terminal session.
        ///
        /// Only checks on Windows, ignores other operating systems.
        /// </summary>
        /// <param name="appName"></param>
        public static void DetectDoubleClickOnWindows(string appName)
        {
#if OS_WINDOWS
            var currentProcess = Process.GetCurrentProcess();
            var parentProcess = ParentProcessUtilities.GetParentProcess(currentProcess.Id);

            if (parentProcess.ProcessName != "explorer")
            {
                return;
            }

            var startInfo = new ProcessStartInfo {
               FileName = "powershell.exe",
               WorkingDirectory = Directory.GetCurrentDirectory()
            };
            Process.Start(startInfo);
#endif
        }
    }
}