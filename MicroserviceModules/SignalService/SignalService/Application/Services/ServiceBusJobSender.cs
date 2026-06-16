using Azure.Messaging.ServiceBus;

namespace SignalService.Application.Services
{
    public class ServiceBusJobSender
    {
        private readonly ServiceBusSender _sender;

        public ServiceBusJobSender(IConfiguration config)
        {
            var client = new ServiceBusClient(config["ServiceBus:ConnectionString"]);
            _sender = client.CreateSender(config["ServiceBus:QueueName"]);
        }

        public async Task SendMessageAsync(string json)
        {
            var message = new ServiceBusMessage(json);
            await _sender.SendMessageAsync(message);
        }
    }
}
