using FluentAssertions;
using Force.DeepCloner;
using Moq;
using WorkPlan.Api.Models.Plans;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Plans;

public partial class PlanServiceTests
{
    [Fact]
    public async Task ShouldAddPlanAsync()
    {
        // given
        Plan randomPlan = CreateRandomPlan();
        Plan inputPlan = randomPlan;
        Plan storagePlan = inputPlan;
        Plan expectedPlan = storagePlan.DeepClone();

        this.storageBrokerMock.Setup(broker =>
            broker.InsertPlanAsync(inputPlan))
                .ReturnsAsync(storagePlan);

        // when
        Plan actualPlan = await this.planService
            .AddPlanAsync(inputPlan);

        // then
        actualPlan.Should().BeEquivalentTo(expectedPlan);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertPlanAsync(inputPlan),
                Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}