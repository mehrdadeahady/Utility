using Grpc.Health.V1;
using Grpc.Net.Client;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class GrpcHealthCheck : IHealthCheck
{
    private readonly string _grpcUrl;

    public GrpcHealthCheck(IConfiguration config)
    {
        _grpcUrl = config["Grpc:BaseUrl"];
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var channel = GrpcChannel.ForAddress(_grpcUrl);
            var client = new Health.HealthClient(channel);

            // ⭐ CRITICAL FIX: add a strict timeout to prevent hanging
            var deadline = DateTime.UtcNow.AddSeconds(2);

            var reply = await client.CheckAsync(
                new HealthCheckRequest(),
                deadline: deadline,
                cancellationToken: cancellationToken
            );

            return reply.Status == HealthCheckResponse.Types.ServingStatus.Serving
                ? HealthCheckResult.Healthy("gRPC reachable")
                : HealthCheckResult.Unhealthy("gRPC not serving");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("gRPC unreachable", ex);
        }
    }
}
