using Moq;
using WorkPlan.Api.Models.Plans;
using WorkPlan.Api.Models.Plans.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Plans;

public partial class PlanServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
    {
        //given
        Guid invalidPlanId = Guid.Empty;

        var invalidPlanException =
            new InvalidPlanException();

        invalidPlanException.AddData(
            key: nameof(Plan.ID),
            values: "Id is required");

        var expectedPlanValidationException =
            new PlanValidationException(invalidPlanException);

        //when
        ValueTask<Plan> retrievePlanByIdTask =
            this.planService.RetrievePlanByIdAsync(invalidPlanId);

        //then
        await Assert.ThrowsAsync<PlanValidationException>(() =>
            retrievePlanByIdTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedPlanValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectPlanByIdAsync(It.IsAny<Guid>()),
                Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfPlanIsNotFoundAndLogItAsync()
    {
        //given
        Guid somePlanId = Guid.NewGuid();
        Plan noPlan = null;

        var notFoundPlanException =
            new NotFoundPlanException(somePlanId);

        var expectedPlanValidationException =
            new PlanValidationException(notFoundPlanException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectPlanByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(noPlan);

        //when
        ValueTask<Plan> retrievePlanByIdTask =
            this.planService.RetrievePlanByIdAsync(somePlanId);

        //then
        await Assert.ThrowsAsync<PlanValidationException>(() =>
            retrievePlanByIdTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectPlanByIdAsync(It.IsAny<Guid>()),
                Times.Once());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedPlanValidationException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}