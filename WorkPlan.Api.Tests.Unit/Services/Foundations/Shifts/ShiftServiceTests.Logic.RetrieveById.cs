using FluentAssertions;
using Force.DeepCloner;
using Moq;
using WorkPlan.Api.Models.Shifts;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Shifts;

public partial class ShiftServiceTests
{
    [Fact]
    public async void ShouldRetrieveShiftByIdAsync()
    {
        //given
        Shift someShift = CreateRandomShift();
        Shift storageShift = someShift;
        Shift expectedShift = storageShift.DeepClone();

        this.storageBrokerMock.Setup(broker =>
            broker.SelectShiftByIdAsync(someShift.ID))
                .ReturnsAsync(storageShift);

        //when
        Shift actualShift =
            await this.shiftService.RetrieveShiftByIdAsync(someShift.ID);

        //then
        actualShift.Should().BeEquivalentTo(expectedShift);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectShiftByIdAsync(someShift.ID),
                Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}