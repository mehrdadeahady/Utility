using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SignalService.Infrastructure.Health
{
    public class WorkerHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            // You can expand this later with real worker state
            return Task.FromResult(HealthCheckResult.Healthy("Worker running"));
        }
    }
}
