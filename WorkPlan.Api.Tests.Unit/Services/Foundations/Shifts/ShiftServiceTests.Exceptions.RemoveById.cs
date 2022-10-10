using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using WorkPlan.Api.Models.Shifts;
using WorkPlan.Api.Models.Shifts.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Shifts;

public partial class ShiftServiceTests
{
    [Fact]
    public async Task ShouldThrowCriticalDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
    {
        // given
        Guid someShiftId = Guid.NewGuid();
        SqlException sqlException = GetSqlException();

        var failedShiftStorageException =
            new FailedShiftStorageException(sqlException);

        var expectedShiftDependencyException =
            new ShiftDependencyException(failedShiftStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectShiftByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(sqlException);

        // when
        ValueTask<Shift> removeShiftByIdTask =
            this.shiftService.RemoveShiftByIdAsync(someShiftId);

        // then
        await Assert.ThrowsAsync<ShiftDependencyException>(() =>
            removeShiftByIdTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectShiftByIdAsync(It.IsAny<Guid>()),
                Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.DeleteShiftAsync(It.IsAny<Shift>()),
                Times.Never);
        
        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedShiftDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
    {
        // given
        Guid someShiftId = Guid.NewGuid();

        var databaseUpdateConcurrencyException =
            new DbUpdateConcurrencyException();

        var lockedShiftException =
            new LockedShiftException(databaseUpdateConcurrencyException);

        var expectedShiftDependencyValidationException =
            new ShiftDependencyException(lockedShiftException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectShiftByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(databaseUpdateConcurrencyException);

        // when
        ValueTask<Shift> removeShiftByIdTask =
            this.shiftService.RemoveShiftByIdAsync(someShiftId);

        // then
        await Assert.ThrowsAsync<ShiftDependencyException>(() =>
            removeShiftByIdTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectShiftByIdAsync(It.IsAny<Guid>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedShiftDependencyValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.DeleteShiftAsync(It.IsAny<Shift>()),
                Times.Never);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
    {
        // given
        Guid someShiftId = Guid.NewGuid();
        var serviceException = new Exception();

        var failedShiftServiceException =
            new FailedShiftServiceException(serviceException);

        var expectedShiftServiceException =
            new ShiftServiceException(failedShiftServiceException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectShiftByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(serviceException);

        // when
        ValueTask<Shift> removeShiftByIdTask =
            this.shiftService.RemoveShiftByIdAsync(someShiftId);

        // then
        await Assert.ThrowsAsync<ShiftServiceException>(() =>
            removeShiftByIdTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectShiftByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedShiftServiceException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.DeleteShiftAsync(It.IsAny<Shift>()),
                    Times.Never());

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}