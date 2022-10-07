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
        Worker randomWorker = CreateRandomWorker();
        Worker inputWorker = randomWorker;
        Worker storageWorker = inputWorker;
        Worker expectedWorker = storageWorker.DeepClone();

        this.storageBrokerMock.Setup(broker =>
            broker.InsertWorkerAsync(inputWorker))
                .ReturnsAsync(storageWorker);

        // when
        Worker actualWorker = await this.workerService
            .AddWorkerAsync(inputWorker);

        // then
        actualWorker.Should().BeEquivalentTo(expectedWorker);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertWorkerAsync(inputWorker),
                Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}