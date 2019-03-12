using System.Collections.Concurrent;
using System.Threading.Tasks;
using VideoApi.Events.Models;
using VideoApi.Events.ServiceBus;

namespace VideoApi.UnitTests.Stubs
{
    public class ServiceBusQueueClientStub : IServiceBusQueueClient
    {
        private readonly ConcurrentQueue<EventMessage> _eventMessages = new ConcurrentQueue<EventMessage>();

        public int Count => _eventMessages.Count;

        public Task AddMessageToQueue(EventMessage eventMessage)
        {
            _eventMessages.Enqueue(eventMessage);
            return Task.CompletedTask;
        }

        public EventMessage ReadMessageFromQueue()
        {
            _eventMessages.TryDequeue(out var message);
            return message;
        }
    }
}