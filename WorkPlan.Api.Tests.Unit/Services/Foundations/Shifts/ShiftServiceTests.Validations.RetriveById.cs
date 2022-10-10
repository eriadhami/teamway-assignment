using Moq;
using WorkPlan.Api.Models.Shifts;
using WorkPlan.Api.Models.Shifts.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Shifts;

public partial class ShiftServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
    {
        //given
        Guid invalidShiftId = Guid.Empty;

        var invalidShiftException =
            new InvalidShiftException();

        invalidShiftException.AddData(
            key: nameof(Shift.ID),
            values: "Id is required");

        var expectedShiftValidationException =
            new ShiftValidationException(invalidShiftException);

        //when
        ValueTask<Shift> retrieveShiftByIdTask =
            this.shiftService.RetrieveShiftByIdAsync(invalidShiftId);

        //then
        await Assert.ThrowsAsync<ShiftValidationException>(() =>
            retrieveShiftByIdTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedShiftValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectShiftByIdAsync(It.IsAny<Guid>()),
                Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfShiftIsNotFoundAndLogItAsync()
    {
        //given
        Guid someShiftId = Guid.NewGuid();
        Shift noShift = null;

        var notFoundShiftException =
            new NotFoundShiftException(someShiftId);

        var expectedShiftValidationException =
            new ShiftValidationException(notFoundShiftException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectShiftByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(noShift);

        //when
        ValueTask<Shift> retrieveShiftByIdTask =
            this.shiftService.RetrieveShiftByIdAsync(someShiftId);

        //then
        await Assert.ThrowsAsync<ShiftValidationException>(() =>
            retrieveShiftByIdTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectShiftByIdAsync(It.IsAny<Guid>()),
                Times.Once());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedShiftValidationException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}