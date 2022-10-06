using Microsoft.Data.SqlClient;
using Moq;
using WorkPlan.Api.Models.Workers;
using WorkPlan.Api.Models.Workers.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Workers;

public partial class WorkerServiceTests
{
    [Fact]
    public async Task ShouldThrowCriticalDependencyExceptionOnCreateIfSqlErrorOccursAndLogItAsync()
    {
        // given
        DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
        Worker someWorker = CreateRandomWorker(randomDateTime);
        SqlException sqlException = GetSqlException();

        var failedWorkerStorageException =
            new FailedWorkerStorageException(sqlException);

        var expectedWorkerDependencyException =
            new WorkerDependencyException(failedWorkerStorageException);

        this.dateTimeBrokerMock.Setup(broker =>
            broker.GetCurrentDateTimeOffset())
                .Throws(sqlException);

        // when
        ValueTask<Worker> addWorkerTask =
            this.workerService.AddWorkerAsync(someWorker);

        // then
        await Assert.ThrowsAsync<WorkerDependencyException>(() =>
            addWorkerTask.AsTask());

        this.dateTimeBrokerMock.Verify(broker =>
            broker.GetCurrentDateTimeOffset(),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedWorkerDependencyException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertWorkerAsync(It.IsAny<Worker>()),
                Times.Never);

        this.dateTimeBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }
}