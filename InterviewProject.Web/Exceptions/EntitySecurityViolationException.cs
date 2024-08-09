using System;
using System.Runtime.Serialization;

namespace InterviewProject.Exceptions
{
    [Serializable]
    public class EntitySecurityViolationException : Exception
    {
        public EntitySecurityViolationException()
        {
        }

        public EntitySecurityViolationException(string message)
            : base(message)
        {
        }

        public EntitySecurityViolationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public EntitySecurityViolationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}