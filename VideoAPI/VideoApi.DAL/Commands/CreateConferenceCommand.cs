using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class CreateConferenceCommand : ICommand
    {

        public Guid HearingRefId { get; set; }
        public string CaseType { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string CaseNumber { get; set; }
        public string CaseName { get; set; }
        public int ScheduledDuration { get; set; }
        public Guid NewConferenceId { get; set; }
        public List<Participant> Participants { get; set; }

        public CreateConferenceCommand(Guid hearingRefId, string caseType, DateTime scheduledDateTime,
            string caseNumber, string caseName, int scheduledDuration, List<Participant> participants)
        {
            HearingRefId = hearingRefId;
            CaseType = caseType;
            ScheduledDateTime = scheduledDateTime;
            CaseNumber = caseNumber;
            CaseName = caseName;
            ScheduledDuration = scheduledDuration;
            Participants = participants;
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
                command.CaseNumber,command.CaseName, command.ScheduledDuration);
            foreach (var participant in command.Participants)
            {
                conference.AddParticipant(participant);
            }
            _context.Conferences.Add(conference);
            await _context.SaveChangesAsync();
            command.NewConferenceId = conference.Id;
        }
    }
}