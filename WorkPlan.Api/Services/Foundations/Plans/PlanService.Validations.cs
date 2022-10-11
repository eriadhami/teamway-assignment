using WorkPlan.Api.Models.Plans;
using WorkPlan.Api.Models.Plans.Exceptions;

namespace WorkPlan.Api.Services.Foundations.Plans;

public partial class PlanService
{
    private void ValidatePlan(Plan plan)
    {
        ValidatePlanIsNotNull(plan);
    }

    private static void ValidatePlanIsNotNull(Plan plan)
    {
        if (plan is null)
        {
            throw new NullPlanException();
        }
    }
}