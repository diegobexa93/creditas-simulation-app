using CreditSimulator.BuildingBlocks.Contractors;
using MediatR;

namespace CreditSimulatorService.Application.Commands
{
    public record CreateLoanSimulationBatchCommand(List<CreateLoanSimulationCommand> Simulations) : IRequest<Guid>
    {
        public Guid BatchId { get; init; } = Guid.NewGuid();
    }
}
