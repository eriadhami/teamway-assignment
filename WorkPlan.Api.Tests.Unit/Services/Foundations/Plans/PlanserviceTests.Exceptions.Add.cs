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
}