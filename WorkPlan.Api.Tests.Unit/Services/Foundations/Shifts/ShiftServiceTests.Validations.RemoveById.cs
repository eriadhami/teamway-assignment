using Moq;
using WorkPlan.Api.Models.Shifts;
using WorkPlan.Api.Models.Shifts.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Shifts;

public partial class ShiftServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
    {
        // given
        Guid invalidShiftId = Guid.Empty;

        var invalidShiftException =
            new InvalidShiftException();

        invalidShiftException.AddData(
            key: nameof(Shift.ID),
            values: "Id is required");

        var expectedShiftValidationException =
            new ShiftValidationException(invalidShiftException);

        // when
        ValueTask<Shift> removeShiftByIdTask =
            this.shiftService.RemoveShiftByIdAsync(invalidShiftId);

        // then
        await Assert.ThrowsAsync<ShiftValidationException>(() =>
            removeShiftByIdTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedShiftValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectShiftByIdAsync(It.IsAny<Guid>()),
                Times.Never);

        this.storageBrokerMock.Verify(broker =>
            broker.DeleteShiftAsync(It.IsAny<Shift>()),
                Times.Never);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowNotFoundExceptionOnRemoveShiftByIdIsNotFoundAndLogItAsync()
    {
        // given
        Guid inputShiftId = Guid.NewGuid();
        Shift noShift = null;

        var notFoundShiftException =
            new NotFoundShiftException(inputShiftId);

        var expectedShiftValidationException =
            new ShiftValidationException(notFoundShiftException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectShiftByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(noShift);

        // when
        ValueTask<Shift> removeShiftByIdTask =
            this.shiftService.RemoveShiftByIdAsync(inputShiftId);

        // then
        await Assert.ThrowsAsync<ShiftValidationException>(() =>
            removeShiftByIdTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectShiftByIdAsync(It.IsAny<Guid>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedShiftValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.DeleteShiftAsync(It.IsAny<Shift>()),
                Times.Never);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}