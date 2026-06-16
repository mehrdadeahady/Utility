using Microsoft.AspNetCore.Mvc;
using SignalService.Application.Services;

namespace SignalService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceBusController : ControllerBase
    {
        private readonly ServiceBusJobSender _sender;

        public ServiceBusController(ServiceBusJobSender sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] object payload)
        {
            await _sender.SendMessageAsync(payload.ToString()!);
            return Ok(new { status = "sent" });
        }
    }
}
