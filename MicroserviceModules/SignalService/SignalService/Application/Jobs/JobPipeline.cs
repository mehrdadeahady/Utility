using SignalService.Domain.Entities;

namespace SignalService.Application.Jobs
{
    public class JobPipeline : IJobPipeline
    {
        private readonly IList<IJobMiddleware> _middlewares;

        public JobPipeline(IEnumerable<IJobMiddleware> middlewares)
        {
            _middlewares = middlewares.ToList();
        }

        public Task ExecuteAsync(JobRequest job)
        {
            return ExecuteMiddleware(0, job);
        }

        private Task ExecuteMiddleware(int index, JobRequest job)
        {
            if (index == _middlewares.Count)
                return Task.CompletedTask;

            return _middlewares[index].InvokeAsync(job, () => ExecuteMiddleware(index + 1, job));
        }
    }
}
