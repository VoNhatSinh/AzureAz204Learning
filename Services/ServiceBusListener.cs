using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.SignalR;

namespace Azure_Az204.Services
{
    public class ServiceBusListener : BackgroundService
    {
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly IConfiguration _configuration;
        private ServiceBusClient _client;

        public ServiceBusListener(IHubContext<MessageHub> hubContext, IConfiguration configuration)
        {
            _hubContext = hubContext;
            _configuration = configuration;

        }

        async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string message = args.Message.Body.ToString();
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);

            // complete the message. message is deleted from the queue.
            await args.CompleteMessageAsync(args.Message);
        }

        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connectionString = Environment.GetEnvironmentVariable("SERVICEBUSCONNSTR_ServiceBusConnectionString");
            _client = new ServiceBusClient(connectionString, new ServiceBusClientOptions());
            var processor = _client.CreateProcessor(_configuration["QueueName"]);
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;
            await processor.StartProcessingAsync(stoppingToken);
        }

    }
}