using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SignalService.Application.Queues;
using SignalService.Domain.Entities;
using SignalService.Grpc;
using SignalService.Infrastructure;
using System.Net.NetworkInformation;

namespace SignalService.Services
{
    [Authorize]
    public class JobGrpcService : JobService.JobServiceBase
    {
        private readonly IJobQueue _jobQueue;
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JobGrpcService(
            IJobQueue jobQueue,
            AppDbContext db,
            IHttpContextAccessor httpContextAccessor)
        {
            _jobQueue = jobQueue;
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<JobCreateResponse> SubmitJob(
            JobCreateRequest request,
            ServerCallContext context)
        {
            var senderIp = _httpContextAccessor.HttpContext?
                .Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var job = new JobRequest
            {
                SenderIp = senderIp,
                JobType = request.JobType,
                PayloadJson = request.PayloadJson,
                Status = "Queued"
            };

            await _jobQueue.EnqueueAsync(job);

            return new JobCreateResponse
            {
                Id = job.Id.ToString(),
                SenderIp = job.SenderIp,
                JobType = job.JobType,
                PayloadJson = job.PayloadJson,
                CreatedAtUtc = job.CreatedAtUtc.ToString("o"),
                Status = job.Status
            };
        }

        public override async Task<JobStatusResponse> GetJobStatus(
            JobStatusRequest request,
            ServerCallContext context)
        {
            if (!Guid.TryParse(request.Id, out var jobId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid job ID"));

            var job = await _db.JobRequests.FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
                throw new RpcException(new Status(StatusCode.NotFound, "Job not found"));

            return new JobStatusResponse
            {
                Id = job.Id.ToString(),
                Status = job.Status ?? "Unknown",
                CreatedAtUtc = job.CreatedAtUtc.ToString("o")
            };
        }
    }
}
