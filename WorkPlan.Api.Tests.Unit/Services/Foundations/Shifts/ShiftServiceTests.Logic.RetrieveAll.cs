using FluentAssertions;
using Force.DeepCloner;
using Moq;
using WorkPlan.Api.Models.Shifts;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Shifts;

public partial class ShiftServiceTests
{
    [Fact]
    public void ShouldRetrieveAllShifts()
    {
        // given
        IQueryable<Shift> randomShifts = CreateRandomShifts();
        IQueryable<Shift> storageShifts = randomShifts;
        IQueryable<Shift> expectedShifts = storageShifts;

        this.storageBrokerMock.Setup(broker =>
            broker.SelectAllShifts())
                .Returns(storageShifts);

        // when
        IQueryable<Shift> actualShifts =
            this.shiftService.RetrieveAllShifts();

        // then
        actualShifts.Should().BeEquivalentTo(expectedShifts);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllShifts(),
                Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}