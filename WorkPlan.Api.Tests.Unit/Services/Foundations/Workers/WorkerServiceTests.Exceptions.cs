using FluentAssertions;
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
        Worker someWorker = CreateRandomWorker();
        SqlException sqlException = GetSqlException();

        var failedWorkerStorageException =
            new FailedWorkerStorageException(sqlException);

        var expectedWorkerDependencyException =
            new WorkerDependencyException(failedWorkerStorageException);

        this.storageBrokerMock.Setup(broker =>
                broker.InsertWorkerAsync(
                    It.IsAny<Worker>()))
                        .ThrowsAsync(sqlException);

        // when
        ValueTask<Worker> addWorkerTask =
            this.workerService.AddWorkerAsync(someWorker);

        WorkerDependencyException actualWorkerDependencyException =
            await Assert.ThrowsAsync<WorkerDependencyException>(
                addWorkerTask.AsTask);

        // then
        actualWorkerDependencyException.Should().BeEquivalentTo(
            expectedWorkerDependencyException);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertWorkerAsync(It.IsAny<Worker>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedWorkerDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}