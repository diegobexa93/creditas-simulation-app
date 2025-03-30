using CreditSimulatorService.Domain.Mongo;

namespace CreditSimulatorService.Application.Interfaces
{
    public interface ILoanSimulationRepository
    {
        Task InsertAsync(LoanSimulationDocument loanSimulationDocument, CancellationToken cancellationToken);
    }
}
