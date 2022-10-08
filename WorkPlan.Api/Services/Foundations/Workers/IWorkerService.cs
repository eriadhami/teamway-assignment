using WorkPlan.Api.Models.Workers;

namespace WorkPlan.Api.Services.Foundations.Workers;

public interface IWorkerService
{
    ValueTask<Worker> AddWorkerAsync(Worker worker);
    IQueryable<Worker> RetrieveAllWorkers();
    ValueTask<Worker> RetrieveWorkerByIdAsync(Guid workerId);
}