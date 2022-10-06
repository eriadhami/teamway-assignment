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

        this.dateTimeBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }
}