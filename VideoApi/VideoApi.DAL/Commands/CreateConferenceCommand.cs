using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.DTOs;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;
using Supplier = VideoApi.Domain.Enums.Supplier;

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
        public List<LinkedParticipantDto> LinkedParticipants { get; set; }
        public Supplier Supplier { get; set; }

        public CreateConferenceCommand(Guid hearingRefId, string caseType, DateTime scheduledDateTime,
            string caseNumber, string caseName, int scheduledDuration, List<Participant> participants,
            string hearingVenueName, bool audioRecordingRequired, string ingestUrl, List<Endpoint> endpoints,
            List<LinkedParticipantDto> linkedParticipants, Supplier supplier)
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
            LinkedParticipants = linkedParticipants;
            Supplier = supplier;
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
                command.CaseNumber,command.CaseName, command.ScheduledDuration, command.HearingVenueName, command.AudioRecordingRequired, command.IngestUrl,
                command.Supplier);
            
            foreach (var participant in command.Participants)
            {
                conference.AddParticipant(participant);
            }
            
            foreach (var linkedParticipant in command.LinkedParticipants)
            {
                try
                {
                    var primaryParticipant =
                        conference.Participants.Single(x => x.ParticipantRefId == linkedParticipant.ParticipantRefId);
                
                    var secondaryParticipant =
                        conference.Participants.Single(x => x.ParticipantRefId == linkedParticipant.LinkedRefId);
                    
                    primaryParticipant.AddLink(secondaryParticipant.Id, linkedParticipant.Type);
                }
                catch (Exception)
                {
                    throw new ParticipantLinkException(linkedParticipant.ParticipantRefId, linkedParticipant.LinkedRefId);
                }
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
