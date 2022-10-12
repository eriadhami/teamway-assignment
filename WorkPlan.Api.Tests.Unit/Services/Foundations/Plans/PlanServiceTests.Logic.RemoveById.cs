using FluentAssertions;
using Force.DeepCloner;
using Moq;
using WorkPlan.Api.Models.Plans;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Plans;

public partial class PlanServiceTests
{
    [Fact]
    public async void ShouldRemovePlanByIdAsync()
    {
        // given
        Guid randomId = Guid.NewGuid();
        Guid inputPlanId = randomId;
        Plan randomPlan = CreateRandomPlan();
        Plan storagePlan = randomPlan;
        Plan expectedInputPlan = storagePlan;
        Plan deletedPlan = expectedInputPlan;
        Plan expectedPlan = deletedPlan.DeepClone();

        this.storageBrokerMock.Setup(broker =>
            broker.SelectPlanByIdAsync(inputPlanId))
                .ReturnsAsync(storagePlan);

        this.storageBrokerMock.Setup(broker =>
            broker.DeletePlanAsync(expectedInputPlan))
                .ReturnsAsync(deletedPlan);

        // when
        Plan actualPlan = await this.planService
            .RemovePlanByIdAsync(inputPlanId);

        // then
        actualPlan.Should().BeEquivalentTo(expectedPlan);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectPlanByIdAsync(inputPlanId),
                Times.Once());

        this.storageBrokerMock.Verify(broker =>
            broker.DeletePlanAsync(expectedInputPlan),
                Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}