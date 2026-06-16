using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalService.Application.Contracts;
using SignalService.Application.Queues;
using SignalService.Domain.Entities;
using SignalService.Infrastructure;

namespace SignalService.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IJobQueue _jobQueue;

        public JobsController(AppDbContext db, IJobQueue jobQueue)
        {
            _db = db;
            _jobQueue = jobQueue;
        }

        // POST: api/jobs
        [HttpPost]
        public async Task<ActionResult<JobRequestDto>> CreateJob([FromBody] JobRequestCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var senderIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var job = new JobRequest
            {
                SenderIp = senderIp,
                JobType = dto.JobType,
                PayloadJson = dto.PayloadJson,
                Status = "Queued"
            };

            await _jobQueue.EnqueueAsync(job);

            var result = new JobRequestDto
            {
                Id = job.Id,
                SenderIp = job.SenderIp,
                JobType = job.JobType,
                PayloadJson = job.PayloadJson,
                CreatedAtUtc = job.CreatedAtUtc,
                Status = job.Status
            };

            return CreatedAtAction(nameof(GetJobById), new { id = job.Id }, result);
        }

        // GET: api/jobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobRequestDto>>> GetJobs()
        {
            var jobs = await _db.JobRequests
                .OrderByDescending(j => j.CreatedAtUtc)
                .Take(100)
                .ToListAsync();

            var result = jobs.Select(j => new JobRequestDto
            {
                Id = j.Id,
                SenderIp = j.SenderIp,
                JobType = j.JobType,
                PayloadJson = j.PayloadJson,
                CreatedAtUtc = j.CreatedAtUtc,
                Status = j.Status
            });

            return Ok(result);
        }

        // GET: api/jobs/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<JobRequestDto>> GetJobById(Guid id)
        {
            var job = await _db.JobRequests.FindAsync(id);
            if (job == null)
                return NotFound();

            var result = new JobRequestDto
            {
                Id = job.Id,
                SenderIp = job.SenderIp,
                JobType = job.JobType,
                PayloadJson = job.PayloadJson,
                CreatedAtUtc = job.CreatedAtUtc,
                Status = job.Status
            };

            return Ok(result);
        }

        // PUT: api/jobs/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateJobStatus(Guid id, [FromBody] string status)
        {
            var job = await _db.JobRequests.FindAsync(id);
            if (job == null)
                return NotFound();

            job.Status = status;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/jobs/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteJob(Guid id)
        {
            var job = await _db.JobRequests.FindAsync(id);
            if (job == null)
                return NotFound();

            _db.JobRequests.Remove(job);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
