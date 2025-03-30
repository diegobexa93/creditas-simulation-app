using CreditSimulator.BuildingBlocks.Contractors;
using CreditSimulator.BuildingBlocks.Messaging.Events;
using CreditSimulatorService.Domain.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CreditSimulatorService.Worker.Consumer
{
    public class CreateLoanSimulationConsumer : IConsumer<CreateLoanSimulationCommand>
    {
        private readonly ILogger<CreateLoanSimulationConsumer> _logger;

        public CreateLoanSimulationConsumer(ILogger<CreateLoanSimulationConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CreateLoanSimulationCommand> context)
        {
            var message = context.Message;

            var simulation = new LoanSimulation(message.ValueLoan, message.PaymentTerm, message.BirthDate);
            var result = simulation.Simulate();

            _logger.LogInformation("Simulação para {Email} → Parcela: {Parcela} | Total: {Total}",
                message.Email, result.MonthlyInstallment, result.TotalToPay);

            // Publicar evento para envio de email
            var sendEndpoint = await context.GetSendEndpoint(new Uri("queue:loan_simulation_email"));

            await sendEndpoint.Send(new LoanSimulationEmailEvent
            {
                Email = message.Email,
                MonthlyInstallment = result.MonthlyInstallment,
                TotalToPay = result.TotalToPay,
                InterestPaid = result.InterestPaid,
                BatchId = message.BatchId
            });

            // Publicar evento para persistencia
            var persistEndpoint = await context.GetSendEndpoint(new Uri("queue:loan_simulation_persistence"));

            await persistEndpoint.Send(new LoanSimulationPersistenceEvent
            {
                Email = message.Email,
                ValueLoan = message.ValueLoan,
                PaymentTerm = message.PaymentTerm,
                BirthDate = message.BirthDate,
                MonthlyInstallment = result.MonthlyInstallment,
                TotalToPay = result.TotalToPay,
                InterestPaid = result.InterestPaid,
                BatchId = message.BatchId
            });
        }
    }
}
