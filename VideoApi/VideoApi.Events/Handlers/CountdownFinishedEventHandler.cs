using System.Linq;
using Microsoft.Extensions.Logging;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Events.Handlers
{
    public class CountdownFinishedEventHandler : EventHandlerBase<CountdownFinishedEventHandler>
    {
        private readonly IConsultationService _consultationService;
        public CountdownFinishedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<CountdownFinishedEventHandler> logger, IConsultationService consultationService) : base(queryHandler, commandHandler, logger)
        {
            _consultationService = consultationService;
        }

        public override EventType EventType => EventType.CountdownFinished;
        
        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var allWitnessesInConsultation = SourceConference.Participants.Where(x => x is Participant && ((Participant)x).IsAWitness())
                .Where(x => x.State == ParticipantState.InConsultation);
            foreach (var witness in allWitnessesInConsultation)
            {
                await ReturnParticipantToWaitingRoom(witness);
            }
        }
        
        private async Task ReturnParticipantToWaitingRoom(ParticipantBase witness)
        {
            var currentConsultationRoom = witness.GetCurrentRoom();
            await _consultationService.LeaveConsultationAsync(SourceConference.Id, witness.Id,
                currentConsultationRoom, RoomType.WaitingRoom.ToString());
        }
    }
}
