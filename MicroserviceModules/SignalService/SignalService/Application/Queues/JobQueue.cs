using System.Threading.Channels;
using SignalService.Domain.Entities;

namespace SignalService.Application.Queues
{
    public class JobQueue : IJobQueue
    {
        private readonly Channel<JobRequest> _channel;

        public JobQueue()
        {
            var options = new BoundedChannelOptions(1000)
            {
                SingleReader = true,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.Wait
            };

            _channel = Channel.CreateBounded<JobRequest>(options);
        }

        public async Task EnqueueAsync(JobRequest job)
        {
            await _channel.Writer.WriteAsync(job);
        }

        public async Task<JobRequest?> DequeueAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (await _channel.Reader.WaitToReadAsync(cancellationToken))
                {
                    if (_channel.Reader.TryRead(out var job))
                        return job;
                }
            }
            catch (OperationCanceledException)
            {
                // Graceful shutdown
                return null;
            }

            return null;
        }
    }
}
