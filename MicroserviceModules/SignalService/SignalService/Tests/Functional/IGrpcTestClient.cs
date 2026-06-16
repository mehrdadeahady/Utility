using SignalService.Grpc;

namespace SignalService.Tests.Functional
{
    public interface IGrpcTestClient
    {
        Task<JobCreateResponse> SubmitJobAsync();
        Task<JobStatusResponse> GetJobStatusAsync(string id);
    }
}
