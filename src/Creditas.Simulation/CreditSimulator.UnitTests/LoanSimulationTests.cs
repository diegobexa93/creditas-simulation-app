using CreditSimulatorService.Domain.Entities;

namespace CreditSimulator.UnitTests
{
    public class LoanSimulationTests
    {
        [Fact]
        public void Simulate_ShouldReturnExpectedValues()
        {
            var simulation = new LoanSimulation(10000m, 12, DateTime.Today.AddYears(-30));

            var result = simulation.Simulate();

            Assert.True(result.MonthlyInstallment.Equals(846.94m));
            Assert.True(result.TotalToPay.Equals(10163.24m));
            Assert.Equal(result.TotalToPay - 10000m, result.InterestPaid);
        }
    }
}
