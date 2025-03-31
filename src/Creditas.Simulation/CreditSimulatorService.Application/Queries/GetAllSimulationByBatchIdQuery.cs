using CreditSimulatorService.Application.DTOs;
using CreditSimulatorService.Application.Pagination;
using MediatR;

namespace CreditSimulatorService.Application.Queries
{
    public class GetAllSimulationByBatchIdQuery : IRequest<PaginatedResult<LoanSimulationResponseDto>>
    {
        public Guid BatchId { get; set; }
        public int PageNumber { get; set; } = 1; // Default
        public int PageSize { get; set; } = 19;  // Default
    }
}
