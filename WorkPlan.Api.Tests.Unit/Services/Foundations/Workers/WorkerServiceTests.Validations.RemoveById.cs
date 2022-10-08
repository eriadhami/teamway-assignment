using Moq;
using WorkPlan.Api.Models.Workers;
using WorkPlan.Api.Models.Workers.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Workers;

public partial class WorkerServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
    {
        // given
        Guid invalidWorkerId = Guid.Empty;

        var invalidWorkerException =
            new InvalidWorkerException();

        invalidWorkerException.AddData(
            key: nameof(Worker.ID),
            values: "Id is required");

        var expectedWorkerValidationException =
            new WorkerValidationException(invalidWorkerException);

        // when
        ValueTask<Worker> removeWorkerByIdTask =
            this.workerService.RemoveWorkerByIdAsync(invalidWorkerId);

        // then
        await Assert.ThrowsAsync<WorkerValidationException>(() =>
            removeWorkerByIdTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedWorkerValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectWorkerByIdAsync(It.IsAny<Guid>()),
                Times.Never);

        this.storageBrokerMock.Verify(broker =>
            broker.DeleteWorkerAsync(It.IsAny<Worker>()),
                Times.Never);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowNotFoundExceptionOnRemoveWorkerByIdIsNotFoundAndLogItAsync()
    {
        // given
        Guid inputWorkerId = Guid.NewGuid();
        Worker noWorker = null;

        var notFoundWorkerException =
            new NotFoundWorkerException(inputWorkerId);

        var expectedWorkerValidationException =
            new WorkerValidationException(notFoundWorkerException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectWorkerByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(noWorker);

        // when
        ValueTask<Worker> removeWorkerByIdTask =
            this.workerService.RemoveWorkerByIdAsync(inputWorkerId);

        // then
        await Assert.ThrowsAsync<WorkerValidationException>(() =>
            removeWorkerByIdTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectWorkerByIdAsync(It.IsAny<Guid>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedWorkerValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.DeleteWorkerAsync(It.IsAny<Worker>()),
                Times.Never);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}