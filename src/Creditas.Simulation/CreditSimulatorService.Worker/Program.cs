﻿using CreditSimulator.BuildingBlocks.Messaging;
using CreditSimulatorService.Application;
using CreditSimulatorService.Infrastructure;
using CreditSimulatorService.Worker.Consumer;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

//Carregamento das configurações
builder.Services.AddApplicationRegistration(builder.Configuration);
builder.Services.AddInfrastructureRegistration(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateLoanSimulationConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMQSettings = context.GetRequiredService<IOptions<RabbitMqConfiguration>>().Value;

        cfg.Host(new Uri($"rabbitmq://{rabbitMQSettings.HostName}:{rabbitMQSettings.Port}"), h =>
        {
            h.Username(rabbitMQSettings.UserName);
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
