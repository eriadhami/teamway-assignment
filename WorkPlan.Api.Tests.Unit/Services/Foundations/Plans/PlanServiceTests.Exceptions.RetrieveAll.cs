using Microsoft.Data.SqlClient;
using Moq;
using WorkPlan.Api.Models.Plans.Exceptions;
using Xunit;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Plans;

public partial class PlanServiceTests
{
    [Fact]
    public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlErrorOccursAndLogIt()
    {
        // given
        SqlException sqlException = GetSqlException();

        var failedPlanStorageException =
            new FailedPlanStorageException(sqlException);

        var expectedPlanDependencyException =
            new PlanDependencyException(failedPlanStorageException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectAllPlans())
                .Throws(sqlException);

        // when
        Action retrieveAllPlansAction = () =>
            this.planService.RetrieveAllPlans();

        // then
        Assert.Throws<PlanDependencyException>(
            retrieveAllPlansAction);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllPlans(),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedPlanDependencyException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void ShouldThrowServiceExceptionOnRetrieveAllWhenServiceErrorOccursAndLogIt()
    {
        //given
        string exceptionMessage = GetRandomMessage();
        var serviceException = new Exception(exceptionMessage);

        var failedPlanServiceException =
            new FailedPlanServiceException(serviceException);

        var expectedPlanServiceException =
            new PlanServiceException(failedPlanServiceException);

        this.storageBrokerMock.Setup(broker =>
            broker.SelectAllPlans())
                .Throws(serviceException);

        //when
        Action retrieveAllPlansAction = () =>
                this.planService.RetrieveAllPlans();

        //then
        Assert.Throws<PlanServiceException>(
            retrieveAllPlansAction);

        this.storageBrokerMock.Verify(broker =>
            broker.SelectAllPlans(),
                Times.Once);

        this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expectedPlanServiceException))),
                    Times.Once);

        this.storageBrokerMock.VerifyNoOtherCalls();
        this.loggingBrokerMock.VerifyNoOtherCalls();
    }
}