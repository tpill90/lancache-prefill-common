namespace LancachePrefill.Common
{
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
