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
}