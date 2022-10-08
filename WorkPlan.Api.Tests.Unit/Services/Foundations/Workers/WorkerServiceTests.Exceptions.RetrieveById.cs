using Microsoft.Data.SqlClient;
using Moq;
using WorkPlan.Api.Models.Workers;
using WorkPlan.Api.Models.Workers.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Workers;

public partial class WorkerServiceTests
{
    [Fact]
    public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
    {
        //given
        Guid someWorkerId = Guid.NewGuid();
        SqlException sqlException = GetSqlException();

        var failedWorkerStorageException =
            new FailedWorkerStorageException(sqlException);

        var expectedWorkerDependencyException =
            new WorkerDependencyException(failedWorkerStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectWorkerByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(sqlException);

        //when
        ValueTask<Worker> retrieveWorkerByIdTask =
            this.workerService.RetrieveWorkerByIdAsync(someWorkerId);

        //then
        await Assert.ThrowsAsync<WorkerDependencyException>(() =>
            retrieveWorkerByIdTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectWorkerByIdAsync(It.IsAny<Guid>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedWorkerDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
    {
        //given
        Guid someWorkerId = Guid.NewGuid();
        var serviceException = new Exception();

        var failedWorkerServiceException =
            new FailedWorkerServiceException(serviceException);

        var expectedWorkerServiceException =
            new WorkerServiceException(failedWorkerServiceException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectWorkerByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(serviceException);

        //when
        ValueTask<Worker> retrieveWorkerByIdTask =
            this.workerService.RetrieveWorkerByIdAsync(someWorkerId);

        //then
        await Assert.ThrowsAsync<WorkerServiceException>(() =>
            retrieveWorkerByIdTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectWorkerByIdAsync(It.IsAny<Guid>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedWorkerServiceException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}