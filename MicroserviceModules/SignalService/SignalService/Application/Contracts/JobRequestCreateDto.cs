namespace SignalService.Application.Contracts
{
    public class JobRequestCreateDto
    {
        public string JobType { get; set; } = default!;
        public string PayloadJson { get; set; } = default!;
    }
}
