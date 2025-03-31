using AutoMapper;
using CreditSimulatorService.Application.DTOs;
using CreditSimulatorService.Application.Interfaces;
using CreditSimulatorService.Application.Pagination;
using CreditSimulatorService.Application.Queries;
using MediatR;

namespace CreditSimulatorService.Application.Handlers.Queries
{
    public class GetAllSimulationByEmailQueryHandler : IRequestHandler<GetAllSimulationByEmailQuery, PaginatedResult<LoanSimulationResponseDto>>
    {
        private readonly IMapper _mapper;
        private readonly ILoanSimulationRepository _loanSimulationRepository;
        public GetAllSimulationByEmailQueryHandler(IMapper mapper, ILoanSimulationRepository loanSimulationRepository)
        {
            _mapper = mapper;
            _loanSimulationRepository = loanSimulationRepository;
        }

        public async Task<PaginatedResult<LoanSimulationResponseDto>> Handle(GetAllSimulationByEmailQuery request,
                                                                            CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _loanSimulationRepository
                .GetSimulationByEmailPagedAsync(request.Email, request.PageNumber, request.PageSize, cancellationToken);

            var mappedItems = _mapper.Map<IEnumerable<LoanSimulationResponseDto>>(items);

            return new PaginatedResult<LoanSimulationResponseDto>(
                mappedItems,
                totalCount,
                request.PageNumber,
                request.PageSize
            );
        }
    }
}
