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
        await Assert.ThrowsAsync<WorkerDependencyException>(() =>
            addWorkerTask.AsTask());

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

    [Fact]
    public async Task ShouldThrowDependencyValidationExceptionOnCreateIfWorkerAlreadyExsitsAndLogItAsync()
    {
        // given
        Worker randomWorker = CreateRandomWorker();
        Worker alreadyExistsWorker = randomWorker;
        string randomMessage = GetRandomMessage();

        var duplicateKeyException =
            new DuplicateKeyException(randomMessage);

        var alreadyExistsWorkerException =
            new AlreadyExistsWorkerException(duplicateKeyException);

        var expectedWorkerDependencyValidationException =
            new WorkerDependencyException(alreadyExistsWorkerException);

        this.storageBrokerMock.Setup(broker =>
                broker.InsertWorkerAsync(
                    It.IsAny<Worker>()))
                        .ThrowsAsync(duplicateKeyException);

        // when
        ValueTask<Worker> addWorkerTask =
            this.workerService.AddWorkerAsync(alreadyExistsWorker);

        // then
        await Assert.ThrowsAsync<WorkerDependencyException>(() =>
            addWorkerTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.InsertWorkerAsync(It.IsAny<Worker>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedWorkerDependencyValidationException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async void ShouldThrowValidationExceptionOnCreateIfReferenceErrorOccursAndLogItAsync()
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
                broker.InsertWorkerAsync(
                    It.IsAny<Worker>()))
                        .ThrowsAsync(foreignKeyConstraintConflictException);

        // when
        ValueTask<Worker> addWorkerTask =
            this.workerService.AddWorkerAsync(someWorker);

        // then
        await Assert.ThrowsAsync<WorkerDependencyException>(() =>
            addWorkerTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedWorkerDependencyValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertWorkerAsync(It.IsAny<Worker>()),
                    Times.Once);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyExceptionOnCreateIfDatabaseUpdateErrorOccursAndLogItAsync()
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
                broker.InsertWorkerAsync(
                    It.IsAny<Worker>()))
                        .ThrowsAsync(databaseUpdateException);

        // when
        ValueTask<Worker> addWorkerTask =
            this.workerService.AddWorkerAsync(someWorker);

        // then
        await Assert.ThrowsAsync<WorkerDependencyException>(() =>
            addWorkerTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.InsertWorkerAsync(It.IsAny<Worker>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedWorkerDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionOnCreateIfServiceErrorOccursAndLogItAsync()
    {
        // given
        Worker someWorker = CreateRandomWorker();
        var serviceException = new Exception();

        var failedWorkerServiceException =
            new FailedWorkerServiceException(serviceException);

        var expectedWorkerServiceException =
            new WorkerServiceException(failedWorkerServiceException);

        this.storageBrokerMock.Setup(broker =>
                broker.InsertWorkerAsync(
                    It.IsAny<Worker>()))
                        .ThrowsAsync(serviceException);

        // when
        ValueTask<Worker> addWorkerTask =
            this.workerService.AddWorkerAsync(someWorker);

        // then
        await Assert.ThrowsAsync<WorkerServiceException>(() =>
            addWorkerTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.InsertWorkerAsync(It.IsAny<Worker>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedWorkerServiceException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}