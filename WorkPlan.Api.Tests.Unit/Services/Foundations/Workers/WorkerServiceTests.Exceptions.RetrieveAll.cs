using EFxceptions.Models.Exceptions;
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
    public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlErrorOccursAndLogIt()
    {
        // given
        SqlException sqlException = GetSqlException();

        var failedWorkerStorageException =
            new FailedWorkerStorageException(sqlException);

        var expectedWorkerDependencyException =
            new WorkerDependencyException(failedWorkerStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectAllWorkers())
                .Throws(sqlException);

        // when
        Action retrieveAllWorkersAction = () =>
            this.workerService.RetrieveAllWorkers();

        // then
        Assert.Throws<WorkerDependencyException>(
            retrieveAllWorkersAction);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllWorkers(),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedWorkerDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowServiceExceptionOnRetrieveAllWhenServiceErrorOccursAndLogIt()
    {
        //given
        string exceptionMessage = GetRandomMessage();
        var serviceException = new Exception(exceptionMessage);

        var failedWorkerServiceException =
            new FailedWorkerServiceException(serviceException);

        var expectedWorkerServiceException =
            new WorkerServiceException(failedWorkerServiceException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectAllWorkers())
                .Throws(serviceException);

        //when
        Action retrieveAllWorkersAction = () =>
                this.workerService.RetrieveAllWorkers();

        //then
        Assert.Throws<WorkerServiceException>(
            retrieveAllWorkersAction);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllWorkers(),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedWorkerServiceException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}