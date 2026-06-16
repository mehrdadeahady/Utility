using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SignalService.Application.Contracts;
using SignalService.Application.Jobs;
using SignalService.Application.Queues;
using SignalService.Hubs;
using SignalService.Infrastructure;

namespace SignalService.Application.Services
{
    public class JobWorker : BackgroundService
    {
        private readonly ILogger<JobWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IJobQueue _jobQueue;
        private readonly IHubContext<JobHub> _hubContext;

        public JobWorker(
            ILogger<JobWorker> logger,
            IServiceScopeFactory scopeFactory,
            IJobQueue jobQueue,
            IHubContext<JobHub> hubContext)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _jobQueue = jobQueue;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("JobWorker started.");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var job = await _jobQueue.DequeueAsync(stoppingToken);

                    if (job is null)
                        continue;

                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var pipeline = scope.ServiceProvider.GetRequiredService<IJobPipeline>();

                        var groupName = job.GroupName;

                        job.Status = "Processing";
                        await db.JobRequests.AddAsync(job, stoppingToken);
                        await db.SaveChangesAsync(stoppingToken);

                        var processingUpdate = new JobStatusUpdateDto
                        {
                            JobId = job.Id,
                            Status = "Processing"
                        };

                        if (!string.IsNullOrWhiteSpace(groupName))
                        {
                            await _hubContext.Clients.Group(groupName)
                                .SendAsync("JobStatusUpdated", processingUpdate);
                        }

                        await pipeline.ExecuteAsync(job);

                        job.Status = "Completed";
                        await db.SaveChangesAsync(stoppingToken);

                        var completedUpdate = new JobStatusUpdateDto
                        {
                            JobId = job.Id,
                            Status = "Completed"
                        };

                        if (!string.IsNullOrWhiteSpace(groupName))
                        {
                            await _hubContext.Clients.Group(groupName)
                                .SendAsync("JobStatusUpdated", completedUpdate);
                        }

                        _logger.LogInformation("Job {JobId} processed.", job.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing job {JobId}.", job.Id);

                        var failedUpdate = new JobStatusUpdateDto
                        {
                            JobId = job.Id,
                            Status = "Failed"
                        };

                        var groupName = job.GroupName;

                        if (!string.IsNullOrWhiteSpace(groupName))
                        {
                            await _hubContext.Clients.Group(groupName)
                                .SendAsync("JobStatusUpdated", failedUpdate);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Graceful shutdown — expected during host stop
            }

            _logger.LogInformation("JobWorker stopping.");
        }
    }
}
