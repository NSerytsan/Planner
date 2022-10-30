using System.Net.WebSockets;
using System.Text;
using Planner.WebSocketManager;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class MQMonitor : IHostedService
{
    private readonly ILogger<MQMonitor> _logger;
    private readonly ConnectionManager _connManager;

    public MQMonitor(ILogger<MQMonitor> logger, ConnectionManager connManager)
    {
        _logger = logger;
        _connManager = connManager;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            var connection = factory.CreateConnection();
            
            while (!cancellationToken.IsCancellationRequested)
            {
                using var channel = connection.CreateModel();
                channel.QueueDeclare("plans", exclusive: false);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    
                    
                    foreach (var dict in _connManager.GetAll())
                    {
                        var socket = dict.Value;
                        if (socket.State == WebSocketState.Open)
                        {
                            
                            socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(message),
                                                                      offset: 0,
                                                                      count: message.Length),
                                       messageType: WebSocketMessageType.Text,
                                       endOfMessage: true,
                                       cancellationToken: CancellationToken.None);
                        }
                    }
                };
                channel.BasicConsume(queue: "plans", autoAck: true, consumer: consumer);
                
            }
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}