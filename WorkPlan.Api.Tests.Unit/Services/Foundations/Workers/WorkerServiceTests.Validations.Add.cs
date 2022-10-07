using Moq;
using WorkPlan.Api.Models.Workers;
using WorkPlan.Api.Models.Workers.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Workers;

public partial class WorkerServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationExceptionOnAddIfWorkerIsNullAndLogItAsync()
    {
        // given
        Worker invalidWorker = null;

        var nullWorkerException =
            new NullWorkerException();

        var expectedWorkerValidationException =
            new WorkerValidationException(nullWorkerException);

        // when
        ValueTask<Worker> addWorkerTask =
            this.workerService.AddWorkerAsync(invalidWorker);

        // then
        await Assert.ThrowsAsync<WorkerValidationException>(() =>
            addWorkerTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedWorkerValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertWorkerAsync(invalidWorker),
                Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ShouldThrowValidationExceptionOnCreateIfWorkerIsInvalidAndLogItAsync(
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
        ValueTask<Worker> addWorkerTask =
            this.workerService.AddWorkerAsync(invalidWorker);

        // then
        await Assert.ThrowsAsync<WorkerValidationException>(() =>
            addWorkerTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedWorkerValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertWorkerAsync(invalidWorker),
                Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }
}