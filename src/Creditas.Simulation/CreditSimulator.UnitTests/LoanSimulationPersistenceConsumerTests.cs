using CreditSimulator.BuildingBlocks.Messaging.Events;
using CreditSimulatorService.Application.Interfaces;
using CreditSimulatorService.Domain.Mongo;
using CreditSimulatorService.Worker.Consumer;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace CreditSimulator.UnitTests
{
    public class LoanSimulationPersistenceConsumerTests
    {
        [Fact]
        public async Task ShouldPersistSimulationInRepository()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<LoanSimulationPersistenceConsumer>>();
            var repoMock = new Mock<ILoanSimulationRepository>();

            var evt = new LoanSimulationPersistenceEvent
            {
                Email = "cliente@teste.com",
                ValueLoan = 10000,
                PaymentTerm = 12,
                BirthDate = DateTime.Today.AddYears(-30),
                MonthlyInstallment = 859.20m,
                TotalToPay = 10310.40m,
                InterestPaid = 310.40m,
                BatchId = Guid.NewGuid(),
                SimulatedAt = DateTime.UtcNow
            };

            var contextMock = new Mock<ConsumeContext<LoanSimulationPersistenceEvent>>();
            contextMock.Setup(x => x.Message).Returns(evt);
            contextMock.Setup(x => x.CancellationToken).Returns(CancellationToken.None);

            var consumer = new LoanSimulationPersistenceConsumer(loggerMock.Object, repoMock.Object);

            // Act
            await consumer.Consume(contextMock.Object);

            // Assert
            repoMock.Verify(repo =>
                repo.InsertAsync(It.Is<LoanSimulationDocument>(doc =>
                    doc.Email == evt.Email &&
                    doc.ValueLoan == evt.ValueLoan &&
                    doc.TotalToPay == evt.TotalToPay &&
                    doc.BatchId == evt.BatchId),
                    CancellationToken.None),
                Times.Once);
        }
    }

}
