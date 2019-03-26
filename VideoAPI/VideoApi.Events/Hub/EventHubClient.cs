using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using VideoApi.Domain.Enums;

namespace VideoApi.Events.Hub
{
    public interface IEventHubClient
    {
        Task ParticipantStatusMessage(string email, ParticipantState participantState);
        Task ConferenceStatusMessage(Guid conferenceId, ConferenceState conferenceState);
        Task ConsultationMessage(Guid conferenceId, string requestedBy, string requestedFor, string result);
        Task HelpMessage(Guid conferenceId, string participantName);
    }

    [Authorize(Policy = "EventHubUser")]
    public class EventHub : Hub<IEventHubClient>
    {
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            await base.OnDisconnectedAsync(exception);
        }
    }
}