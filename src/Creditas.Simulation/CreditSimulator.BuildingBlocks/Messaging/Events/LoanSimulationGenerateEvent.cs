namespace CreditSimulator.BuildingBlocks.Messaging.Events
{
    public class LoanSimulationGenerateEvent
    {
        public Guid BatchId { get; set; }
        public string Email { get; set; }
        public decimal ValueLoan { get; set; }
        public int PaymentTerm { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
