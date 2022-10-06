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
    }

    private WorkerValidationException CreateAndLogValidationException(Xeption exception)
    {
        var workerValidationException = new WorkerValidationException(exception);
        this.loggingBroker.LogError(workerValidationException);

        return workerValidationException;
    }
}