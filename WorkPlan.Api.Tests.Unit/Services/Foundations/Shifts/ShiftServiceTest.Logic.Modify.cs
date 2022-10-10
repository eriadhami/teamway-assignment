using FluentAssertions;
using Force.DeepCloner;
using Moq;
using WorkPlan.Api.Models.Shifts;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Shifts;

public partial class ShiftServiceTests
{
    [Fact]
    public async Task ShouldUpdateShiftAsync()
    {
        // given
        int randomNumber = GetRandomNumber();
        Shift randomShift = CreateRandomShift();
        Shift inputShift = randomShift;
        Shift storageShift = inputShift;
        Shift updatedShift = inputShift;
        updatedShift.EndHour = updatedShift.StartHour.AddHours(randomNumber);
        Shift expectedShift = updatedShift.DeepClone();
        Guid inputShiftId = inputShift.ID;

        this.storageBrokerMock.Setup(broker =>
            broker.SelectShiftByIdAsync(inputShiftId))
                    .ReturnsAsync(storageShift);

        this.storageBrokerMock.Setup(broker =>
            broker.UpdateShiftAsync(inputShift))
                    .ReturnsAsync(updatedShift);

        // when
        Shift actualShift =
            await this.shiftService.ModifyShiftAsync(inputShift);

        // then
        actualShift.Should().BeEquivalentTo(expectedShift);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectShiftByIdAsync(inputShiftId),
                Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.UpdateShiftAsync(inputShift),
                Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}