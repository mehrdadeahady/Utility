using Microsoft.Extensions.Logging;
using SignalService.Domain.Entities;

namespace SignalService.Application.Jobs.Middleware
{
    public class LoggingMiddleware : IJobMiddleware
    {
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(JobRequest job, Func<Task> next)
        {
            _logger.LogInformation("Starting job {JobId}", job.Id);

            await next();

            _logger.LogInformation("Finished job {JobId}", job.Id);
        }
    }
}
