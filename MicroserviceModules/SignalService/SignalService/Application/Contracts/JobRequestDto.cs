using System;

namespace SignalService.Application.Contracts
{
    public class JobRequestDto
    {
        public Guid Id { get; set; }
        public string? GroupName { get; set; }
        public string SenderIp { get; set; } = default!;
        public string JobType { get; set; } = default!;
        public string PayloadJson { get; set; } = default!;
        public DateTime CreatedAtUtc { get; set; }
        public string? Status { get; set; }
    }
}
