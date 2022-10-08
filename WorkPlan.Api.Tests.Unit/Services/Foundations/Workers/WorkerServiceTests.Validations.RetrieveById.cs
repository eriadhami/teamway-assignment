using Moq;
using WorkPlan.Api.Models.Workers;
using WorkPlan.Api.Models.Workers.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Workers;

public partial class WorkerServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
    {
        //given
        Guid invalidWorkerId = Guid.Empty;

        var invalidWorkerException =
            new InvalidWorkerException();

        invalidWorkerException.AddData(
            key: nameof(Worker.ID),
            values: "Id is required");

        var expectedWorkerValidationException =
            new WorkerValidationException(invalidWorkerException);

        //when
        ValueTask<Worker> retrieveWorkerByIdTask =
            this.workerService.RetrieveWorkerByIdAsync(invalidWorkerId);

        //then
        await Assert.ThrowsAsync<WorkerValidationException>(() =>
            retrieveWorkerByIdTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedWorkerValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectWorkerByIdAsync(It.IsAny<Guid>()),
                Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }
}