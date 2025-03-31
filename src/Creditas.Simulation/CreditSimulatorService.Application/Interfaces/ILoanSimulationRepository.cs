using CreditSimulatorService.Domain.Mongo;

namespace CreditSimulatorService.Application.Interfaces
{
    public interface ILoanSimulationRepository
    {
        Task InsertAsync(LoanSimulationDocument loanSimulationDocument, CancellationToken cancellationToken);
        Task<(IEnumerable<LoanSimulationDocument> Items, int TotalCount)> GetSimulationByBatchIdPagedAsync(Guid batchId, int pageNumber,
            int pageSize, CancellationToken cancellationToken);
        Task<(IEnumerable<LoanSimulationDocument> Items, int TotalCount)> GetSimulationByEmailPagedAsync(string email, int pageNumber,
            int pageSize, CancellationToken cancellationToken);
    }
}
