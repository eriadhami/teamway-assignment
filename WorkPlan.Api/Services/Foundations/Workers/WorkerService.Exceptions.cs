using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WorkPlan.Api.Models.Workers;
using WorkPlan.Api.Models.Workers.Exceptions;
using Xeptions;

namespace WorkPlan.Api.Services.Foundations.Workers;

public partial class WorkerService
{
    private delegate ValueTask<Worker> ReturningWorkerFunction();
    private delegate IQueryable<Worker> ReturningWorkersFunction();

    private async ValueTask<Worker> TryCatch(ReturningWorkerFunction returningWorkerFunction)
    {
        try
        {
            return await returningWorkerFunction();
        }
        catch (NullWorkerException nullWorkerException)
        {
            throw CreateAndLogValidationException(nullWorkerException);
        }
        catch (InvalidWorkerException invalidWorkerException)
        {
            throw CreateAndLogValidationException(invalidWorkerException);
        }
        catch (NotFoundWorkerException notFoundWorkerException)
        {
            throw CreateAndLogValidationException(notFoundWorkerException);
        }
        catch (SqlException sqlException)
        {
            var failedWorkerStorageException =
                new FailedWorkerStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedWorkerStorageException);
        }
        catch (DuplicateKeyException duplicateKeyException)
        {
            var alreadyExistWorkerException =
                new AlreadyExistsWorkerException(duplicateKeyException);

            throw CreateAndLogDependencyException(alreadyExistWorkerException);
        }
        catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
        {
            var invalidWorkerReferenceException =
                new InvalidWorkerReferenceException(foreignKeyConstraintConflictException);

            throw CreateAndLogDependencyException(invalidWorkerReferenceException);
        }
        catch (DbUpdateConcurrencyException databaseUpdateConcurrencyException)
        {
            var lockedWorkerException =
                new LockedWorkerException(databaseUpdateConcurrencyException);

            throw CreateAndLogDependencyException(lockedWorkerException);
        }
        catch (DbUpdateException databaseUpdateException)
        {
            var failedStorageWorkerException =
                new FailedWorkerStorageException(databaseUpdateException);

            throw CreateAndLogDependencyException(failedStorageWorkerException);
        }
    }

    private IQueryable<Worker> TryCatch(ReturningWorkersFunction returningWorkersFunction)
    {
        try
        {
            return returningWorkersFunction();
        }
        catch (SqlException sqlException)
        {
            var failedWorkerStorageException =
                new FailedWorkerStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedWorkerStorageException);
        }
        catch (Exception serviceException)
        {
            var failedServiceWorkerException =
                new FailedWorkerServiceException(serviceException);

            throw CreateAndLogServiceException(failedServiceWorkerException);
        }
    }

    private WorkerValidationException CreateAndLogValidationException(Xeption exception)
    {
        var workerValidationException = new WorkerValidationException(exception);
        this.loggingBroker.LogError(workerValidationException);

        return workerValidationException;
    }

    private WorkerDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
    {
        var workerDependencyException = new WorkerDependencyException(exception);
        this.loggingBroker.LogCritical(workerDependencyException);

        return workerDependencyException;
    }

    private WorkerDependencyException CreateAndLogDependencyException(Xeption exception)
    {
        var workerDependencyException = new WorkerDependencyException(exception);
        this.loggingBroker.LogError(workerDependencyException);

        return workerDependencyException;
    }

    private WorkerServiceException CreateAndLogServiceException(Xeption exception)
    {
        var workerServiceException = new WorkerServiceException(exception);
        this.loggingBroker.LogError(workerServiceException);

        return workerServiceException;
    }
}