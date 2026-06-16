using SignalService.Application.Contracts;

namespace SignalService.Tests.Functional
{
    public interface IWebApiTestClient
    {
        Task<JobRequestDto?> SubmitJobAsync();
        Task<string> CheckHealthAsync();
    }
}
