using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using WorkPlan.Api.Models.Workers;
using WorkPlan.Api.Models.Workers.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Workers;

public partial class WorkerServiceTests
{
    [Fact]
    public async Task ShouldThrowCriticalDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
    {
        // given
        Guid someWorkerId = Guid.NewGuid();
        SqlException sqlException = GetSqlException();

        var failedWorkerStorageException =
            new FailedWorkerStorageException(sqlException);

        var expectedWorkerDependencyException =
            new WorkerDependencyException(failedWorkerStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectWorkerByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(sqlException);

        // when
        ValueTask<Worker> removeWorkerByIdTask =
            this.workerService.RemoveWorkerByIdAsync(someWorkerId);

        // then
        await Assert.ThrowsAsync<WorkerDependencyException>(() =>
            removeWorkerByIdTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectWorkerByIdAsync(It.IsAny<Guid>()),
                Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.DeleteWorkerAsync(It.IsAny<Worker>()),
                Times.Never);
        
        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedWorkerDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
    {
        // given
        Guid someWorkerId = Guid.NewGuid();

        var databaseUpdateConcurrencyException =
            new DbUpdateConcurrencyException();

        var lockedWorkerException =
            new LockedWorkerException(databaseUpdateConcurrencyException);

        var expectedWorkerDependencyValidationException =
            new WorkerDependencyException(lockedWorkerException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectWorkerByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(databaseUpdateConcurrencyException);

        // when
        ValueTask<Worker> removeWorkerByIdTask =
            this.workerService.RemoveWorkerByIdAsync(someWorkerId);

        // then
        await Assert.ThrowsAsync<WorkerDependencyException>(() =>
            removeWorkerByIdTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectWorkerByIdAsync(It.IsAny<Guid>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedWorkerDependencyValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.DeleteWorkerAsync(It.IsAny<Worker>()),
                Times.Never);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}