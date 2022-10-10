using Moq;
using WorkPlan.Api.Models.Shifts;
using WorkPlan.Api.Models.Shifts.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Shifts;

public partial class ShiftServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationExceptionOnModifyIfShiftIsNullAndLogItAsync()
    {
        // given
        Shift invalidShift = null;

        var nullShiftException =
            new NullShiftException();

        var expectedShiftValidationException =
            new ShiftValidationException(nullShiftException);

        // when
        ValueTask<Shift> addShiftTask =
            this.shiftService.ModifyShiftAsync(invalidShift);

        // then
        await Assert.ThrowsAsync<ShiftValidationException>(() =>
            addShiftTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedShiftValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectShiftByIdAsync(It.IsAny<Guid>()),
                Times.Never);

        this.storageBrokerMock.Verify(broker =>
            broker.UpdateShiftAsync(It.IsAny<Shift>()),
                Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ShouldThrowValidationExceptionOnUpdateIfShiftIsInvalidAndLogItAsync(
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
            key: nameof(Shift.EndHour),
            values: $"Time is the same as or before the {nameof(Shift.StartHour)}");

        var expectedShiftValidationException =
            new ShiftValidationException(invalidShiftException);

        // when
        ValueTask<Shift> updateShiftTask =
            this.shiftService.ModifyShiftAsync(invalidShift);

        // then
        await Assert.ThrowsAsync<ShiftValidationException>(() =>
            updateShiftTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedShiftValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.UpdateShiftAsync(invalidShift),
                Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionOnModifyIfShiftDoesNotExistAndLogItAsync()
    {
        // given
        int randomNumber = GetRandomNumber();
        Shift randomShift = CreateRandomShift();
        Shift nonExistShift = randomShift;
        nonExistShift.EndHour = nonExistShift.StartHour.AddHours(randomNumber);
        Shift nullShift = null;

        var notFoundShiftException =
            new NotFoundShiftException(nonExistShift.ID);

        var expectedShiftValidationException =
            new ShiftValidationException(notFoundShiftException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectShiftByIdAsync(nonExistShift.ID))
                .ReturnsAsync(nullShift);

        // when 
        ValueTask<Shift> modifyShiftTask =
            this.shiftService.ModifyShiftAsync(nonExistShift);

        // then
        await Assert.ThrowsAsync<ShiftValidationException>(() =>
            modifyShiftTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectShiftByIdAsync(nonExistShift.ID),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedShiftValidationException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}