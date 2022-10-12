using FluentAssertions;
using Force.DeepCloner;
using Moq;
using WorkPlan.Api.Models.Plans;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Plans;

public partial class PlanServiceTests
{
    [Fact]
    public async Task ShouldUpdatePlanAsync()
    {
        // given
        int randomNumber = GetRandomNumber();
        Plan randomPlan = CreateRandomPlan();
        Plan inputPlan = randomPlan;
        Plan storagePlan = inputPlan;
        Plan updatedPlan = inputPlan;
        updatedPlan.Date = DateOnly.FromDateTime(DateTime.UtcNow);
        Plan expectedPlan = updatedPlan.DeepClone();
        Guid inputPlanId = inputPlan.ID;

        this.storageBrokerMock.Setup(broker =>
            broker.SelectPlanByIdAsync(inputPlanId))
                    .ReturnsAsync(storagePlan);

        this.storageBrokerMock.Setup(broker =>
            broker.UpdatePlanAsync(inputPlan))
                    .ReturnsAsync(updatedPlan);

        // when
        Plan actualPlan =
            await this.planService.ModifyPlanAsync(inputPlan);

        // then
        actualPlan.Should().BeEquivalentTo(expectedPlan);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectPlanByIdAsync(inputPlanId),
                Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.UpdatePlanAsync(inputPlan),
                Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}