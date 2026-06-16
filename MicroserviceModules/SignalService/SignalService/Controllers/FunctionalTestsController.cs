using Microsoft.AspNetCore.Mvc;
using SignalService.Tests.Functional;

namespace SignalService.Controllers
{
    public class FunctionalTestsController : Controller
    {
        private readonly IWebApiTestClient _webApi;
        private readonly IGrpcTestClient _grpc;
        private readonly ISignalRTestClient _signalR;
        private readonly IServiceBusTestClient _serviceBus;

        public FunctionalTestsController(
            IWebApiTestClient webApi,
            IGrpcTestClient grpc,
            ISignalRTestClient signalR,
            IServiceBusTestClient serviceBus)
        {
            _webApi = webApi;
            _grpc = grpc;
            _signalR = signalR;
            _serviceBus = serviceBus;
        }

        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> RunWebApiTest()
        {
            var result = await _webApi.SubmitJobAsync();
            return Json(result);
        }

        [HttpGet]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        [HttpPost]
        public async Task<IActionResult> RunGrpcTest()
        {
            var result = await _grpc.SubmitJobAsync();
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> RunSignalRTest()
        {
            var result = await _signalR.SubmitJobAsync();
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> RunServiceBusTest()
        {
            await _serviceBus.PublishJobAsync("{\"value\":999}");
            return Json(new { status = "Message published" });
        }

        [HttpPost]
        public async Task<IActionResult> RunHealthCheck()
        {
            var json = await _webApi.CheckHealthAsync();
            return Content(json, "application/json");
        }

        public IActionResult Health()
        {
            return View();
        }

    }
}
