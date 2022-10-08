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
            ValidateWorkerOnAdd(worker);

            return await this.storageBroker.InsertWorkerAsync(worker);
        });
    
    public IQueryable<Worker> RetrieveAllWorkers() =>
            TryCatch(() => this.storageBroker.SelectAllWorkers());

    public async ValueTask<Worker> RetrieveWorkerByIdAsync(Guid workerId)
    {
        return await this.storageBroker.SelectWorkerByIdAsync(workerId);
    }
}