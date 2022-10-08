using FluentAssertions;
using Force.DeepCloner;
using Moq;
using WorkPlan.Api.Models.Workers;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Workers;

public partial class WorkerServiceTests
{

    [Fact]
    public async void ShouldRemoveWorkerByIdAsync()
    {
        // given
        Guid randomId = Guid.NewGuid();
        Guid inputWorkerId = randomId;
        Worker randomWorker = CreateRandomWorker();
        Worker storageWorker = randomWorker;
        Worker expectedInputWorker = storageWorker;
        Worker deletedWorker = expectedInputWorker;
        Worker expectedWorker = deletedWorker.DeepClone();

        this.storageBrokerMock.Setup(broker =>
            broker.SelectWorkerByIdAsync(inputWorkerId))
                .ReturnsAsync(storageWorker);

        this.storageBrokerMock.Setup(broker =>
            broker.DeleteWorkerAsync(expectedInputWorker))
                .ReturnsAsync(deletedWorker);

        // when
        Worker actualWorker = await this.workerService
            .RemoveWorkerByIdAsync(inputWorkerId);

        // then
        actualWorker.Should().BeEquivalentTo(expectedWorker);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectWorkerByIdAsync(inputWorkerId),
                Times.Once());

        this.storageBrokerMock.Verify(broker =>
            broker.DeleteWorkerAsync(expectedInputWorker),
                Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}