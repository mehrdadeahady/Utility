using SignalService.Domain.Entities;

namespace SignalService.Application.Jobs
{
    public interface IJobPipeline
    {
        Task ExecuteAsync(JobRequest job);
    }
}
