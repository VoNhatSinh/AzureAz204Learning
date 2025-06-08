using System.Text;
using System.Text.Json;
using Azure_Az204.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Azure_Az204.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpdateController : ControllerBase
    {
        private readonly IHubContext<EventGridHub> _hubContext;
        private readonly ILogger<HomeController> _logger;
        public UpdateController(IHubContext<EventGridHub> hubContext, ILogger<HomeController> logger)
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

            try
            {
                // Parse as a JSON array
                var events = JsonSerializer.Deserialize<JsonElement[]>(jsonContent);
                foreach (var ev in events)
                {
                    var eventType = ev.GetProperty("eventType").GetString();

                    if (eventType == "Microsoft.EventGrid.SubscriptionValidationEvent")
                    {
                        var validationCode = ev.GetProperty("data").GetProperty("validationCode").GetString();
                        _logger.LogInformation("Subscription validation event received. ValidationCode: {ValidationCode}", validationCode);

                        var response = new
                        {
                            validationResponse = validationCode
                        };

                        return Ok(response);
                    }
                }

                // Not a validation event - broadcast to SignalR
                await _hubContext.Clients.All.SendAsync("UpdateMessage", jsonContent);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Event Grid message.");
                return BadRequest("Invalid request format.");
            }
        }

        [HttpOptions]
        public IActionResult Options()
        {
            return Ok();
        }
    }
}