using WorkPlan.Api.Models.Plans;

namespace WorkPlan.Api.Services.Foundations.Plans;

public interface IPlanService
{
    ValueTask<Plan> AddPlanAsync(Plan plan);
    IQueryable<Plan> RetrieveAllPlans();
    ValueTask<Plan> RetrievePlanByIdAsync(Guid planId);
    ValueTask<Plan> ModifyPlanAsync(Plan plan);
    ValueTask<Plan> RemovePlanByIdAsync(Guid planId);
}