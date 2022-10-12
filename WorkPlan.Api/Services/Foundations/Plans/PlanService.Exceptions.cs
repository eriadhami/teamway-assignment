using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WorkPlan.Api.Models.Plans;
using WorkPlan.Api.Models.Plans.Exceptions;
using Xeptions;

namespace WorkPlan.Api.Services.Foundations.Plans;

public partial class PlanService
{
    private delegate ValueTask<Plan> ReturningPlanFunction();
    private delegate IQueryable<Plan> ReturningPlansFunction();

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
        catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
        {
            var invalidPlanReferenceException =
                new InvalidPlanReferenceException(foreignKeyConstraintConflictException);

            throw CreateAndLogDependencyException(invalidPlanReferenceException);
        }
        catch (DbUpdateException databaseUpdateException)
        {
            var failedStoragePlanException =
                new FailedPlanStorageException(databaseUpdateException);

            throw CreateAndLogDependencyException(failedStoragePlanException);
        }
        catch (Exception serviceException)
        {
            var failedServicePlanException =
                new FailedPlanServiceException(serviceException);

            throw CreateAndLogServiceException(failedServicePlanException);
        }
    }

    private IQueryable<Plan> TryCatch(ReturningPlansFunction returningPlansFunction)
    {
        try
        {
            return returningPlansFunction();
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

    private PlanDependencyException CreateAndLogDependencyException(Xeption exception)
    {
        var planDependencyException = new PlanDependencyException(exception);
        this.loggingBroker.LogError(planDependencyException);

        return planDependencyException;
    }

    private PlanServiceException CreateAndLogServiceException(Xeption exception)
    {
        var planServiceException = new PlanServiceException(exception);
        this.loggingBroker.LogError(planServiceException);

        return planServiceException;
    }
}