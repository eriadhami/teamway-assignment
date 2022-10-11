using WorkPlan.Api.Models.Plans;
using WorkPlan.Api.Brokers.Loggings;
using WorkPlan.Api.Brokers.Storages;

namespace WorkPlan.Api.Services.Foundations.Plans;

public partial class PlanService : IPlanService
{
    private readonly IStorageBroker storageBroker;
    private readonly ILoggingBroker loggingBroker;

    public PlanService(
        IStorageBroker storageBroker,
        ILoggingBroker loggingBroker)
    {
        this.storageBroker = storageBroker;
        this.loggingBroker = loggingBroker;
    }

    public ValueTask<Plan> AddPlanAsync(Plan plan)
    {
        throw new NotImplementedException();
    }
}