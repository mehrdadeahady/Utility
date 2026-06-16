using System.Net.Http.Json;
using SignalService.Application.Contracts;

namespace SignalService.Tests.Functional
{
    public class WebApiTestClient : IWebApiTestClient
    {
        private readonly HttpClient _client;

        public WebApiTestClient(string baseUrl, string apiKey)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };

            _client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        }

        public async Task<JobRequestDto?> SubmitJobAsync()
        {
            var payload = new JobRequestCreateDto
            {
                JobType = "TestJob",
                PayloadJson = "{\"value\":123}"
            };

            var response = await _client.PostAsJsonAsync("/api/jobs", payload);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<JobRequestDto>();
        }

        public async Task<string> CheckHealthAsync()
        {
            var response = await _client.GetAsync("/health");
            return await response.Content.ReadAsStringAsync();
        }

    }
}
