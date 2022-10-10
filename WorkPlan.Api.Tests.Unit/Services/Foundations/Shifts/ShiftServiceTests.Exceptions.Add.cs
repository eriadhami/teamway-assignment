using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;
using WorkPlan.Api.Models.Shifts;
using WorkPlan.Api.Models.Shifts.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Shifts;

public partial class ShiftServiceTests
{
    [Fact]
    public async Task ShouldThrowCriticalDependencyExceptionOnCreateIfSqlErrorOccursAndLogItAsync()
    {
        // given
        Shift someShift = CreateRandomShift();
        int randomNumber = GetRandomNumber();
        someShift.EndHour = someShift.StartHour.AddHours(randomNumber);
        SqlException sqlException = GetSqlException();

        var failedShiftStorageException =
            new FailedShiftStorageException(sqlException);

        var expectedShiftDependencyException =
            new ShiftDependencyException(failedShiftStorageException);

        this.storageBrokerMock.Setup(broker =>
                broker.InsertShiftAsync(
                    It.IsAny<Shift>()))
                        .ThrowsAsync(sqlException);

        // when
        ValueTask<Shift> addShiftTask =
            this.shiftService.AddShiftAsync(someShift);

        ShiftDependencyException actualShiftDependencyException =
            await Assert.ThrowsAsync<ShiftDependencyException>(
                addShiftTask.AsTask);

        // then
        await Assert.ThrowsAsync<ShiftDependencyException>(() =>
            addShiftTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.InsertShiftAsync(It.IsAny<Shift>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedShiftDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async void ShouldThrowValidationExceptionOnCreateIfReferenceErrorOccursAndLogItAsync()
    {
        // given
        int randomNumber = GetRandomNumber();
        Shift someShift = CreateRandomShift();
        someShift.EndHour = someShift.StartHour.AddHours(randomNumber);
        string randomMessage = GetRandomMessage();
        string exceptionMessage = randomMessage;

        var foreignKeyConstraintConflictException =
            new ForeignKeyConstraintConflictException(exceptionMessage);

        var invalidShiftReferenceException =
            new InvalidShiftReferenceException(foreignKeyConstraintConflictException);

        var expectedShiftDependencyValidationException =
            new ShiftDependencyException(invalidShiftReferenceException);

        this.storageBrokerMock.Setup(broker =>
                broker.InsertShiftAsync(
                    It.IsAny<Shift>()))
                        .ThrowsAsync(foreignKeyConstraintConflictException);

        // when
        ValueTask<Shift> addShiftTask =
            this.shiftService.AddShiftAsync(someShift);

        // then
        await Assert.ThrowsAsync<ShiftDependencyException>(() =>
            addShiftTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedShiftDependencyValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertShiftAsync(It.IsAny<Shift>()),
                    Times.Once);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }
}