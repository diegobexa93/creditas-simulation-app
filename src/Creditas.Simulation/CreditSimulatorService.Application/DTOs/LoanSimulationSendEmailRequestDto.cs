namespace CreditSimulatorService.Application.DTOs
{
    public class LoanSimulationSendEmailRequestDto
    {
        public required string Email { get; set; }
        public required decimal ValueLoan { get; set; }
        public required int PaymentTerm { get; set; }
        public required DateTime BirthDate { get; set; }
        
    }
}
