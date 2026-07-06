namespace LancachePrefill.Common
{
    // TODO Finish removing this in the other prefills.
    public sealed class NullableBoolConverter : BindingConverter<bool?>
    {
        // When a [CommandOption] is a bool, it will always display 'Default "False"' in the --help text.  This is redundant and fatiguing to read, so
        // switching the property over to bool? will make that text go away.  This however creates a new issue where passing the flag is no longer enough since "true",
        // is no longer the default.
        //
        // What this essentially does is when a flag is specified but no value is provided it is (null), so the converter will return true to restore the original
        // flag functionality.
        public override bool? Convert(string rawValue)
        {
            return true;
        }
    }

    public sealed class TransferSpeedUnitConverter : BindingConverter<TransferSpeedUnit>
    {
        public override TransferSpeedUnit Convert(string rawValue)
        {
            // This will throw an error if a user specifies '--unit' but does not provide a value.  Does not work with List<T>
            if (rawValue == null)
            {
                AnsiConsole.MarkupLine(Red($"A transfer speed unit must be specified when using {LightYellow("--unit")}"));
                AnsiConsole.Markup(Red($"Valid units include : {LightYellow("bits/bytes")}"));
                throw new CommandException(".", 1, true);
            }

            // Checking to make sure that the value provided is one of the enum's values
            rawValue = rawValue.ToLower();
            if (TransferSpeedUnit.TryFromValue(rawValue, out var _))
            {
                return TransferSpeedUnit.FromValue(rawValue);
            }

            AnsiConsole.MarkupLine(Red($"{White(rawValue)} is not a valid transfer speed unit!"));
            AnsiConsole.Markup(Red($"Valid units include : {LightYellow("bits/bytes")}"));
            throw new CommandException(".", 1, true);
        }
    }
}
