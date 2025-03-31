using CreditSimulatorService.Application.DTOs;
using MediatR;

namespace CreditSimulatorService.Application.Commands
{
    public class CreateLoanSimulationBatchCommand() : IRequest<Guid>
    {
        public required List<LoanSimulationSendEmailRequestDto> Simulations { get; set; }

    }
}
