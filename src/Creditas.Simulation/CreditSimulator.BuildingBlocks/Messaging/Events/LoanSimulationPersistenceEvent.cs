namespace CreditSimulator.BuildingBlocks.Messaging.Events
{
    public class LoanSimulationPersistenceEvent
    {
        public string Email { get; init; }
        public decimal ValueLoan { get; init; } // Valor Empréstimo
        public int PaymentTerm { get; init; } // Prazo de pagamento
        public DateTime BirthDate { get; init; } // Data Nascimento
        public decimal MonthlyInstallment { get; init; } //Prestação mensal
        public decimal TotalToPay { get; init; } // Total Pagar
        public decimal InterestPaid { get; init; } // Juros Pagos
        public Guid BatchId { get; init; } // Id da coleção/lote
        public DateTime SimulatedAt { get; init; } = DateTime.UtcNow;
    }
}
