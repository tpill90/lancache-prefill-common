namespace LancachePrefill.Common
{
    public static class OperatingSystemUtils
    {
        //TODO document
        public static void DetectDoubleClickOnWindows(string appName)
        {
#if OS_WINDOWS
            var currentProcess = Process.GetCurrentProcess();
            var parentProcess = ParentProcessUtilities.GetParentProcess(currentProcess.Id);

            if (parentProcess.ProcessName != "explorer")
            {
                return;
            }

            AnsiConsole.Console.MarkupLine($"Detected that {Cyan(appName)} was started via double click!");
            AnsiConsole.Console.MarkupLine($"{Cyan(appName)} is a command line app that must be started from a Powershell prompt.");
            AnsiConsole.Console.MarkupLine("");
            AnsiConsole.Console.MarkupLine("To run the app, open a Powershell prompt in this directory, " +
                                           $"and start the app with {LightYellow($".\\{appName}.exe")}");

            // Blocking on user input, so that they have a chance to read the output.
            // No need to exit() the process here, it will immediately close itself anyway.
            Console.ReadLine();
#endif
        }
    }

    // This code uses Windows kernel specific functions, shouldn't be included in compilation on other platforms
}