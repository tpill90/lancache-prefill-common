﻿namespace LancachePrefill.Common.Exceptions
{
    public class UserCancelledException : Exception
    {
        protected UserCancelledException(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public UserCancelledException()
        {

        }

        public UserCancelledException(string message) : base(message)
        {

        }

        public UserCancelledException(string message, Exception inner) : base(message, inner)
        {

        }
    }
}