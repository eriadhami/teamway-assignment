using WorkPlan.Api.Models.Workers;
using WorkPlan.Api.Brokers.DateTimes;
using WorkPlan.Api.Brokers.Loggings;
using WorkPlan.Api.Brokers.Storages;

namespace WorkPlan.Api.Services.Foundations.Workers;

public class WorkerService : IWorkerService
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
    public ValueTask<Worker> AddWorkerAsync(Worker worker)
    {
        return storageBroker.InsertWorkerAsync(worker);
    }
}