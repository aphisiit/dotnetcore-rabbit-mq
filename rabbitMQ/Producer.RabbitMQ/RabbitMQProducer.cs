using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace rabbitMQ.Producer.RabbitMQ
{
	public class RabbitMQProducer: IMessageProducer
	{

		private readonly ILogger _logger;
		private readonly HttpClient httpClient;

		public RabbitMQProducer(ILogger<RabbitMQProducer> logger, HttpClient httpClient)
		{
			_logger = logger;
			this.httpClient = httpClient;
		}

		public void SendMessage<T>(T message)
		{
			string rabbitMQHostName = "localhost";
			string rabbitUsername = "admin";
			string rabbitPassword = "admin";

			var factory = new ConnectionFactory { HostName = rabbitMQHostName, UserName = rabbitUsername, Password = rabbitPassword };
			var connection = factory.CreateConnection();
			using var channel = connection.CreateModel();

			channel.QueueDeclare(queue: "orders", durable: false, exclusive: false, autoDelete: false, arguments: null);

			var json = JsonConvert.SerializeObject(message);
			var body = Encoding.UTF8.GetBytes(json);
			channel.BasicPublish(exchange: "", routingKey: "orders", basicProperties: null, body: body);

			var consumer = new EventingBasicConsumer(channel);
			consumer.Received += (model, eventArgs) =>
			{
				var body = eventArgs.Body.ToArray();
				//Console.WriteLine($"body: {body}");
				var message = Encoding.UTF8.GetString(body);
				//Console.WriteLine($"message: {message}");
				_logger.LogInformation($"message: {message}");
				//httpClient.Post
			};

			channel.BasicConsume(queue: "orders", autoAck: true, consumer: consumer);
			Console.ReadKey();

			//connection.Close();
			//connection.Dispose();
		}

		public async Task<string> ReceviedMessage()
		{
			var stringTask = "";
			try
			{
				stringTask = await httpClient.GetStringAsync("https://api.github.com/orgs/dotnet/reposs");
			} catch (Exception e)
			{
				_logger.LogError($"error - {e.Message}");
			}
			return stringTask;
		}
	}
}
