using CreditSimulator.BuildingBlocks.Messaging;
using CreditSimulatorService.Application;
using CreditSimulatorService.Infrastructure;
using MassTransit;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Carregamento das configurações
builder.Services.AddApplicationRegistration(builder.Configuration);
builder.Services.AddInfrastructureRegistration(builder.Configuration);
builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMQ"));

// Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Credit Simulator API",
        Version = "v1",
        Description = "Credit Simulator API",
    });
});

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMQSettings = context.GetRequiredService<IOptions<RabbitMqConfiguration>>().Value;

        cfg.Host(new Uri($"rabbitmq://{rabbitMQSettings.Host}:{rabbitMQSettings.Port}"), h =>
        {
            h.Username(rabbitMQSettings.Username);
            h.Password(rabbitMQSettings.Password);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Credit Simulator API v1");
        options.RoutePrefix = ""; // Deixa a UI do Swagger na raiz (http://localhost:5000/)
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
