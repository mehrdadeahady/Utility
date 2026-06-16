using SignalService.Application.Contracts;

namespace SignalService.Tests.Functional
{
    public interface ISignalRTestClient
    {
        Task StartAsync();
        Task<JobRequestDto> SubmitJobAsync();
    }
}
