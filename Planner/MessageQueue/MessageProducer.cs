using System.Text;
using RabbitMQ.Client;
namespace Planner.MessageQueue;

public class MessageProducer : IMessageProducer
{
    public void SendMessage(string message)
    {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare("plans", exclusive: false);
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "", routingKey: "plans", body: body);
    }
}