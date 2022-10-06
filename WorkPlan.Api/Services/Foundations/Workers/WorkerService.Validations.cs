using WorkPlan.Api.Models.Workers;
using WorkPlan.Api.Models.Workers.Exceptions;

namespace WorkPlan.Api.Services.Foundations.Workers;

public partial class WorkerService
{
    private void ValidateWorkerOnAdd(Worker worker)
    {
        ValidateWorkerIsNotNull(worker);
    }

    private static void ValidateWorkerIsNotNull(Worker worker)
    {
        if (worker is null)
        {
            throw new NullWorkerException();
        }
    }
}