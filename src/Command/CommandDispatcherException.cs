using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace SFA.DAS.Apprenticeships.Command
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public sealed class CommandDispatcherException : Exception
    {
        public CommandDispatcherException()
        {
        }

        public CommandDispatcherException(string message)
            : base(message)
        {
        }

        public CommandDispatcherException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private CommandDispatcherException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}