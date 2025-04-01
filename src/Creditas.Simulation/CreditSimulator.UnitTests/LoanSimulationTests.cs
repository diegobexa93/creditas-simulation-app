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

            Assert.True(result.MonthlyInstallment > 0);
            Assert.True(result.TotalToPay > 10000);
            Assert.Equal(result.TotalToPay - 10000, result.InterestPaid);
        }
    }
}
