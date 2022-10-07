using FluentAssertions;
using Force.DeepCloner;
using Moq;
using WorkPlan.Api.Models.Workers;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Workers;

public partial class WorkerServiceTests
{
    [Fact]
    public void ShouldRetrieveAllWorkers()
    {
        // given
        IQueryable<Worker> randomWorkers = CreateRandomWorkers();
        IQueryable<Worker> storageWorkers = randomWorkers;
        IQueryable<Worker> expectedWorkers = storageWorkers;

        this.storageBrokerMock.Setup(broker =>
            broker.SelectAllWorkers())
                .Returns(storageWorkers);

        // when
        IQueryable<Worker> actualWorkers =
            this.workerService.RetrieveAllWorkers();

        // then
        actualWorkers.Should().BeEquivalentTo(expectedWorkers);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllWorkers(),
                Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}