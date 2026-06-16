using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SignalService.HealthChecks
{
    public class WebApiHealthCheck : IHealthCheck
    {
        private readonly HttpClient _client;

        public WebApiHealthCheck(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("webapi");
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // IMPORTANT: do NOT call /health (recursive)
                var response = await _client.GetAsync("/FunctionalTests/Ping", cancellationToken);

                if (response.IsSuccessStatusCode)
                    return HealthCheckResult.Healthy("Web API reachable");

                return HealthCheckResult.Unhealthy("Web API returned error");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Web API unreachable", ex);
            }
        }
    }
}
