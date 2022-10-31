using System.Net.WebSockets;
using System.Text;
using Planner.WebSocketManager;
using RabbitMQ.Client;
namespace Planner.MessageQueue;

public class MessageProducer : IMessageProducer
{
    private readonly ConnectionManager _connManager;

    public MessageProducer(ConnectionManager connManager)
    {
        _connManager = connManager;
    }

    public Task SendMessageAsync(string message)
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare("plans", autoDelete: false, exclusive: false);
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "", routingKey: "plans", body: body);

        return Task.CompletedTask;
    }
}