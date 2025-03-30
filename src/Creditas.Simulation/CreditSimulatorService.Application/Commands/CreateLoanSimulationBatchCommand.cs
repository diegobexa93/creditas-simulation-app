using MediatR;

namespace CreditSimulatorService.Application.Commands
{
    public class CreateLoanSimulationBatchCommand() : IRequest<Guid>
    {
        public required List<CreateLoanSimulationSendEmail> Simulations { get; set; }

    }
}
