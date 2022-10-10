using FluentAssertions;
using Moq;
using WorkPlan.Api.Models.Shifts;
using WorkPlan.Api.Models.Shifts.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Shifts;

public partial class ShiftServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationExceptionOnAddIfShiftIsNullAndLogItAsync()
    {
        // given
        Shift invalidShift = null;

        var nullShiftException =
            new NullShiftException();

        var expectedShiftValidationException =
            new ShiftValidationException(nullShiftException);

        // when
        ValueTask<Shift> addShiftTask =
            this.shiftService.AddShiftAsync(invalidShift);

        // then
        await Assert.ThrowsAsync<ShiftValidationException>(() =>
            addShiftTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedShiftValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertShiftAsync(invalidShift),
                Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ShouldThrowValidationExceptionOnCreateIfShiftIsInvalidAndLogItAsync(
        string invalidText)
    {
        // given
        var invalidShift = new Shift
        {
            Name = invalidText
        };

        var invalidShiftException =
            new InvalidShiftException();

        invalidShiftException.AddData(
            key: nameof(Shift.ID),
            values: "Id is required");

        invalidShiftException.AddData(
            key: nameof(Shift.Name),
            values: "Text is required");

        invalidShiftException.AddData(
            key: nameof(Shift.StartHour),
            values: "Time is required");

        invalidShiftException.AddData(
            key: nameof(Shift.EndHour),
            values:
            new[] {
                "Time is required",
                $"Time is the same as {nameof(Shift.StartHour)}"
            });

        var expectedShiftValidationException =
            new ShiftValidationException(invalidShiftException);

        // when
        ValueTask<Shift> addShiftTask =
            this.shiftService.AddShiftAsync(invalidShift);

        // then
        await Assert.ThrowsAsync<ShiftValidationException>(() =>
            addShiftTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedShiftValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertShiftAsync(invalidShift),
                Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }
}