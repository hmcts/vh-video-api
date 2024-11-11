using System;

namespace VideoApi.DAL.Exceptions
{
    public class ConferenceNotFoundException : EntityNotFoundException
    {
        public ConferenceNotFoundException(Guid conferenceId) : base($"Conference {conferenceId} does not exist")
        {
        }
    }
}
