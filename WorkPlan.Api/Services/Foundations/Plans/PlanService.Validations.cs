using WorkPlan.Api.Models.Plans;
using WorkPlan.Api.Models.Plans.Exceptions;

namespace WorkPlan.Api.Services.Foundations.Plans;

public partial class PlanService
{
    private void ValidatePlan(Plan plan)
    {
        ValidatePlanIsNotNull(plan);

        Validate(
                (Rule: IsInvalid(plan.ID), Parameter: nameof(Plan.ID)),
                (Rule: IsInvalid(plan.WorkerID), Parameter: nameof(Plan.WorkerID)),
                (Rule: IsInvalid(plan.ShiftID), Parameter: nameof(Plan.ShiftID)),
                (Rule: IsInvalid(plan.Date), Parameter: nameof(Plan.Date)));
        
        ValidateDublicateWorkerOnSameDate(plan);
    }

    private static void ValidatePlanIsNotNull(Plan plan)
    {
        if (plan is null)
        {
            throw new NullPlanException();
        }
    }

    private static void Validate(params (dynamic Rule, string Parameter)[] validations)
    {
        var invalidPlanException =
            new InvalidPlanException();

        foreach ((dynamic rule, string parameter) in validations)
        {
            if (rule.Condition)
            {
                invalidPlanException.UpsertDataList(
                    key: parameter,
                    value: rule.Message);
            }
        }

        invalidPlanException.ThrowIfContainsErrors();
    }

    private void ValidatePlanId(Guid planId) =>
        Validate((Rule: IsInvalid(planId), Parameter: nameof(Plan.ID)));
    
    private void ValidateStoragePlan(Plan maybePlan, Guid planId)
    {
        if (maybePlan is null)
        {
            throw new NotFoundPlanException(planId);
        }
    }

    private void ValidateDublicateWorkerOnSameDate(Plan plan)
    {
        var storagePlans = this.storageBroker.SelectAllPlans();
        bool exist = storagePlans
            .Where(storagePlan => 
                storagePlan.WorkerID == plan.WorkerID && 
                storagePlan.Date == plan.Date)
            .Any();

        if (exist)
            {
                throw new DublicatePlanWorkerDateException();
            }
    }

    private static dynamic IsInvalid(Guid id) => new
    {
        Condition = id == Guid.Empty,
        Message = "Id is required"
    };

    private static dynamic IsInvalid(DateOnly date) => new
    {
        Condition = date == default,
        Message = "Date is required"
    };
}