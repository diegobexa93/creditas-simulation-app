using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CreditSimulatorService.Domain.Mongo
{
    public class LoanSimulationDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; private set; }

        public string Email { get; private set; }
        public decimal ValueLoan { get; private set; }
        public int PaymentTerm { get; private set; }
        public DateTime BirthDate { get; private set; }
        public decimal MonthlyInstallment { get; private set; }
        public decimal TotalToPay { get; private set; }
        public decimal InterestPaid { get; private set; }
        public Guid BatchId { get; private set; }
        public DateTime SimulatedAt { get; private set; }

        private LoanSimulationDocument() { }

        public LoanSimulationDocument(
            string email,
            decimal valueLoan,
            int paymentTerm,
            DateTime birthDate,
            decimal monthlyInstallment,
            decimal totalToPay,
            decimal interestPaid,
            Guid batchId,
            DateTime simulatedAt)
        {
            Email = email;
            ValueLoan = valueLoan;
            PaymentTerm = paymentTerm;
            BirthDate = birthDate;
            MonthlyInstallment = monthlyInstallment;
            TotalToPay = totalToPay;
            InterestPaid = interestPaid;
            BatchId = batchId;
            SimulatedAt = simulatedAt;
        }
    }
}
