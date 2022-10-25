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
    
    [Fact]
    public async Task ShouldThrowValidationExceptionOnUpdateIfPlanIsInvalidAndLogItAsync()
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
        ValueTask<Plan> updatePlanTask =
            this.planService.ModifyPlanAsync(invalidPlan);

        // then
        await Assert.ThrowsAsync<PlanValidationException>(() =>
            updatePlanTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedPlanValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.UpdatePlanAsync(invalidPlan),
                Times.Never);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionOnModifyIfPlanDoesNotExistAndLogItAsync()
    {
        // given
        int randomNumber = GetRandomNumber();
        Plan randomPlan = CreateRandomPlan();
        Plan nonExistPlan = randomPlan;
        nonExistPlan.Date = DateOnly.FromDateTime(DateTime.UtcNow);
        Plan nullPlan = null;

        var notFoundPlanException =
            new NotFoundPlanException(nonExistPlan.ID);

        var expectedPlanValidationException =
            new PlanValidationException(notFoundPlanException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectPlanByIdAsync(nonExistPlan.ID))
                .ReturnsAsync(nullPlan);

        // when 
        ValueTask<Plan> modifyPlanTask =
            this.planService.ModifyPlanAsync(nonExistPlan);

        // then
        await Assert.ThrowsAsync<PlanValidationException>(() =>
            modifyPlanTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectPlanByIdAsync(nonExistPlan.ID),
                Times.Once);
        
        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllPlans(),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedPlanValidationException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}