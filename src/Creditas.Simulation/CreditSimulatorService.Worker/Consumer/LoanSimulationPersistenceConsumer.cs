using CreditSimulator.BuildingBlocks.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CreditSimulatorService.Worker.Consumer
{
    public class LoanSimulationPersistenceConsumer : IConsumer<LoanSimulationPersistenceEvent>
    {
        private readonly ILogger<LoanSimulationPersistenceConsumer> _logger;

        public LoanSimulationPersistenceConsumer(ILogger<LoanSimulationPersistenceConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<LoanSimulationPersistenceEvent> context)
        {
            var e = context.Message;
            _logger.LogInformation("[MongoDB] Persistindo simulação de {Email} → Total: {Total}", e.Email, e.TotalToPay);

            // TODO: persistir no MongoDB
            return Task.CompletedTask;
        }
    }
}
