using CreditSimulator.BuildingBlocks.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CreditSimulatorService.Worker.Consumer
{
    public class LoanSimulationEmailConsumer : IConsumer<LoanSimulationEmailEvent>
    {
        private readonly ILogger<LoanSimulationEmailConsumer> _logger;

        public LoanSimulationEmailConsumer(ILogger<LoanSimulationEmailConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<LoanSimulationEmailEvent> context)
        {
            var e = context.Message;
            _logger.LogInformation("[Email] Enviar simulação para {Email}: Total {Total}", e.Email, e.TotalToPay);

            // TODO: enviar email
            return Task.CompletedTask;
        }
    }
}
