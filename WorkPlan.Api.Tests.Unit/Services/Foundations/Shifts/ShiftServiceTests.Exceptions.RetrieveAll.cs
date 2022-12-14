using Microsoft.Data.SqlClient;
using Moq;
using WorkPlan.Api.Models.Shifts.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Shifts;

public partial class ShiftServiceTests
{
    [Fact]
    public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlErrorOccursAndLogIt()
    {
        // given
        SqlException sqlException = GetSqlException();

        var failedShiftStorageException =
            new FailedShiftStorageException(sqlException);

        var expectedShiftDependencyException =
            new ShiftDependencyException(failedShiftStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectAllShifts())
                .Throws(sqlException);

        // when
        Action retrieveAllShiftsAction = () =>
            this.shiftService.RetrieveAllShifts();

        // then
        Assert.Throws<ShiftDependencyException>(
            retrieveAllShiftsAction);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllShifts(),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedShiftDependencyException))),
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

        var failedShiftServiceException =
            new FailedShiftServiceException(serviceException);

        var expectedShiftServiceException =
            new ShiftServiceException(failedShiftServiceException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectAllShifts())
                .Throws(serviceException);

        //when
        Action retrieveAllShiftsAction = () =>
                this.shiftService.RetrieveAllShifts();

        //then
        Assert.Throws<ShiftServiceException>(
            retrieveAllShiftsAction);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllShifts(),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedShiftServiceException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}