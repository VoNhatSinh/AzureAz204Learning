using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Azure_Az204.Models;
using Azure.Messaging.ServiceBus;

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
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public async Task<ActionResult> SendToQueue(string message)
    {
        var client = new ServiceBusClient(_config["ServiceBusConnectionString"]);
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
        return View("Privacy");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
