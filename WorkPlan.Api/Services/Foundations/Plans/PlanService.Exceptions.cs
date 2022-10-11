using WorkPlan.Api.Models.Plans;
using WorkPlan.Api.Models.Plans.Exceptions;
using Xeptions;

namespace WorkPlan.Api.Services.Foundations.Plans;

public partial class PlanService
{
    private delegate ValueTask<Plan> ReturningPlanFunction();

    private async ValueTask<Plan> TryCatch(ReturningPlanFunction returningPlanFunction)
    {
        try
        {
            return await returningPlanFunction();
        }
        catch (NullPlanException nullPlanException)
        {
            throw CreateAndLogValidationException(nullPlanException);
        }
        catch (InvalidPlanException invalidPlanException)
        {
            throw CreateAndLogValidationException(invalidPlanException);
        }
    }

    private PlanValidationException CreateAndLogValidationException(Xeption exception)
    {
        var planValidationException = new PlanValidationException(exception);
        this.loggingBroker.LogError(planValidationException);

        return planValidationException;
    }
}