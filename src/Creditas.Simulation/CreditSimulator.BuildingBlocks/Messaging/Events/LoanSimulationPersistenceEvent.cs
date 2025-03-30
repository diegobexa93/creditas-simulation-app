namespace CreditSimulator.BuildingBlocks.Messaging.Events
{
    public class LoanSimulationPersistenceEvent
    {
        public string Email { get; init; }
        public decimal ValueLoan { get; init; }
        public int PaymentTerm { get; init; }
        public DateTime BirthDate { get; init; }
        public decimal MonthlyInstallment { get; init; }
        public decimal TotalToPay { get; init; }
        public decimal InterestPaid { get; init; }
        public Guid BatchId { get; init; }
        public DateTime SimulatedAt { get; init; } = DateTime.UtcNow;
    }
}
