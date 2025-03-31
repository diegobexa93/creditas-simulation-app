using CreditSimulatorService.Application.DTOs;
using CreditSimulatorService.Application.Pagination;
using MediatR;

namespace CreditSimulatorService.Application.Queries
{
    public class GetAllSimulationByEmailQuery : IRequest<PaginatedResult<LoanSimulationResponseDto>>
    {
        public string Email { get; set; }
        public int PageNumber { get; set; } = 1; // Default
        public int PageSize { get; set; } = 19;  // Default
    }
}
