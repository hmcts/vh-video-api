using System;
using System.Collections.Generic;
using VideoApi.DAL.Commands.Core;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class CreateConferenceCommand : ICommand
    {

        public Guid HearingRefId { get; }
        public string CaseType { get; }
        public DateTime ScheduledDateTime { get; }
        public string CaseNumber { get; }
        public string CaseName { get; }
        public int ScheduledDuration { get; }
        public Guid NewConferenceId { get; set; }
        public List<Participant> Participants { get; }
        public string HearingVenueName { get; }
        public bool AudioRecordingRequired { get; set; }
        public string IngestUrl { get; set; }
        public List<Endpoint> Endpoints { get; set; }

        public CreateConferenceCommand(Guid hearingRefId, string caseType, DateTime scheduledDateTime,
            string caseNumber, string caseName, int scheduledDuration, List<Participant> participants,
            string hearingVenueName, bool audioRecordingRequired, string ingestUrl, List<Endpoint> endpoints)
        {
            HearingRefId = hearingRefId;
            CaseType = caseType;
            ScheduledDateTime = scheduledDateTime;
            CaseNumber = caseNumber;
            CaseName = caseName;
            ScheduledDuration = scheduledDuration;
            Participants = participants;
            HearingVenueName = hearingVenueName;
            AudioRecordingRequired = audioRecordingRequired;
            IngestUrl = ingestUrl;
            Endpoints = endpoints;
        }
    }

    public class CreateConferenceCommandHandler : ICommandHandler<CreateConferenceCommand>
    {
        private readonly VideoApiDbContext _context;

        public CreateConferenceCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CreateConferenceCommand command)
        {
            var conference = new Conference(command.HearingRefId, command.CaseType, command.ScheduledDateTime,
                command.CaseNumber,command.CaseName, command.ScheduledDuration, command.HearingVenueName, command.AudioRecordingRequired, command.IngestUrl);
            
            foreach (var participant in command.Participants)
            {
                conference.AddParticipant(participant);
            }

            foreach (var endpoint in command.Endpoints)
            {
                conference.AddEndpoint(endpoint);
            }
            
            _context.Conferences.Add(conference);
            
            await _context.SaveChangesAsync();
            
            command.NewConferenceId = conference.Id;
        }
    }
}
