using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Azure_Az204.Models;
using Azure_Az204.Services;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Rest;

namespace Azure_Az204.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _config;
    public HomeController(ILogger<HomeController> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public IActionResult Index()
    {
        _logger.LogInformation("Index called");
        return View();
    }

    public IActionResult ServiceBus()
    {
        _logger.LogWarning("ServiceBus called");

        return View();
    }

    public IActionResult EventGrid()
    {
        return View();
    }

    [HttpPost]
    public IActionResult EventGrid(string userInput)
    {
        var topicKey = _config["NsvEventGridTopicKey"];
        _logger.LogInformation($"NsvEventGridTopicKey_{topicKey}");
        var client = new EventGridClient(new TopicCredentials(topicKey));
        var events = new List<EventGridEvent>() {
            new EventGridEvent()
            {
                Id = Guid.NewGuid().ToString(),
                Subject = "My Subject",
                Data = new { userInput = userInput },
                EventType = "microsoft.eventGrid.EventGridEvent",
            }
        };

        client.PublishEventsWithHttpMessagesAsync("NsvTopic", events);

        ViewBag.Message = $"You entered: {userInput}";

        return View();
    }

    [HttpGet]
    public async Task<ActionResult> SendToQueue(string message)
    {
        var client = new ServiceBusClient(ConnectionStringHelper.GetServiceBusConnectionString());
        var sender = client.CreateSender(_config["QueueName"]);
        using var messageBatch = await sender.CreateMessageBatchAsync();
        messageBatch.TryAddMessage(new ServiceBusMessage(message));
        try
        {
            await sender.SendMessagesAsync(messageBatch);
        }
        finally
        {
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }

        ViewBag.Message = message;
        return View("ServiceBus");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
