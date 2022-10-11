using Xeptions;

namespace WorkPlan.Api.Models.Plans.Exceptions;

public class PlanValidationException : Xeption
{
    public PlanValidationException(Xeption innerException)
        : base(message: "Plan validation errors occurred, please try again.",
                innerException){ }
}