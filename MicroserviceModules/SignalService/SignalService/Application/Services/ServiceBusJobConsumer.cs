using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SignalService.Application.Queues;
using SignalService.Domain.Entities;

namespace SignalService.Application.Services
{
    public class ServiceBusJobConsumer : BackgroundService
    {
        private readonly ILogger<ServiceBusJobConsumer> _logger;
        private readonly IJobQueue _jobQueue;
        private readonly ServiceBusProcessor _processor;

        public ServiceBusJobConsumer(
            ILogger<ServiceBusJobConsumer> logger,
            IJobQueue jobQueue,
            IConfiguration config)
        {
            _logger = logger;
            _jobQueue = jobQueue;

            var client = new ServiceBusClient(config["ServiceBus:ConnectionString"]);
            _processor = client.CreateProcessor(config["ServiceBus:QueueName"], new ServiceBusProcessorOptions());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += OnMessageReceived;
            _processor.ProcessErrorAsync += OnError;

            await _processor.StartProcessingAsync(stoppingToken);
        }

        private async Task OnMessageReceived(ProcessMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();

            var job = new JobRequest
            {
                SenderIp = "servicebus",
                JobType = "ServiceBusMessage",
                PayloadJson = body,
                Status = "Queued"
            };

            await _jobQueue.EnqueueAsync(job);

            await args.CompleteMessageAsync(args.Message);
        }

        private Task OnError(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Service Bus error");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }
    }
}
