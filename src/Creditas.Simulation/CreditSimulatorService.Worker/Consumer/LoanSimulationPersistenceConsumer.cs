using CreditSimulator.BuildingBlocks.Messaging.Events;
using CreditSimulatorService.Application.Interfaces;
using CreditSimulatorService.Domain.Mongo;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CreditSimulatorService.Worker.Consumer
{
    public class LoanSimulationPersistenceConsumer : IConsumer<LoanSimulationPersistenceEvent>
    {
        private readonly ILogger<LoanSimulationPersistenceConsumer> _logger;
        private readonly ILoanSimulationRepository _loanSimulationRepository;

        public LoanSimulationPersistenceConsumer(ILogger<LoanSimulationPersistenceConsumer> logger,
                                                ILoanSimulationRepository loanSimulationRepository)
        {
            _logger = logger;
            _loanSimulationRepository = loanSimulationRepository;
        }

        public async Task Consume(ConsumeContext<LoanSimulationPersistenceEvent> context)
        {
            var e = context.Message;
            _logger.LogInformation("[MongoDB] Persistindo simulação de {Email} → Total: {Total}", e.Email, e.TotalToPay);

            var doc = new LoanSimulationDocument(e.Email, e.ValueLoan, e.PaymentTerm,
                                                 e.BirthDate, e.MonthlyInstallment, e.TotalToPay,
                                                 e.InterestPaid, e.BatchId, e.SimulatedAt);

            await _loanSimulationRepository.InsertAsync(doc, context.CancellationToken);

            _logger.LogInformation("[MongoDB] Simulação de {Email} persistida com sucesso", e.Email);
        }
    }
}
