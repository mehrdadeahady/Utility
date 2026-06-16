using SignalService.Domain.Entities;

namespace SignalService.Application.Jobs
{
    public interface IJobMiddleware
    {
        Task InvokeAsync(JobRequest job, Func<Task> next);
    }
}
