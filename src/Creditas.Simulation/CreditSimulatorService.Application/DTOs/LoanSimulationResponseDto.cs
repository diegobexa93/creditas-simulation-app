namespace CreditSimulatorService.Application.DTOs
{
    public class LoanSimulationResponseDto
    {
        public string Email { get; set; } = default!;
        public decimal ValueLoan { get; set; }
        public int PaymentTerm { get; set; }
        public DateTime BirthDate { get; set; }
        public decimal MonthlyInstallment { get; set; }
        public decimal TotalToPay { get; set; }
        public decimal InterestPaid { get; set; }
        public DateTime SimulatedAt { get; set; }
    }
}
