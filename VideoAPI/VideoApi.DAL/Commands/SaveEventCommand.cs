using System;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Commands
{
    public class SaveEventCommand : ICommand
    {
        public SaveEventCommand(Guid conferenceId, string externalEventId, DateTime externalTimestamp,
            long participantId, RoomType? transferredFrom, RoomType? transferredTo, string reason)
        {
            ConferenceId = conferenceId;
            ExternalEventId = externalEventId;
            ExternalTimestamp = externalTimestamp;
            ParticipantId = participantId;
            TransferredFrom = transferredFrom;
            TransferredTo = transferredTo;
            Reason = reason;
        }

        public Guid ConferenceId { get; set; }
        public string ExternalEventId { get; set; }
        public DateTime ExternalTimestamp { get; set; }
        public long ParticipantId { get; set; }
        public RoomType? TransferredFrom { get; set; }
        public RoomType? TransferredTo { get; set; }
        public string Reason { get; set; }
    }

    public class SaveEventCommandHandler : ICommandHandler<SaveEventCommand>
    {
        private readonly VideoApiDbContext _context;
        public SaveEventCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public Task Handle(SaveEventCommand command)
        {
            throw new System.NotImplementedException();
        }
    }
}