using System;

namespace SignalService.Application.Contracts
{
    public class JobStatusUpdateDto
    {
        public Guid JobId { get; set; }
        public string Status { get; set; } = default!;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
