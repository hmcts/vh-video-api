using System;
using System.Runtime.Serialization;

namespace VideoApi.DAL.Exceptions
{
    [Serializable]
    public class ConferenceNotFoundException : Exception
    {
        public ConferenceNotFoundException(Guid conferenceId) : base($"Conference {conferenceId} does not exist")
        {
        }
        
        protected ConferenceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
