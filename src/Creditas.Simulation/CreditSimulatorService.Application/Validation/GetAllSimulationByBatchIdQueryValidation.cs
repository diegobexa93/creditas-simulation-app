using CreditSimulatorService.Application.Queries;
using FluentValidation;

namespace CreditSimulatorService.Application.Validation
{
    public class GetAllSimulationByBatchIdQueryValidation : AbstractValidator<GetAllSimulationByBatchIdQuery>
    {
        public GetAllSimulationByBatchIdQueryValidation()
        {
            RuleFor(x => x.BatchId)
                .NotEqual(Guid.Empty)
                .WithMessage("O campo 'BatchId' é obrigatório.");
        }
    }
}
