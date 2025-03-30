using CreditSimulatorService.Application.Commands;

namespace CreditSimulatorService.Application.Messaging
{
    public interface IMessageQueuePublisher
    {
        Task PublishAsync(CreateLoanSimulationCommand command, CancellationToken cancellationToken = default);
    }
}
