using Grpc.Net.Client;
using SignalService.Grpc;

namespace SignalService.Tests.Functional
{
    public class GrpcTestService : IGrpcTestClient
    {
        public async Task<JobCreateResponse> SubmitJobAsync()
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new JobService.JobServiceClient(channel);

            var request = new JobCreateRequest
            {
                JobType = "TestJob",
                PayloadJson = "{\"value\":123}"
            };

            return await client.SubmitJobAsync(request);
        }

        public async Task<JobStatusResponse> GetJobStatusAsync(string id)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new JobService.JobServiceClient(channel);

            var request = new JobStatusRequest { Id = id };

            return await client.GetJobStatusAsync(request);
        }
    }
}
