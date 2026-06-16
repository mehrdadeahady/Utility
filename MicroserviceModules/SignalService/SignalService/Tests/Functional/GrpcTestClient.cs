using Grpc.Core;
using Grpc.Net.Client;
using SignalService.Grpc;

namespace SignalService.Tests.Functional
{
    public class GrpcTestClient : IGrpcTestClient
    {
        private readonly JobService.JobServiceClient _client;
        private readonly Metadata _headers;

        public GrpcTestClient(string baseUrl, string apiKey)
        {
            var channel = GrpcChannel.ForAddress(baseUrl);
            _client = new JobService.JobServiceClient(channel);

            _headers = new Metadata
            {
                { "X-Api-Key", apiKey }
            };
        }

        public async Task<JobCreateResponse> SubmitJobAsync()
        {
            var request = new JobCreateRequest
            {
                JobType = "TestJob",
                PayloadJson = "{\"value\":789}"
            };

            return await _client.SubmitJobAsync(request, _headers);
        }

        public async Task<JobStatusResponse> GetJobStatusAsync(string id)
        {
            var request = new JobStatusRequest
            {
                Id = id
            };

            return await _client.GetJobStatusAsync(request, _headers);
        }
    }
}
