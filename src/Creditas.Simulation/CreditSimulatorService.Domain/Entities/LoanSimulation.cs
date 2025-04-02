namespace CreditSimulatorService.Domain.Entities
{
    public sealed class LoanSimulation
    {
        public decimal ValueLoan { get; }
        public int PaymentTerm { get; }
        public DateTime BirthDate { get; }

        public LoanSimulation(decimal valueLoan, int paymentTerm, DateTime birthDate)
        {
            if (valueLoan <= 0)
                ThrowArgument(nameof(valueLoan), "Valor do empréstimo deve ser maior que zero.");
            if (paymentTerm <= 0)
                ThrowArgument(nameof(paymentTerm), "Prazo de pagamento deve ser maior que zero.");
            if (birthDate > DateTime.Today)
                ThrowArgument(nameof(birthDate), "Data de nascimento inválida.");

            ValueLoan = valueLoan;
            PaymentTerm = paymentTerm;
            BirthDate = birthDate;
        }

        private static void ThrowArgument(string name, string message) =>
            throw new ArgumentException(message, name);

        public LoanResult Simulate()
        {
            int age = CalculateAge(BirthDate);
            decimal annualRate = GetAnnualInterestRate(age);
            double monthlyRate = (double)(annualRate / 100m / 12m);

            int n = PaymentTerm;
            double pv = (double)ValueLoan;

            // pmt = pv * i / (1 - (1 + i)^-n)
            double factor = Math.Pow(1 + monthlyRate, -n);
            double pmt = pv * monthlyRate / (1 - factor);

            double totalPaid = pmt * n;
            double interestPaid = totalPaid - pv;

            return new LoanResult(
                MonthlyInstallment: Math.Round((decimal)pmt, 2),
                TotalToPay: Math.Round((decimal)totalPaid, 2),
                InterestPaid: Math.Round((decimal)interestPaid, 2),
                AnnualRate: annualRate
            );
        }

        private static int CalculateAge(DateTime birthDate)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var birth = DateOnly.FromDateTime(birthDate);
            int age = today.Year - birth.Year;
            if (birth > today.AddYears(-age)) age--;
            return age;
        }

        private static decimal GetAnnualInterestRate(int age) => age switch
        {
            <= 25 => 5.0m,
            <= 40 => 3.0m,
            <= 60 => 2.0m,
            _ => 4.0m
        };
    }

}
