namespace CreditSimulator.BuildingBlocks.Messaging.Events
{
    public class LoanSimulationEmailEvent
    {
        public Guid BatchId { get; init; }
        public string Email { get; init; }
        public decimal MonthlyInstallment { get; init; }
        public decimal TotalToPay { get; init; }
        public decimal InterestPaid { get; init; }
    }
}
