using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class SignalRHealthCheck : IHealthCheck
{
    private readonly string _hubUrl;

    public SignalRHealthCheck(IConfiguration config)
    {
        _hubUrl = config["SignalR:HubUrl"];
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = new HubConnectionBuilder()
                .WithUrl(_hubUrl)
                .Build();

            await connection.StartAsync(cancellationToken);
            await connection.StopAsync(cancellationToken);

            return HealthCheckResult.Healthy("SignalR reachable");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("SignalR unreachable", ex);
        }
    }
}
