using Moq;
using WorkPlan.Api.Models.Plans;
using WorkPlan.Api.Models.Plans.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Plans;

public partial class PlanServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationExceptionOnModifyIfPlanIsNullAndLogItAsync()
    {
        // given
        Plan invalidPlan = null;

        var nullPlanException =
            new NullPlanException();

        var expectedPlanValidationException =
            new PlanValidationException(nullPlanException);

        // when
        ValueTask<Plan> addPlanTask =
            this.planService.ModifyPlanAsync(invalidPlan);

        // then
        await Assert.ThrowsAsync<PlanValidationException>(() =>
            addPlanTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedPlanValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectPlanByIdAsync(It.IsAny<Guid>()),
                Times.Never);

        this.storageBrokerMock.Verify(broker =>
            broker.UpdatePlanAsync(It.IsAny<Plan>()),
                Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }
}