using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Exceptions;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Hub;
using VideoApi.Events.Models;
using VideoApi.Events.ServiceBus;

namespace VideoApi.Events.Handlers
{
    public class HelpEventHandler : EventHandlerBase
    {
        public HelpEventHandler(IQueryHandler queryHandler, IServiceBusQueueClient serviceBusQueueClient,
            IHubContext<EventHub, IEventHubClient> hubContext) : base(queryHandler, serviceBusQueueClient, hubContext)
        {
        }

        public override EventType EventType => EventType.Help;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var vhOfficer = SourceConference.GetParticipants()
                .FirstOrDefault(x => x.UserRole == UserRole.VideoHearingsOfficer);

            if (vhOfficer == null)
            {
                throw new VideoHearingOfficerNotFoundException(SourceConference.HearingRefId);
            }

            await HubContext.Clients.Group(vhOfficer.Username.ToLowerInvariant())
                .HelpMessage(SourceConference.HearingRefId, SourceParticipant.DisplayName);
        }
    }
}