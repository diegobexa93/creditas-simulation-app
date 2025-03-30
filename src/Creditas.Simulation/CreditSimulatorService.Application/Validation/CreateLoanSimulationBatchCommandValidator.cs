using CreditSimulatorService.Application.Commands;
using FluentValidation;

namespace CreditSimulatorService.Application.Validation
{
    public class CreateLoanSimulationBatchCommandValidator : AbstractValidator<CreateLoanSimulationSendEmail>
    {
        public CreateLoanSimulationBatchCommandValidator()
        {
            RuleFor(x => x.ValueLoan)
                .GreaterThan(0).WithMessage("O valor do empréstimo deve ser maior que zero.");

            RuleFor(x => x.PaymentTerm)
                .InclusiveBetween(1, 360).WithMessage("O prazo deve estar entre 1 e 360 meses.");

            RuleFor(x => x.BirthDate)
                .LessThan(DateTime.Today).WithMessage("A data de nascimento deve ser anterior à data atual.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("Formato de e-mail inválido.");
        }
    }
}
