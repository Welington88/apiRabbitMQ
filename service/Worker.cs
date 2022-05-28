using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using apiRabbitMQ.App;
using apiRabbitMQ.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using apiRabbitMQ.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace service
{

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMqConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672"),
                UserName = "guest",
                Password = "guest",
                /*HostName = "localhost",
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                Port = 5672,
                RequestedHeartbeat = TimeSpan.Parse("60")*/

            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                queue: "messages",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumer = new EventingBasicConsumer(_channel);

                consumer.Received += (sender, eventArgs) =>
                {
                    var contentArray = eventArgs.Body.ToArray();
                    var contentString = Encoding.UTF8.GetString(contentArray);
                    var message = JsonConvert.DeserializeObject<MessageInputModel>(contentString);
                    _logger.LogCritical("Message: {Content}", message.FromId);
                    _logger.LogCritical("Message: {Content}", message.Told);
                    _logger.LogCritical("Message: {Content}", message.Content);
                    //NotifyUser(message); // notificação

                    _channel.BasicAck(eventArgs.DeliveryTag, false);
                };
                _logger.LogInformation("-----------------------------------------------------------------------------");
                _channel.BasicConsume("messages", false, consumer);
                _logger.LogInformation("Message: {Task}", Task.CompletedTask);
                _logger.LogInformation("-----------------------------------------------------------------------------");
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation("-----------------------------------------------------------------------------");
                await Task.Delay(10000, stoppingToken);
            }
        }

        public void NotifyUser(MessageInputModel message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {

                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                notificationService.NotifyUser(message.FromId, message.Told, message.Content);

            }
        }
    }
}