using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VideoApi.Common.Configuration;
using VideoApi.Events.Models;

namespace VideoApi.Events.ServiceBus
{
    public interface IServiceBusQueueClient
    {
        Task AddMessageToQueue(EventMessage eventMessage);
    }

    public class ServiceBusQueueClient : IServiceBusQueueClient
    {
        private readonly ServiceBusSettings _serviceBusSettings;

        public ServiceBusQueueClient(IOptions<ServiceBusSettings> serviceBusSettings)
        {
            _serviceBusSettings = serviceBusSettings.Value;
        }

        public async Task AddMessageToQueue(EventMessage eventMessage)
        {
            var queueClient = new QueueClient(_serviceBusSettings.ConnectionString,
                _serviceBusSettings.QueueName);
            var jsonObjectString = JsonConvert.SerializeObject(eventMessage);

            var messageBytes = Encoding.UTF8.GetBytes(jsonObjectString);
            await queueClient.SendAsync(new Message(messageBytes));
        }
    }
}