using System.Threading.Tasks;
using VideoApi.Domain.Enums;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers.Core
{
    public interface IEventHandler
    {
        EventType EventType { get; }
        Task HandleAsync(CallbackEvent callbackEvent);
    }
    
    public class EventHandler : IEventHandler
    {
        public EventType EventType { get; }
        public Task HandleAsync(CallbackEvent callbackEvent)
        {
            throw new System.NotImplementedException();
        }
    }
}