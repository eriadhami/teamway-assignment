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
    public async Task ShouldThrowCriticalDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
    {
        // given
        Guid somePlanId = Guid.NewGuid();
        SqlException sqlException = GetSqlException();

        var failedPlanStorageException =
            new FailedPlanStorageException(sqlException);

        var expectedPlanDependencyException =
            new PlanDependencyException(failedPlanStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectPlanByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(sqlException);

        // when
        ValueTask<Plan> removePlanByIdTask =
            this.planService.RemovePlanByIdAsync(somePlanId);

        // then
        await Assert.ThrowsAsync<PlanDependencyException>(() =>
            removePlanByIdTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectPlanByIdAsync(It.IsAny<Guid>()),
                Times.Once);

        this.storageBrokerMock.Verify(broker =>
            broker.DeletePlanAsync(It.IsAny<Plan>()),
                Times.Never);
        
        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedPlanDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}