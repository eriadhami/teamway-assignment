using FluentAssertions;
using Force.DeepCloner;
using Moq;
using WorkPlan.Api.Models.Workers;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Workers;

public partial class WorkerServiceTests
{
    [Fact]
    public async void ShouldRetrieveWorkerByIdAsync()
    {
        //given
        Worker someWorker = CreateRandomWorker();
        Worker storageWorker = someWorker;
        Worker expectedWorker = storageWorker.DeepClone();

        this.storageBrokerMock.Setup(broker =>
            broker.SelectWorkerByIdAsync(someWorker.ID))
                .ReturnsAsync(storageWorker);

        //when
        Worker actualWorker =
            await this.workerService.RetrieveWorkerByIdAsync(someWorker.ID);

        //then
        actualWorker.Should().BeEquivalentTo(expectedWorker);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectWorkerByIdAsync(someWorker.ID),
                Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}