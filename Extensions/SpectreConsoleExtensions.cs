﻿namespace LancachePrefill.Common.Extensions
{
    public static class SpectreConsoleExtensions
    {
        /// <summary>
        /// Creates an instance of <see cref="IAnsiConsole"/> that is integrated with CliFX
        /// </summary>
        /// <param name="console">IConsole instance provided by CliFX</param>
        /// <returns></returns>
        public static IAnsiConsole CreateAnsiConsole(this IConsole console)
        {
            return AnsiConsole.Create(new AnsiConsoleSettings
            {
                Ansi = AnsiSupport.Detect,
                ColorSystem = ColorSystemSupport.Detect,
                Out = new AnsiConsoleOutput(console.Output)
            });
        }

        public static Status StatusSpinner(this IAnsiConsole ansiConsole)
        {
            return ansiConsole.Status()
                              .AutoRefresh(true)
                              .SpinnerStyle(Style.Parse("green"))
                              .Spinner(Spinner.Known.Dots2);
        }

        public static Progress CreateSpectreProgress(this IAnsiConsole ansiConsole)
        {
            return CreateSpectreProgress(ansiConsole, TransferSpeedUnit.Bits, false);
        }

        public static Progress CreateSpectreProgress(this IAnsiConsole ansiConsole, TransferSpeedUnit unit, bool displayTransferRate = true)
        {
            var displayBits = unit == TransferSpeedUnit.Bits;
            var columns = new List<ProgressColumn>
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn()
            };
            if (displayTransferRate)
            {
                columns.Add(new DownloadedColumn());
                columns.Add(new TransferSpeedColumn { Base = FileSizeBase.Decimal, DisplayBits = displayBits });
            }
            var spectreProgress = ansiConsole.Progress()
                                             .HideCompleted(true)
                                             .AutoClear(true)
                                             .Columns(columns.ToArray());

            return spectreProgress;
        }

        // TODO this is Steam specific.  Should move out of here.
        public static async Task<string> ReadPasswordAsync(this IAnsiConsole console, string promptText = null)
        {
            var promptTask = Task.Run(() =>
            {
                var defaultPrompt = $"Please enter your {Cyan("Steam password")}. {LightYellow("(Password won't be saved)")} : ";
                return console.Prompt(new TextPrompt<string>(promptText ?? defaultPrompt)
                                      .PromptStyle("white")
                                      .Secret());
            });
            return await promptTask.WaitAsync(TimeSpan.FromSeconds(30));
        }

        public static Markup ToMarkup(this Object obj)
        {
            return new Markup(obj.ToString());
        }
    }
}
