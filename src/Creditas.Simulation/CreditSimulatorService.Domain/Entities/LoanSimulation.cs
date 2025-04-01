namespace CreditSimulatorService.Domain.Entities
{
    public class LoanSimulation
    {
        public decimal ValueLoan { get; private set; }
        public int PaymentTerm { get; private set; }
        public DateTime BirthDate { get; private set; }

        public LoanSimulation(decimal valueLoan, int paymentTerm, DateTime birthDate)
        {
            if (valueLoan <= 0)
                throw new ArgumentException("Valor do empréstimo deve ser maior que zero.");
            if (paymentTerm <= 0)
                throw new ArgumentException("Prazo de pagamento deve ser maior que zero.");
            if (birthDate > DateTime.Today)
                throw new ArgumentException("Data de nascimento inválida.");

            ValueLoan = valueLoan;
            PaymentTerm = paymentTerm;
            BirthDate = birthDate;
        }


        /// <summary>
        /// Realiza a simulação do empréstimo com base no valor solicitado, prazo e idade do cliente.
        /// </summary>
        /// <returns>
        /// Um <see cref="LoanResult"/> contendo:
        /// - O valor da parcela mensal.
        /// - O valor total a ser pago ao final do período.
        /// - O total de juros pagos.
        /// - A taxa de juros anual aplicada com base na idade.
        /// </returns>
        /// <remarks>
        /// A taxa de juros anual varia conforme a idade do cliente:
        /// - Até 25 anos: 5%
        /// - De 26 a 40 anos: 3%
        /// - De 41 a 60 anos: 2%
        /// - Acima de 60 anos: 4%
        /// </remarks>
        public LoanResult Simulate()
        {
            var age = CalculateAge(BirthDate);
            var annualRate = GetAnnualInterestRate(age);
            var monthlyRate = (double)(annualRate / 100 / 12);
            var n = PaymentTerm;
            var pv = (double)ValueLoan;

            var factor = Math.Pow(1 + monthlyRate, -n);
            var pmt = pv * monthlyRate / (1 - factor);

            var totalPaid = pmt * n;
            var interestPaid = totalPaid - pv;

            return new LoanResult(
                MonthlyInstallment: Math.Round((decimal)pmt, 2),
                TotalToPay: Math.Round((decimal)totalPaid, 2),
                InterestPaid: Math.Round((decimal)interestPaid, 2),
                AnnualRate: annualRate
            );
        }


        /// <summary>
        /// Retorna a taxa de juros anual com base na idade do cliente.
        /// </summary>
        /// <param name="age">Idade do cliente em anos.</param>
        /// <returns>Taxa de juros anual correspondente à faixa etária.</returns>
        /// <remarks>
        /// Regras de cálculo:
        /// - Até 25 anos: 5% ao ano
        /// - De 26 a 40 anos: 3% ao ano
        /// - De 41 a 60 anos: 2% ao ano
        /// - Acima de 60 anos: 4% ao ano
        /// </remarks>
        private static decimal GetAnnualInterestRate(int age)
        {
            return age switch
            {
                <= 25 => 5.0m,
                <= 40 => 3.0m,
                <= 60 => 2.0m,
                _ => 4.0m
            };
        }

        private static int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

}
