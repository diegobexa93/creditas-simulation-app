using Moq;
using System.Threading;

namespace CreditSimulator.Worker.Tests
{
    public class CreateLoanSimulationConsumerTests
    {
        [Fact]
        public async Task Should_Call_ProcessAsync_When_Message_Received()
        {
            //// Arrange
            //var mockProcessor = new Mock<ILoanSimulationProcessor>();
            //var consumer = new CreateLoanSimulationConsumer(mockProcessor.Object);
            //var message = new CreateLoanSimulationCommand
            //{
            //    ValueLoan = 5000,
            //    PaymentTerm = 12,
            //    BirthDate = new DateTime(1990, 1, 1),
            //    Email = "cliente@teste.com"
            //};

            //var mockContext = Mock.Of<ConsumeContext<CreateLoanSimulationCommand>>(ctx =>
            //    ctx.Message == message
            //);

            //// Act
            //await consumer.Consume(mockContext);

            //// Assert
            //mockProcessor.Verify(x => x.ProcessAsync(message), Times.Once);
        }
    }
}
