using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using rabbitMQ.Producer.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddControllers();
builder.Services.AddHostedService<RabbitMQConsumer>();
builder.Services.AddScoped<IMessageProducer, RabbitMQProducer>();
builder.Services.AddHttpClient<IMessageProducer, RabbitMQProducer>()
	.SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
	.AddPolicyHandler(GetRetryPolicy());

var app = builder.Build();

app.UseStatusCodePages();
app.UseExceptionHandler();

if(app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();

IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
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