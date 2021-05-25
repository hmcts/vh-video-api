using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class LeaveEventHandler : EventHandlerBase<LeaveEventHandler>
    {
        public LeaveEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<LeaveEventHandler> logger) : base(
            queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.Leave;

        protected override Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var command = new UpdateParticipantStatusCommand(SourceConference.Id, SourceParticipant.Id, ParticipantState.Disconnected);
            
            _logger.LogInformation("{ConferenceId} {ParticipantId} Leave callback - {Tags}",
                SourceConference.Id, SourceParticipant.Id, new [] {"VIH-7730", "HearingEvent"});
            return CommandHandler.Handle(command);
        }
    }
}
