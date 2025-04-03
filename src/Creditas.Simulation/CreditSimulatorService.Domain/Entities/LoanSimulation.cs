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
            //Calcula a Idade com base da data de aniversário
            int age = CalculateAge(BirthDate);

            //Retorna a taxa de juros anual com base na idade
            decimal annualRate = GetAnnualInterestRate(age);

            //Taxa de juros mensal (dividimos por 100 precisamos converter para decimal para usar em fórmulas matemáticas)
            double monthlyRate = (double)(annualRate / 100m / 12m);

            //Número de pagamentos (meses)
            int n = PaymentTerm;

            //Valor do Emprestimo
            double pv = (double)ValueLoan;

            //Cálculo da parcela mensal pmt
            // pmt = pv * i / (1 - (1 + i)^-n)
            double factor = Math.Pow(1 + monthlyRate, -n);
            double pmt = pv * monthlyRate / (1 - factor);

            //Total a pagar
            double totalPaid = pmt * n;

            //Total de juros
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
        private static decimal GetAnnualInterestRate(int age) => age switch
        {
            <= 25 => 5.0m,
            <= 40 => 3.0m,
            <= 60 => 2.0m,
            _ => 4.0m
        };
    }

}
