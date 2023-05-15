using System;
using System.Runtime.Serialization;

namespace VideoApi.DAL.Exceptions
{
    public abstract class VideoDalException : Exception
    {
        protected VideoDalException(string message) : base(message)
        {
        }
        
        protected VideoDalException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}
