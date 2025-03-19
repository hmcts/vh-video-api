using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.DTOs;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;
using Supplier = VideoApi.Domain.Enums.Supplier;

namespace VideoApi.DAL.Commands
{
    public class CreateConferenceCommand(
        Guid hearingRefId,
        string caseType,
        DateTime scheduledDateTime,
        string caseNumber,
        string caseName,
        int scheduledDuration,
        List<Participant> participants,
        string hearingVenueName,
        bool audioRecordingRequired,
        string ingestUrl,
        List<Endpoint> endpoints,
        List<LinkedParticipantDto> linkedParticipants,
        Supplier supplier,
        ConferenceRoomType conferenceRoomType,
        AudioPlaybackLanguage audioPlaybackLanguage)
        : ICommand
    {
        public Guid HearingRefId { get; } = hearingRefId;
        public string CaseType { get; } = caseType;
        public DateTime ScheduledDateTime { get; } = scheduledDateTime;
        public string CaseNumber { get; } = caseNumber;
        public string CaseName { get; } = caseName;
        public int ScheduledDuration { get; } = scheduledDuration;
        public Guid NewConferenceId { get; set; }
        public List<Participant> Participants { get; } = participants;
        public string HearingVenueName { get; } = hearingVenueName;
        public bool AudioRecordingRequired { get; set; } = audioRecordingRequired;
        public string IngestUrl { get; set; } = ingestUrl;
        public List<Endpoint> Endpoints { get; set; } = endpoints;
        public List<LinkedParticipantDto> LinkedParticipants { get; set; } = linkedParticipants;
        public Supplier Supplier { get; set; } = supplier;
        public ConferenceRoomType ConferenceRoomType { get; set; } = conferenceRoomType;
        public AudioPlaybackLanguage AudioPlaybackLanguage { get; } = audioPlaybackLanguage;
    }

    public class CreateConferenceCommandHandler(VideoApiDbContext context) : ICommandHandler<CreateConferenceCommand>
    {
        public async Task Handle(CreateConferenceCommand command)
        {
            var conference = new Conference(command.HearingRefId, command.CaseType, command.ScheduledDateTime,
                command.CaseNumber, command.CaseName, command.ScheduledDuration, command.HearingVenueName,
                command.AudioRecordingRequired, command.IngestUrl, command.Supplier, command.ConferenceRoomType,
                command.AudioPlaybackLanguage);
            
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
            
            context.Conferences.Add(conference);

            await context.SaveChangesAsync();           
            
            command.NewConferenceId = conference.Id;
        }
    }
}
