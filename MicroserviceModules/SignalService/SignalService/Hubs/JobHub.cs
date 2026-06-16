using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SignalService.Application.Contracts;
using SignalService.Application.Queues;
using SignalService.Domain.Entities;

namespace SignalService.Hubs
{
    [Authorize]
    public class JobHub : Hub
    {
        private readonly IJobQueue _jobQueue;

        public JobHub(IJobQueue jobQueue)
        {
            _jobQueue = jobQueue;
        }

        private string GetGroupName(string connectionId)
        {
            return $"client-{connectionId}";
        }

        public override async Task OnConnectedAsync()
        {
            var groupName = GetGroupName(Context.ConnectionId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await base.OnConnectedAsync();
        }

        public async Task<JobRequestDto> SendJob(JobRequestCreateDto dto)
        {
            var senderIp = Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var groupName = GetGroupName(Context.ConnectionId);

            var job = new JobRequest
            {
                SenderIp = senderIp,
                JobType = dto.JobType,
                PayloadJson = dto.PayloadJson,
                Status = "Queued",
                GroupName = groupName
            };

            // Attach group name to job metadata
            job.GroupName = groupName;

            await _jobQueue.EnqueueAsync(job);

            return new JobRequestDto
            {
                Id = job.Id,
                SenderIp = job.SenderIp,
                JobType = job.JobType,
                PayloadJson = job.PayloadJson,
                CreatedAtUtc = job.CreatedAtUtc,
                Status = job.Status,
                GroupName = job.GroupName
            };
        }

        // 🔥 Broadcast job status updates to all clients
        public async Task BroadcastJobStatus(JobStatusUpdateDto update)
        {
            await Clients.All.SendAsync("JobStatusUpdated", update);
        }
    }
}
