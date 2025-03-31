using CreditSimulator.BuildingBlocks.Messaging;
using CreditSimulatorService.Infrastructure;
using CreditSimulatorService.Worker.Consumer;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

//Carregamento das configurações
builder.Services.AddInfrastructureRegistration(builder.Configuration);
builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateLoanSimulationConsumer>();
    x.AddConsumer<LoanSimulationEmailConsumer>();
    x.AddConsumer<LoanSimulationPersistenceConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMQSettings = context.GetRequiredService<IOptions<RabbitMqConfiguration>>().Value;

        cfg.Host(new Uri($"rabbitmq://{rabbitMQSettings.Host}:{rabbitMQSettings.Port}"), h =>
        {
            h.Username(rabbitMQSettings.Username);
            h.Password(rabbitMQSettings.Password);
        });

        cfg.ReceiveEndpoint("loan_simulations", e =>
        {
            e.ConfigureConsumer<CreateLoanSimulationConsumer>(context);
        });

        cfg.ReceiveEndpoint("loan_simulation_email", e =>
        {
            e.ConfigureConsumer<LoanSimulationEmailConsumer>(context);
        });

        cfg.ReceiveEndpoint("loan_simulation_persistence", e =>
        {
            e.ConfigureConsumer<LoanSimulationPersistenceConsumer>(context);
        });
    });
});


var host = builder.Build();
host.Run();
