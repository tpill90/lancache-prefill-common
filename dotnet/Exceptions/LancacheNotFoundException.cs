namespace LancachePrefill.Common.Exceptions
{
    public sealed class LancacheNotFoundException : Exception
    {
        public LancacheNotFoundException()
        {

        }

        public LancacheNotFoundException(string message) : base(message)
        {

        }

        public LancacheNotFoundException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}