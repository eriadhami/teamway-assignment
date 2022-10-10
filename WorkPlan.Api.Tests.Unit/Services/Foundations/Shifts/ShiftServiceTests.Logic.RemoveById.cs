using FluentAssertions;
using Force.DeepCloner;
using Moq;
using WorkPlan.Api.Models.Shifts;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Shifts;

public partial class ShiftServiceTests
{
    [Fact]
    public async void ShouldRemoveShiftByIdAsync()
    {
        // given
        Guid randomId = Guid.NewGuid();
        Guid inputShiftId = randomId;
        Shift randomShift = CreateRandomShift();
        Shift storageShift = randomShift;
        Shift expectedInputShift = storageShift;
        Shift deletedShift = expectedInputShift;
        Shift expectedShift = deletedShift.DeepClone();

        this.storageBrokerMock.Setup(broker =>
            broker.SelectShiftByIdAsync(inputShiftId))
                .ReturnsAsync(storageShift);

        this.storageBrokerMock.Setup(broker =>
            broker.DeleteShiftAsync(expectedInputShift))
                .ReturnsAsync(deletedShift);

        // when
        Shift actualShift = await this.shiftService
            .RemoveShiftByIdAsync(inputShiftId);

        // then
        actualShift.Should().BeEquivalentTo(expectedShift);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectShiftByIdAsync(inputShiftId),
                Times.Once());

        this.storageBrokerMock.Verify(broker =>
            broker.DeleteShiftAsync(expectedInputShift),
                Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}