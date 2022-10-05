using WorkPlan.Api.Models.Workers;

namespace WorkPlan.Api.Brokers.Storages;

public partial interface IStorageBroker
{
    public ValueTask<Worker> InsertWorkerAsync(Worker worker);
    public IQueryable<Worker> SelectAllWorkers();
    public ValueTask<Worker> SelectWorkerByIdAsync(Guid workerId);
    public ValueTask<Worker> UpdateWorkerAsync(Worker worker);
    public ValueTask<Worker> DeleteWorkerAsync(Worker worker);
}