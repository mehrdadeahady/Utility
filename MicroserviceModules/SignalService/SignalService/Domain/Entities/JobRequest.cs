using System;
using System.ComponentModel.DataAnnotations;

namespace SignalService.Domain.Entities
{
    public class JobRequest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(64)]
        public string SenderIp { get; set; } = default!;

        [MaxLength(128)]
        public string? GroupName { get; set; }

        [Required]
        [MaxLength(128)]
        public string JobType { get; set; } = default!;

        [Required]
        [MaxLength(2048)]
        public string PayloadJson { get; set; } = default!;

        [Required]
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        [MaxLength(64)]
        public string? Status { get; set; } = "Queued";
    }
}
