using System;
using Microsoft.EntityFrameworkCore;

namespace rabbitMQ.Models
{
	public class OrderDbContext: DbContext
	{
		public DbSet<Order> Order { get; set; }

		public OrderDbContext()
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder options)
			=> options.UseSqlite($"Data Source=/opt/rabbitMQ/sqliteDB/Order.db");
	}
}
