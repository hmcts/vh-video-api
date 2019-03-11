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
        Task ConferenceStatusMessage(Guid hearingId, ConferenceState conferenceState);
        Task ConsultationMessage(Guid hearingId, string requestedBy, string requestedFor, string result);
        Task HelpMessage(Guid hearingId, string participantName);
    }
    
    [Authorize]
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