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

    public ValueTask<Plan> AddPlanAsync(Plan plan) =>
        TryCatch(async () =>
        {
            ValidatePlan(plan);

            return await this.storageBroker.InsertPlanAsync(plan);
        });
    
    public IQueryable<Plan> RetrieveAllPlans() =>
        TryCatch(() => this.storageBroker.SelectAllPlans());

    public ValueTask<Plan> RetrievePlanByIdAsync(Guid planId) =>
        TryCatch(async () =>
        {
            ValidatePlanId(planId);

            Plan maybePlan = await this.storageBroker
                .SelectPlanByIdAsync(planId);
            
            ValidateStoragePlan(maybePlan, planId);

            return maybePlan;
        });
    
    public ValueTask<Plan> ModifyPlanAsync(Plan plan) =>
        TryCatch(async () =>
        {
            ValidatePlan(plan);

            var maybePlan =
                await this.storageBroker.SelectPlanByIdAsync(plan.ID);
            
            ValidateStoragePlan(maybePlan, plan.ID);

            return await this.storageBroker.UpdatePlanAsync(plan);
        });

    public ValueTask<Plan> RemovePlanByIdAsync(Guid planId)
    {
        throw new NotImplementedException();
    }
}