using Microsoft.Data.SqlClient;
using Moq;
using WorkPlan.Api.Models.Shifts;
using WorkPlan.Api.Models.Shifts.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Shifts;

public partial class ShiftServiceTests
{
    [Fact]
    public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
    {
        //given
        Guid someShiftId = Guid.NewGuid();
        SqlException sqlException = GetSqlException();

        var failedShiftStorageException =
            new FailedShiftStorageException(sqlException);

        var expectedShiftDependencyException =
            new ShiftDependencyException(failedShiftStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectShiftByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(sqlException);

        //when
        ValueTask<Shift> retrieveShiftByIdTask =
            this.shiftService.RetrieveShiftByIdAsync(someShiftId);

        //then
        await Assert.ThrowsAsync<ShiftDependencyException>(() =>
            retrieveShiftByIdTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectShiftByIdAsync(It.IsAny<Guid>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedShiftDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
    {
        //given
        Guid someShiftId = Guid.NewGuid();
        var serviceException = new Exception();

        var failedShiftServiceException =
            new FailedShiftServiceException(serviceException);

        var expectedShiftServiceException =
            new ShiftServiceException(failedShiftServiceException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectShiftByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(serviceException);

        //when
        ValueTask<Shift> retrieveShiftByIdTask =
            this.shiftService.RetrieveShiftByIdAsync(someShiftId);

        //then
        await Assert.ThrowsAsync<ShiftServiceException>(() =>
            retrieveShiftByIdTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectShiftByIdAsync(It.IsAny<Guid>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedShiftServiceException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}