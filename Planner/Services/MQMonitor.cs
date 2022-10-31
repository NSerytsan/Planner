using System.Net.WebSockets;
using System.Text;
using Planner.WebSocketManager;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class MQMonitor : BackgroundService
{
    private readonly ILogger<MQMonitor> _logger;
    private readonly ConnectionManager _connManager;
    private ConnectionFactory _connectionFactory;
    private IConnection _connection;
    private IModel _channel;
    private const string QueueName = "plans";

    public MQMonitor(ILogger<MQMonitor> logger, ConnectionManager connManager)
    {
        _logger = logger;
        _connManager = connManager;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _connectionFactory = new ConnectionFactory { HostName = "localhost", DispatchConsumersAsync = true };

        _connection = _connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclarePassive(QueueName);
        _channel.BasicQos(0, 1, false);
        _logger.LogInformation($"Queue [{QueueName}] is waiting for messages.");

        return base.StartAsync(cancellationToken);
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (ch, ea) =>
        {
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            await HandleMessageAsync(message);
            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }

    private async Task HandleMessageAsync(string message)
    {
        _logger.LogInformation($"consumer received {message}");
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

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
        _connection.Close();
        _logger.LogInformation("RabbitMQ connection is closed.");
    }

    /*

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = "localhost"
                };
                var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare("plans", exclusive: false);
                var consumer = new EventingBasicConsumer(channel);


                consumer.Received += async (model, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    _logger.Log(LogLevel.Information, "++++++++++++++++++++++++++++++++++++");
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
                };

                channel.BasicConsume(queue: "plans", autoAck: true, consumer: consumer);
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        */
}