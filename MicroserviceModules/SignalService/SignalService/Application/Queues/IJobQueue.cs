using System.Threading;
using System.Threading.Tasks;
using SignalService.Domain.Entities;

namespace SignalService.Application.Queues
{
    public interface IJobQueue
    {
        Task EnqueueAsync(JobRequest job);
        Task<JobRequest?> DequeueAsync(CancellationToken cancellationToken);
    }
}
