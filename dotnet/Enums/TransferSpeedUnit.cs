namespace LancachePrefill.Common.Enums
{
    [Intellenum(typeof(string))]
    public sealed partial class TransferSpeedUnit
    {
        public static readonly TransferSpeedUnit Bits = new("bits");
        public static readonly TransferSpeedUnit Bytes = new("bytes");
    }
}