using System;
using System.Collections.Generic;
using System.Text;

namespace VideoApi.DAL.Exceptions
{
    public class ConferenceNotFoundException : Exception
    {
        public ConferenceNotFoundException(Guid conferenceId) : base($"Conference {conferenceId} does not exist")
        {
        }
    }
}
