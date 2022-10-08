using FluentAssertions;
using Force.DeepCloner;
using Moq;
using WorkPlan.Api.Models.Workers;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Workers;

public partial class WorkerServiceTests
{
    [Fact]
    public async Task ShouldUpdateWorkerAsync()
    {
        // given
        Worker randomWorker = CreateRandomWorker();
        Worker inputWorker = randomWorker;
        Worker storageWorker = inputWorker;
        Worker updatedWorker = inputWorker;
        Worker expectedWorker = updatedWorker.DeepClone();
        Guid inputWorkerId = inputWorker.ID;

        this.storageBrokerMock.Setup(broker =>
            broker.SelectWorkerByIdAsync(inputWorkerId))
                    .ReturnsAsync(storageWorker);

        this.storageBrokerMock.Setup(broker =>
            broker.UpdateWorkerAsync(inputWorker))
                    .ReturnsAsync(updatedWorker);

        // when
        Worker actualWorker =
            await this.workerService.ModifyWorkerAsync(inputWorker);

        // then
        actualWorker.Should().BeEquivalentTo(expectedWorker);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectWorkerByIdAsync(inputWorkerId),
                Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.UpdateWorkerAsync(inputWorker),
                Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}