using FluentAssertions;
using Force.DeepCloner;
using Moq;
using WorkPlan.Api.Models.Shifts;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Shifts;

public partial class ShiftServiceTests
{
    [Fact]
    public async Task ShouldAddShiftAsync()
    {
        // given
        Shift randomShift = CreateRandomShift();
        Shift inputShift = randomShift;
        Shift storageShift = inputShift;
        Shift expectedShift = storageShift.DeepClone();

        this.storageBrokerMock.Setup(broker =>
            broker.InsertShiftAsync(inputShift))
                .ReturnsAsync(storageShift);

        // when
        Shift actualShift = await this.shiftService
            .AddShiftAsync(inputShift);

        // then
        actualShift.Should().BeEquivalentTo(expectedShift);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertShiftAsync(inputShift),
                Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}