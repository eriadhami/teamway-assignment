using FluentAssertions;
using Force.DeepCloner;
using Moq;
using WorkPlan.Api.Models.Plans;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Plans;

public partial class PlanServiceTests
{
    [Fact]
    public void ShouldRetrieveAllPlans()
    {
        // given
        IQueryable<Plan> randomPlans = CreateRandomPlans();
        IQueryable<Plan> storagePlans = randomPlans;
        IQueryable<Plan> expectedPlans = storagePlans;

        this.storageBrokerMock.Setup(broker =>
            broker.SelectAllPlans())
                .Returns(storagePlans);

        // when
        IQueryable<Plan> actualPlans =
            this.planService.RetrieveAllPlans();

        // then
        actualPlans.Should().BeEquivalentTo(expectedPlans);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllPlans(),
                Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}