using BenchmarkDotNet.Attributes;
using CreditSimulatorService.Domain.Entities;

namespace CreditSimulator.Benchmarks
{
    [MemoryDiagnoser]
    public class LoanSimulationBenchmark
    {
        [Benchmark]
        public void ProcessMultipleSimulations()
        {
            for (int i = 0; i < 100000; i++)
            {
                new LoanSimulation(10000, 24, DateTime.Now.AddYears(-20)).Simulate();
            }
        }
    }
}
