using CreditSimulatorService.Application.Queries;
using FluentValidation;

namespace CreditSimulatorService.Application.Validation
{
    public class GetAllSimulationByEmailQueryValidation : AbstractValidator<GetAllSimulationByEmailQuery>
    {
        public GetAllSimulationByEmailQueryValidation()
        {
            RuleFor(x => x.Email)
               .NotEmpty().WithMessage("O campo 'Email' é obrigatório.");
        }
    }
}
