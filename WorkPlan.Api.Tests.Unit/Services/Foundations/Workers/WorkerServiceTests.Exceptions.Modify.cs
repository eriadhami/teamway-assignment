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

    [Fact]
    public async void ShouldThrowValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
    {
        // given
        Worker someWorker = CreateRandomWorker();
        string randomMessage = GetRandomMessage();
        string exceptionMessage = randomMessage;

        var foreignKeyConstraintConflictException =
            new ForeignKeyConstraintConflictException(exceptionMessage);

        var invalidWorkerReferenceException =
            new InvalidWorkerReferenceException(foreignKeyConstraintConflictException);

        var expectedWorkerDependencyValidationException =
            new WorkerDependencyException(invalidWorkerReferenceException);

        this.storageBrokerMock.Setup(broker =>
                broker.SelectWorkerByIdAsync(
                    It.IsAny<Guid>()))
                        .ThrowsAsync(foreignKeyConstraintConflictException);

        // when
        ValueTask<Worker> modifyWorkerTask =
            this.workerService.ModifyWorkerAsync(someWorker);

        // then
        await Assert.ThrowsAsync<WorkerDependencyException>(() =>
            modifyWorkerTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedWorkerDependencyValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectWorkerByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
    {
        // given
        Worker someWorker = CreateRandomWorker();

        var databaseUpdateException =
            new DbUpdateException();

        var failedWorkerStorageException =
            new FailedWorkerStorageException(databaseUpdateException);

        var expectedWorkerDependencyException =
            new WorkerDependencyException(failedWorkerStorageException);

        this.storageBrokerMock.Setup(broker =>
                broker.SelectWorkerByIdAsync(
                    It.IsAny<Guid>()))
                        .ThrowsAsync(databaseUpdateException);

        // when
        ValueTask<Worker> modifyWorkerTask =
            this.workerService.ModifyWorkerAsync(someWorker);

        // then
        await Assert.ThrowsAsync<WorkerDependencyException>(() =>
            modifyWorkerTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectWorkerByIdAsync(It.IsAny<Guid>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedWorkerDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionOnModifyIfServiceErrorOccursAndLogItAsync()
    {
        // given
        Worker someWorker = CreateRandomWorker();
        var serviceException = new Exception();

        var failedWorkerServiceException =
            new FailedWorkerServiceException(serviceException);

        var expectedWorkerServiceException =
            new WorkerServiceException(failedWorkerServiceException);

        this.storageBrokerMock.Setup(broker =>
                broker.SelectWorkerByIdAsync(
                    It.IsAny<Guid>()))
                        .ThrowsAsync(serviceException);

        // when
        ValueTask<Worker> modifyWorkerTask =
            this.workerService.ModifyWorkerAsync(someWorker);

        // then
        await Assert.ThrowsAsync<WorkerServiceException>(() =>
            modifyWorkerTask.AsTask());

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