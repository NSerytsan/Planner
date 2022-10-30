using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Planner.Controllers;

public class WebSocketController : ControllerBase
{
    private readonly ILogger<WebSocketController> _logger;

    public WebSocketController(ILogger<WebSocketController> logger)
    {
        _logger = logger;
    }

    [HttpGet("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _logger.LogInformation("Accept socket");
            await ProcessMessages(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private static async Task ProcessMessages(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];

        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = factory.CreateConnection();

        using var channel = connection.CreateModel();
        channel.QueueDeclare("plans", exclusive: false);
        var consumer = new EventingBasicConsumer(channel);
        while (true)
        {
            consumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
            };

            channel.BasicConsume(queue: "plans", autoAck: true, consumer: consumer);
        }
    }
}