namespace CreditSimulatorService.Domain.Entities
{
    public record LoanResult(decimal MonthlyInstallment, decimal TotalToPay, decimal InterestPaid, decimal AnnualRate);

}
