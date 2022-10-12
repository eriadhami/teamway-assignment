using Microsoft.Data.SqlClient;
using WorkPlan.Api.Models.Plans;
using WorkPlan.Api.Models.Plans.Exceptions;
using Xeptions;

namespace WorkPlan.Api.Services.Foundations.Plans;

public partial class PlanService
{
    private delegate ValueTask<Plan> ReturningPlanFunction();

    private async ValueTask<Plan> TryCatch(ReturningPlanFunction returningPlanFunction)
    {
        try
        {
            return await returningPlanFunction();
        }
        catch (NullPlanException nullPlanException)
        {
            throw CreateAndLogValidationException(nullPlanException);
        }
        catch (InvalidPlanException invalidPlanException)
        {
            throw CreateAndLogValidationException(invalidPlanException);
        }
        catch (SqlException sqlException)
        {
            var failedPlanStorageException =
                new FailedPlanStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedPlanStorageException);
        }
    }

    private PlanValidationException CreateAndLogValidationException(Xeption exception)
    {
        var planValidationException = new PlanValidationException(exception);
        this.loggingBroker.LogError(planValidationException);

        return planValidationException;
    }

    private PlanDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
    {
        var planDependencyException = new PlanDependencyException(exception);
        this.loggingBroker.LogCritical(planDependencyException);

        return planDependencyException;
    }
}