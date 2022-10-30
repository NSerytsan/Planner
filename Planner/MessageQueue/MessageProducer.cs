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

    public async Task SendMessageAsync(string message)
    {
        /*
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare("plans", exclusive: false);
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "", routingKey: "plans", body: body);
            */

        foreach (var dict in _connManager.GetAll())
        {
            var socket = dict.Value;

            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(message),
                                                          offset: 0,
                                                          count: message.Length),
                           messageType: WebSocketMessageType.Text,
                           endOfMessage: true,
                           cancellationToken: CancellationToken.None);
            }
        }

    }
}