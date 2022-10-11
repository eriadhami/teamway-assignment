using WorkPlan.Api.Models.Plans;

namespace WorkPlan.Api.Brokers.Storages;

public partial interface IStorageBroker
{
    public ValueTask<Plan> InsertPlanAsync(Plan plan);
    public IQueryable<Plan> SelectAllPlans();
    public ValueTask<Plan> SelectPlanByIdAsync(Guid planId);
    public ValueTask<Plan> UpdatePlanAsync(Plan plan);
    public ValueTask<Plan> DeletePlanAsync(Plan plan);
}