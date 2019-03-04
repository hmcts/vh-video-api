using System;
using System.Threading.Tasks;
using VideoApi.Domain;

namespace VideoApi.DAL.Commands
{
    public class CreateConferenceCommand : ICommand
    {

        public Guid HearingRefId { get; set; }
        public string CaseType { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string CaseNumber { get; set; }
        public Guid NewConferenceId { get; set; }

        public CreateConferenceCommand(Guid hearingRefId, string caseType, DateTime scheduledDateTime, string caseNumber)
        {
            HearingRefId = hearingRefId;
            CaseType = caseType;
            ScheduledDateTime = scheduledDateTime;
            CaseNumber = caseNumber;
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
                command.CaseNumber);
            _context.Conferences.Add(conference);
            await _context.SaveChangesAsync();
            command.NewConferenceId = conference.Id;
        }
    }
}