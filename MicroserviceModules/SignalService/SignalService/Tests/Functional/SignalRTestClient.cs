using Microsoft.AspNetCore.SignalR.Client;
using SignalService.Application.Contracts;

namespace SignalService.Tests.Functional
{
    public class SignalRTestClient : ISignalRTestClient
    {
        private readonly HubConnection _connection;

        public SignalRTestClient(string hubUrl, string apiKey)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.Headers.Add("X-Api-Key", apiKey);
                })
                .WithAutomaticReconnect()
                .Build();

            // Register handlers BEFORE connecting
            _connection.On<JobStatusUpdateDto>("JobStatusUpdated", update =>
            {
                Console.WriteLine($"[SignalR] Job {update.JobId} → {update.Status}");
            });
        }

        public async Task StartAsync()
        {
            if (_connection.State == HubConnectionState.Disconnected)
            {
                Console.WriteLine("[SignalR] Connecting...");
                await _connection.StartAsync();
                Console.WriteLine("[SignalR] Connected.");
            }
        }

        public async Task<JobRequestDto> SubmitJobAsync()
        {
            // Ensure connection is active
            await StartAsync();

            // Invoke hub method
            return await _connection.InvokeAsync<JobRequestDto>(
                "SendJob",
                new JobRequestCreateDto
                {
                    JobType = "TestJob",
                    PayloadJson = "{\"value\":456}"
                }
            );
        }
    }
}
