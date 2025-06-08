using System.Text;
using Azure_Az204.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Azure_Az204.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpdateController : ControllerBase
    {
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly ILogger<HomeController> _logger;
        public UpdateController(IHubContext<MessageHub> hubContext, ILogger<HomeController> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            _logger.LogInformation("Post called");
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var jsonContent = await reader.ReadToEndAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", jsonContent);
            return Ok();
        }
    }
}