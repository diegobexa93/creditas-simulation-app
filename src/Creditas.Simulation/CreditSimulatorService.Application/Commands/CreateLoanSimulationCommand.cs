using System.Text.Json.Serialization;

namespace CreditSimulatorService.Application.Commands
{
    public class CreateLoanSimulationCommand
    {
        [JsonIgnore]
        public Guid BatchId { get; set; }

        public required decimal ValueLoan { get; set; }
        public required int PaymentTerm { get; set; }
        public required DateTime BirthDate { get; set; }
        public required string Email { get; set; }

    }
}
