using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using WorkPlan.Api.Models.Plans;
using WorkPlan.Api.Models.Plans.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Plans;

public partial class PlanServiceTests
{
    [Fact]
    public async Task ShouldThrowCriticalDependencyExceptionOnUpdateIfSqlErrorOccursAndLogItAsync()
    {
        // given
        int randomNumber = GetRandomNumber();
        Plan randomPlan = CreateRandomPlan();
        randomPlan.Date = DateOnly.FromDateTime(DateTime.UtcNow);

        SqlException sqlException = GetSqlException();

        var failedPlanStorageException =
            new FailedPlanStorageException(sqlException);

        var expectedPlanDependencyException =
            new PlanDependencyException(failedPlanStorageException);

        this.storageBrokerMock.Setup(broker =>
                broker.SelectPlanByIdAsync(
                    It.IsAny<Guid>()))
                        .ThrowsAsync(sqlException);

        // when
        ValueTask<Plan> modifyPlanTask =
            this.planService.ModifyPlanAsync(randomPlan);

        // then
        await Assert.ThrowsAsync<PlanDependencyException>(() =>
            modifyPlanTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectPlanByIdAsync(randomPlan.ID),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedPlanDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async void ShouldThrowValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
    {
        // given
        int randomNumber = GetRandomNumber(); 
        Plan somePlan = CreateRandomPlan();
        somePlan.Date = DateOnly.FromDateTime(DateTime.UtcNow);
        string randomMessage = GetRandomMessage();
        string exceptionMessage = randomMessage;

        var foreignKeyConstraintConflictException =
            new ForeignKeyConstraintConflictException(exceptionMessage);

        var invalidPlanReferenceException =
            new InvalidPlanReferenceException(foreignKeyConstraintConflictException);

        var expectedPlanDependencyValidationException =
            new PlanDependencyException(invalidPlanReferenceException);

        this.storageBrokerMock.Setup(broker =>
                broker.SelectPlanByIdAsync(
                    It.IsAny<Guid>()))
                        .ThrowsAsync(foreignKeyConstraintConflictException);

        // when
        ValueTask<Plan> modifyPlanTask =
            this.planService.ModifyPlanAsync(somePlan);

        // then
        await Assert.ThrowsAsync<PlanDependencyException>(() =>
            modifyPlanTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedPlanDependencyValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectPlanByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
    {
        // given
        int randomNumber = GetRandomNumber();
        Plan somePlan = CreateRandomPlan();
        somePlan.Date = DateOnly.FromDateTime(DateTime.UtcNow);

        var databaseUpdateException =
            new DbUpdateException();

        var failedPlanStorageException =
            new FailedPlanStorageException(databaseUpdateException);

        var expectedPlanDependencyException =
            new PlanDependencyException(failedPlanStorageException);

        this.storageBrokerMock.Setup(broker =>
                broker.SelectPlanByIdAsync(
                    It.IsAny<Guid>()))
                        .ThrowsAsync(databaseUpdateException);

        // when
        ValueTask<Plan> modifyPlanTask =
            this.planService.ModifyPlanAsync(somePlan);

        // then
        await Assert.ThrowsAsync<PlanDependencyException>(() =>
            modifyPlanTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectPlanByIdAsync(It.IsAny<Guid>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedPlanDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}