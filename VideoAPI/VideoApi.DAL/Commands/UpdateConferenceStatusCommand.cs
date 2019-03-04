using System;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Commands
{
    public class UpdateConferenceStatusCommand : ICommand
    {
        public Guid ConferenceId { get; set; }
        public ConferenceState ConferenceState { get; set; }

        public UpdateConferenceStatusCommand(Guid conferenceId, ConferenceState conferenceState)
        {
            ConferenceId = conferenceId;
            ConferenceState = conferenceState;
        }
    }
}