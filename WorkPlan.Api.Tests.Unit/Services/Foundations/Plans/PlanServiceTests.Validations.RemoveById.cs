using Moq;
using WorkPlan.Api.Models.Plans;
using WorkPlan.Api.Models.Plans.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Plans;

public partial class PlanServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
    {
        // given
        Guid invalidPlanId = Guid.Empty;

        var invalidPlanException =
            new InvalidPlanException();

        invalidPlanException.AddData(
            key: nameof(Plan.ID),
            values: "Id is required");

        var expectedPlanValidationException =
            new PlanValidationException(invalidPlanException);

        // when
        ValueTask<Plan> removePlanByIdTask =
            this.planService.RemovePlanByIdAsync(invalidPlanId);

        // then
        await Assert.ThrowsAsync<PlanValidationException>(() =>
            removePlanByIdTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedPlanValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectPlanByIdAsync(It.IsAny<Guid>()),
                Times.Never);

        this.storageBrokerMock.Verify(broker =>
            broker.DeletePlanAsync(It.IsAny<Plan>()),
                Times.Never);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowNotFoundExceptionOnRemovePlanByIdIsNotFoundAndLogItAsync()
    {
        // given
        Guid inputPlanId = Guid.NewGuid();
        Plan noPlan = null;

        var notFoundPlanException =
            new NotFoundPlanException(inputPlanId);

        var expectedPlanValidationException =
            new PlanValidationException(notFoundPlanException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectPlanByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(noPlan);

        // when
        ValueTask<Plan> removePlanByIdTask =
            this.planService.RemovePlanByIdAsync(inputPlanId);

        // then
        await Assert.ThrowsAsync<PlanValidationException>(() =>
            removePlanByIdTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectPlanByIdAsync(It.IsAny<Guid>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedPlanValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.DeletePlanAsync(It.IsAny<Plan>()),
                Times.Never);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}