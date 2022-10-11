using Moq;
using WorkPlan.Api.Models.Plans;
using WorkPlan.Api.Models.Plans.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Plans;

public partial class PlanServiceTests
{
    [Fact]
    public async Task ShouldThrowValidationExceptionOnAddIfPlanIsNullAndLogItAsync()
    {
        // given
        Plan invalidPlan = null;

        var nullPlanException =
            new NullPlanException();

        var expectedPlanValidationException =
            new PlanValidationException(nullPlanException);

        // when
        ValueTask<Plan> addPlanTask =
            this.planService.AddPlanAsync(invalidPlan);

        // then
        await Assert.ThrowsAsync<PlanValidationException>(() =>
            addPlanTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedPlanValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertPlanAsync(invalidPlan),
                Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionOnCreateIfPlanIsInvalidAndLogItAsync()
    {
        // given
        Plan invalidPlan = new Plan();

        var invalidPlanException =
            new InvalidPlanException();

        invalidPlanException.AddData(
            key: nameof(Plan.ID),
            values: "Id is required");

        invalidPlanException.AddData(
            key: nameof(Plan.WorkerID),
            values: "Id is required");

        invalidPlanException.AddData(
            key: nameof(Plan.ShiftID),
            values: "Id is required");

        invalidPlanException.AddData(
            key: nameof(Plan.Date),
            values: "Date is required");

        var expectedPlanValidationException =
            new PlanValidationException(invalidPlanException);

        // when
        ValueTask<Plan> addPlanTask =
            this.planService.AddPlanAsync(invalidPlan);

        // then
        await Assert.ThrowsAsync<PlanValidationException>(() =>
            addPlanTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedPlanValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertPlanAsync(invalidPlan),
                Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }
}