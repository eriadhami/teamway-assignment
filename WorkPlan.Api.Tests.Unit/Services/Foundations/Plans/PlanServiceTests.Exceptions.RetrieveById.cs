using Microsoft.Data.SqlClient;
using Moq;
using WorkPlan.Api.Models.Plans;
using WorkPlan.Api.Models.Plans.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Plans;

public partial class PlanServiceTests
{
    [Fact]
    public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
    {
        //given
        Guid somePlanId = Guid.NewGuid();
        SqlException sqlException = GetSqlException();

        var failedPlanStorageException =
            new FailedPlanStorageException(sqlException);

        var expectedPlanDependencyException =
            new PlanDependencyException(failedPlanStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectPlanByIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(sqlException);

        //when
        ValueTask<Plan> retrievePlanByIdTask =
            this.planService.RetrievePlanByIdAsync(somePlanId);

        //then
        await Assert.ThrowsAsync<PlanDependencyException>(() =>
            retrievePlanByIdTask.AsTask());

        this.storageBrokerMock.Verify(broker =>
            broker.SelectPlanByIdAsync(It.IsAny<Guid>()),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedPlanDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}