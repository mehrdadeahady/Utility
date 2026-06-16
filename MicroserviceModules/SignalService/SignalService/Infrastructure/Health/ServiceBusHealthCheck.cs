using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SignalService.Infrastructure.Health
{
    public class ServiceBusHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _config;

        public ServiceBusHealthCheck(IConfiguration config)
        {
            _config = config;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var client = new ServiceBusClient(_config["ServiceBus:ConnectionString"]);

                // ⭐ Correct lightweight check: try opening a sender
                var sender = client.CreateSender(_config["ServiceBus:QueueName"]);
                await sender.CloseAsync(cancellationToken);

                return HealthCheckResult.Healthy("Service Bus reachable");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Service Bus unreachable", ex);
            }
        }
    }
}
