using System;
using System.Threading.Tasks;

namespace rabbitMQ.Producer.RabbitMQ
{
	public interface IMessageProducer
	{
		void SendMessage<T>(T message);
	}
}
