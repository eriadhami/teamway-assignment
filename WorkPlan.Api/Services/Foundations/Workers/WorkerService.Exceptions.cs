using Microsoft.Data.SqlClient;
using WorkPlan.Api.Models.Workers;
using WorkPlan.Api.Models.Workers.Exceptions;
using Xeptions;

namespace WorkPlan.Api.Services.Foundations.Workers;

public partial class WorkerService
{
    private delegate ValueTask<Worker> ReturningWorkerFunction();

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
        catch (SqlException sqlException)
        {
            var failedWorkerStorageException =
                new FailedWorkerStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedWorkerStorageException);
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
}