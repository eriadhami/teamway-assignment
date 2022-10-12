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
    public async Task ShouldThrowCriticalDependencyExceptionOnCreateIfSqlErrorOccursAndLogItAsync()
    {
        // given
        Plan somePlan = CreateRandomPlan();
        somePlan.Date = DateOnly.FromDateTime(DateTime.UtcNow);
        SqlException sqlException = GetSqlException();

        var failedPlanStorageException =
            new FailedPlanStorageException(sqlException);

        var expectedPlanDependencyException =
            new PlanDependencyException(failedPlanStorageException);

        this.storageBrokerMock.Setup(broker =>
                broker.InsertPlanAsync(
                    It.IsAny<Plan>()))
                        .ThrowsAsync(sqlException);

        // when
        ValueTask<Plan> addPlanTask =
            this.planService.AddPlanAsync(somePlan);

        PlanDependencyException actualPlanDependencyException =
            await Assert.ThrowsAsync<PlanDependencyException>(
                addPlanTask.AsTask);

        // then
        await Assert.ThrowsAsync<PlanDependencyException>(() =>
            addPlanTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.InsertPlanAsync(It.IsAny<Plan>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedPlanDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async void ShouldThrowValidationExceptionOnCreateIfReferenceErrorOccursAndLogItAsync()
    {
        // given
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
                broker.InsertPlanAsync(
                    It.IsAny<Plan>()))
                        .ThrowsAsync(foreignKeyConstraintConflictException);

        // when
        ValueTask<Plan> addPlanTask =
            this.planService.AddPlanAsync(somePlan);

        // then
        await Assert.ThrowsAsync<PlanDependencyException>(() =>
            addPlanTask.AsTask());

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedPlanDependencyValidationException))),
                    Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.InsertPlanAsync(It.IsAny<Plan>()),
                    Times.Once);

        this.loggingBrokerMock.VerifyNoOtherCalls();
        this.storageBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowDependencyExceptionOnCreateIfDatabaseUpdateErrorOccursAndLogItAsync()
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
                broker.InsertPlanAsync(
                    It.IsAny<Plan>()))
                        .ThrowsAsync(databaseUpdateException);

        // when
        ValueTask<Plan> addPlanTask =
            this.planService.AddPlanAsync(somePlan);

        // then
        await Assert.ThrowsAsync<PlanDependencyException>(() =>
            addPlanTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.InsertPlanAsync(It.IsAny<Plan>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedPlanDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ShouldThrowServiceExceptionOnCreateIfServiceErrorOccursAndLogItAsync()
    {
        // given
        int randomNumber = GetRandomNumber();
        Plan somePlan = CreateRandomPlan();
        somePlan.Date = DateOnly.FromDateTime(DateTime.UtcNow);
        
        var serviceException = new Exception();

        var failedPlanServiceException =
            new FailedPlanServiceException(serviceException);

        var expectedPlanServiceException =
            new PlanServiceException(failedPlanServiceException);

        this.storageBrokerMock.Setup(broker =>
                broker.InsertPlanAsync(
                    It.IsAny<Plan>()))
                        .ThrowsAsync(serviceException);

        // when
        ValueTask<Plan> addPlanTask =
            this.planService.AddPlanAsync(somePlan);

        // then
        await Assert.ThrowsAsync<PlanServiceException>(() =>
            addPlanTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.InsertPlanAsync(It.IsAny<Plan>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedPlanServiceException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}