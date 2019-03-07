using System.Threading.Tasks;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;

namespace VideoApi.Events.Handlers
{
    public interface IEventHandler
    {
        EventType EventType { get; }
        Task HandleAsync(ConferenceEventRequest conferenceEventRequest);
    }
    
    public class EventHandler : IEventHandler
    {
        public EventType EventType { get; }
        public Task HandleAsync(ConferenceEventRequest conferenceEventRequest)
        {
            throw new System.NotImplementedException();
        }
    }
}