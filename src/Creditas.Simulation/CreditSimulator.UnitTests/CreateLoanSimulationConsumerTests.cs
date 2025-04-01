using CreditSimulator.BuildingBlocks.Messaging.Events;
using CreditSimulatorService.Worker.Consumer;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace CreditSimulator.UnitTests
{
    public class CreateLoanSimulationConsumerTests
    {
        [Fact]
        public async Task ShouldSimulateAndPublishEmailAndPersistenceEvents()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<CreateLoanSimulationConsumer>>();
            var sendEndpointMock = new Mock<ISendEndpoint>();

            sendEndpointMock
                .Setup(x => x.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var contextMock = new Mock<ConsumeContext<LoanSimulationGenerateEvent>>();
            var batchId = Guid.NewGuid();

            contextMock.Setup(c => c.Message).Returns(new LoanSimulationGenerateEvent
            {
                BatchId = batchId,
                Email = "cliente@teste.com",
                ValueLoan = 10000,
                PaymentTerm = 12,
                BirthDate = DateTime.Today.AddYears(-30)
            });

            contextMock
                .Setup(c => c.GetSendEndpoint(It.IsAny<Uri>()))
                .ReturnsAsync(sendEndpointMock.Object);

            var consumer = new CreateLoanSimulationConsumer(loggerMock.Object);

            // Act
            await consumer.Consume(contextMock.Object);

            // Assert
            sendEndpointMock.Verify(x => x.Send(It.Is<LoanSimulationEmailEvent>(
                e => e.Email == "cliente@teste.com"), It.IsAny<CancellationToken>()), Times.Once);

            sendEndpointMock.Verify(x => x.Send(It.Is<LoanSimulationPersistenceEvent>(
                e => e.Email == "cliente@teste.com" && e.ValueLoan == 10000), It.IsAny<CancellationToken>()), Times.Once);
        }
    }

}
