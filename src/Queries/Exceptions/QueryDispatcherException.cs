﻿using System.Runtime.Serialization;

namespace SFA.DAS.Learning.Queries.Exceptions
{
    [Serializable]
    public sealed class QueryDispatcherException : Exception
    {
        public QueryDispatcherException()
        {
        }

        public QueryDispatcherException(string message)
            : base(message)
        {
        }

        public QueryDispatcherException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private QueryDispatcherException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}