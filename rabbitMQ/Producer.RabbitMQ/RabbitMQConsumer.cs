using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbitMQ.Producer.RabbitMQ
{
	public class RabbitMQConsumer: BackgroundService
	{

		private readonly ILogger _logger;
		private IConnection _connection;
        private IModel _channel;

		public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger)
		{
			_logger = logger;
			string rabbitMQHostName = "rabbitmq3";
			string rabbitUsername = "admin";
			string rabbitPassword = "admin";

			var factory = new ConnectionFactory { HostName = rabbitMQHostName, UserName = rabbitUsername, Password = rabbitPassword };
			_connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "orders", durable: false, exclusive: false, autoDelete: false, arguments: null);
		}

 		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{			
			_logger.LogInformation($"[*] Waiting for messages.");

			var consumer = new EventingBasicConsumer(_channel);
			consumer.Received += (model, eventArgs) =>
			{
				var body = eventArgs.Body.ToArray();
				// Console.WriteLine($"body: {body}");
				var message = Encoding.UTF8.GetString(body);
				// Console.WriteLine($"message: {message}");
				_logger.LogInformation($"message: {message}");
				//httpClient.Post
			};

			_channel.BasicConsume(queue: "orders", autoAck: true, consumer: consumer);

			return Task.CompletedTask;			
		}

		public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }

    }
}
