using WorkPlan.Api.Models.Workers;
using WorkPlan.Api.Brokers.DateTimes;
using WorkPlan.Api.Brokers.Loggings;
using WorkPlan.Api.Brokers.Storages;

namespace WorkPlan.Api.Services.Foundations.Workers;

public partial class WorkerService : IWorkerService
{
    private readonly IStorageBroker storageBroker;
    private readonly IDateTimeBroker dateTimeBroker;
    private readonly ILoggingBroker loggingBroker;

    public WorkerService(
        IStorageBroker storageBroker,
        IDateTimeBroker dateTimeBroker,
        ILoggingBroker loggingBroker)
    {
        this.storageBroker = storageBroker;
        this.dateTimeBroker = dateTimeBroker;
        this.loggingBroker = loggingBroker;
    }
    public ValueTask<Worker> AddWorkerAsync(Worker worker) =>
        TryCatch(async () =>
        {
            ValidateWorker(worker);

            return await this.storageBroker.InsertWorkerAsync(worker);
        });
    
    public IQueryable<Worker> RetrieveAllWorkers() =>
            TryCatch(() => this.storageBroker.SelectAllWorkers());

    public ValueTask<Worker> RetrieveWorkerByIdAsync(Guid workerId) =>
        TryCatch(async () =>
        {
            ValidateWorkerId(workerId);

            Worker maybeWorker = await this.storageBroker
                .SelectWorkerByIdAsync(workerId);
            
            ValidateStorageWorker(maybeWorker, workerId);

            return maybeWorker;
        });
    
    public ValueTask<Worker> ModifyWorkerAsync(Worker worker) =>
        TryCatch(async () =>
        {
            ValidateWorker(worker);

            var maybeWorker =
                await this.storageBroker.SelectWorkerByIdAsync(worker.ID);
            
            ValidateStorageWorker(maybeWorker, worker.ID);

            return await this.storageBroker.UpdateWorkerAsync(worker);
        });

    public async ValueTask<Worker> RemoveWorkerByIdAsync(Guid workerId)
    {
        var maybeWorker = await this.storageBroker.SelectWorkerByIdAsync(workerId);
        return await this.storageBroker.DeleteWorkerAsync(maybeWorker);
    }
}