using System;
using System.Collections.Generic;
using VideoApi.Domain;

namespace VideoApi.DAL.Commands
{
    public class AddParticipantsToConferenceCommand : ICommand
    {
        public Guid ConferenceId { get; set; }
        public List<Participant> Participants { get; set; }

        public AddParticipantsToConferenceCommand(Guid conferenceId, List<Participant> participants)
        {
            ConferenceId = conferenceId;
            Participants = participants;
        }
    }
}