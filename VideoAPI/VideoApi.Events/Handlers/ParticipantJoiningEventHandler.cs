using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Events.ServiceBus;

namespace VideoApi.Events.Handlers
{
    public class ParticipantJoiningEventHandler : EventHandlerBase
    {
        public ParticipantJoiningEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IServiceBusQueueClient serviceBusQueueClient) : base(
            queryHandler, commandHandler, serviceBusQueueClient)
        {
        }

        public override EventType EventType => EventType.ParticipantJoining;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var participantState = ParticipantState.Joining;

            var command = new UpdateParticipantStatusCommand(SourceConference.Id, 
                SourceParticipant.Id, participantState);
            await CommandHandler.Handle(command);
        }
    }
}
