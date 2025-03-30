using CreditSimulatorService.Worker.Consumer;
using MassTransit;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<CreateLoanSimulationConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
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
