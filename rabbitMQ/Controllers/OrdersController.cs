using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using rabbitMQ.DTO;
using rabbitMQ.Models;
using rabbitMQ.Producer.RabbitMQ;

namespace rabbitMQ.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class OrdersController: ControllerBase
	{
		private readonly OrderDbContext _context;
		private readonly IMessageProducer _messagePublisher;
		
		public OrdersController(IMessageProducer messagePublisher)
		{
			_context = new OrderDbContext();
			_messagePublisher = messagePublisher;
		}

		[HttpPost]
		public async Task<IActionResult> CreateOrder(OrderDto orderDto)
		{
			Order order = new()
			{
				ProductName = orderDto.ProductName,
				Price = orderDto.Price,
				Quantity = orderDto.Quantity
			};
			
			_context.Order.Add(order);
			await _context.SaveChangesAsync();
			_messagePublisher.SendMessage(order);
			return Ok(new { id = order.Id });
		}		
	}
}
