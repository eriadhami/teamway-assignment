using EFxceptions.Models.Exceptions;
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
    public async Task ShouldThrowCriticalDependencyExceptionOnUpdateIfSqlErrorOccursAndLogItAsync()
    {
        // given
        int randomNumber = GetRandomNumber();
        Shift randomShift = CreateRandomShift();
        randomShift.EndHour = randomShift.StartHour.AddHours(randomNumber);
        
        SqlException sqlException = GetSqlException();

        var failedShiftStorageException =
            new FailedShiftStorageException(sqlException);

        var expectedShiftDependencyException =
            new ShiftDependencyException(failedShiftStorageException);

        this.storageBrokerMock.Setup(broker =>
                broker.SelectShiftByIdAsync(
                    It.IsAny<Guid>()))
                        .ThrowsAsync(sqlException);

        // when
        ValueTask<Shift> modifyShiftTask =
            this.shiftService.ModifyShiftAsync(randomShift);

        // then
        await Assert.ThrowsAsync<ShiftDependencyException>(() =>
            modifyShiftTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectShiftByIdAsync(randomShift.ID),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedShiftDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}