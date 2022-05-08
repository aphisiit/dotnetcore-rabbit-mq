using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using rabbitMQ.Producer.RabbitMQ;

namespace rabbitMQ
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{

			services.AddControllers();
			services.AddScoped<IMessageProducer, RabbitMQProducer>();
			services.AddHttpClient<IMessageProducer, RabbitMQProducer>()
				.SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
				.AddPolicyHandler(GetRetryPolicy());
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}

		static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
		{
			return HttpPolicyExtensions
				.HandleTransientHttpError()
				.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
				.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.Forbidden)
				.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.InternalServerError)
				.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.GatewayTimeout)
				.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.BadGateway)
				.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(10));
		}
	}
}
