using Moq;
using WorkPlan.Api.Models.Workers;
using WorkPlan.Api.Models.Workers.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Workers;

public partial class WorkerServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationExceptionOnModifyIfWorkerIsNullAndLogItAsync()
    {
        // given
        Worker invalidWorker = null;

        var nullWorkerException =
            new NullWorkerException();

        var expectedWorkerValidationException =
            new WorkerValidationException(nullWorkerException);

        // when
        ValueTask<Worker> addWorkerTask =
            this.workerService.ModifyWorkerAsync(invalidWorker);

        // then
        await Assert.ThrowsAsync<WorkerValidationException>(() =>
            addWorkerTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedWorkerValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectWorkerByIdAsync(It.IsAny<Guid>()),
                Times.Never);

        this.storageBrokerMock.Verify(broker =>
            broker.UpdateWorkerAsync(It.IsAny<Worker>()),
                Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ShouldThrowValidationExceptionOnUpdateIfWorkerIsInvalidAndLogItAsync(
        string invalidText)
    {
        // given
        var invalidWorker = new Worker
        {
            FirstName = invalidText,
            LastName = invalidText
        };

        var invalidWorkerException =
            new InvalidWorkerException();

        invalidWorkerException.AddData(
            key: nameof(Worker.ID),
            values: "Id is required");

        invalidWorkerException.AddData(
            key: nameof(Worker.FirstName),
            values: "Text is required");

        invalidWorkerException.AddData(
            key: nameof(Worker.LastName),
            values: "Text is required");

        var expectedWorkerValidationException =
            new WorkerValidationException(invalidWorkerException);

        // when
        ValueTask<Worker> updateWorkerTask =
            this.workerService.ModifyWorkerAsync(invalidWorker);

        // then
        await Assert.ThrowsAsync<WorkerValidationException>(() =>
            updateWorkerTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedWorkerValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.UpdateWorkerAsync(invalidWorker),
                Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionOnModifyIfWorkerDoesNotExistAndLogItAsync()
    {
        // given
        Worker randomWorker = CreateRandomWorker();
        Worker nonExistWorker = randomWorker;
        Worker nullWorker = null;

        var notFoundWorkerException =
            new NotFoundWorkerException(nonExistWorker.ID);

        var expectedWorkerValidationException =
            new WorkerValidationException(notFoundWorkerException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectWorkerByIdAsync(nonExistWorker.ID))
                .ReturnsAsync(nullWorker);

        // when 
        ValueTask<Worker> modifyWorkerTask =
            this.workerService.ModifyWorkerAsync(nonExistWorker);

        // then
        await Assert.ThrowsAsync<WorkerValidationException>(() =>
            modifyWorkerTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectWorkerByIdAsync(nonExistWorker.ID),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedWorkerValidationException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}