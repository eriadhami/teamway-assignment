using Microsoft.Data.SqlClient;
using Moq;
using WorkPlan.Api.Models.Workers;
using WorkPlan.Api.Models.Workers.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Workers;

public partial class WorkerServiceTests
{
    [Fact]
    public async Task ShouldThrowCriticalDependencyExceptionOnUpdateIfSqlErrorOccursAndLogItAsync()
    {
        // given
        Worker randomWorker = CreateRandomWorker();
        SqlException sqlException = GetSqlException();

        var failedWorkerStorageException =
            new FailedWorkerStorageException(sqlException);

        var expectedWorkerDependencyException =
            new WorkerDependencyException(failedWorkerStorageException);

        this.storageBrokerMock.Setup(broker =>
                broker.SelectWorkerByIdAsync(
                    It.IsAny<Guid>()))
                        .ThrowsAsync(sqlException);

        // when
        ValueTask<Worker> modifyWorkerTask =
            this.workerService.ModifyWorkerAsync(randomWorker);

        // then
        await Assert.ThrowsAsync<WorkerDependencyException>(() =>
            modifyWorkerTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectWorkerByIdAsync(randomWorker.ID),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedWorkerDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}