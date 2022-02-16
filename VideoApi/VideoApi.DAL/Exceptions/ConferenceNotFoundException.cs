using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VideoApi.DAL.Exceptions
{
    [Serializable]
    public class ConferenceNotFoundException : Exception
    {
        public ConferenceNotFoundException(Guid conferenceId) : base($"Conference {conferenceId} does not exist")
        {
        }

        public ConferenceNotFoundException(List<Guid> hearingIds) : base(
            $"No conference found with specified list of hearing ids: {hearingIds}")
        {
        }

        protected ConferenceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
