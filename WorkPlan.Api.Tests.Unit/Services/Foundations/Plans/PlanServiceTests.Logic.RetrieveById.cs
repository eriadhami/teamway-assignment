using FluentAssertions;
using Force.DeepCloner;
using Moq;
using WorkPlan.Api.Models.Plans;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Plans;

public partial class PlanServiceTests
{
    [Fact]
    public async void ShouldRetrievePlanByIdAsync()
    {
        //given
        Plan somePlan = CreateRandomPlan();
        Plan storagePlan = somePlan;
        Plan expectedPlan = storagePlan.DeepClone();

        this.storageBrokerMock.Setup(broker =>
            broker.SelectPlanByIdAsync(somePlan.ID))
                .ReturnsAsync(storagePlan);

        //when
        Plan actualPlan =
            await this.planService.RetrievePlanByIdAsync(somePlan.ID);

        //then
        actualPlan.Should().BeEquivalentTo(expectedPlan);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectPlanByIdAsync(somePlan.ID),
                Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}