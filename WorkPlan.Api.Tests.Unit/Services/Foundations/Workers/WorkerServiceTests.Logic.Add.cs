using FluentAssertions;
using Force.DeepCloner;
using Moq;
using WorkPlan.Api.Models.Workers;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Workers;

public partial class WorkerServiceTests
{
    [Fact]
    public async Task ShouldAddWorkerAsync()
    {
        // given
        DateTimeOffset dateTime = GetRandomDateTimeOffset();
        Worker randomWorker = CreateRandomWorker(dateTime);
        Worker inputWorker = randomWorker;
        Worker storageWorker = inputWorker;
        Worker expectedWorker = storageWorker.DeepClone();

        this.dateTimeBrokerMock.Setup(broker =>
            broker.GetCurrentDateTimeOffset())
                .Returns(dateTime);

        this.storageBrokerMock.Setup(broker =>
            broker.InsertWorkerAsync(inputWorker))
                .ReturnsAsync(storageWorker);

        // when
        Worker actualWorker = await this.workerService
            .AddWorkerAsync(inputWorker);

        // then
        actualWorker.Should().BeEquivalentTo(expectedWorker);

        this.dateTimeBrokerMock.Verify(broker =>
            broker.GetCurrentDateTimeOffset(),
                Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertWorkerAsync(inputWorker),
                Times.Once);

        this.dateTimeBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}