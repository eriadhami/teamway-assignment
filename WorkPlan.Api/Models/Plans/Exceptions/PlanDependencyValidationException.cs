using Xeptions;

namespace WorkPlan.Api.Models.Plans.Exceptions
{
    public class PlanDependencyValidationException : Xeption
    {
        public PlanDependencyValidationException(Xeption innerException)
            : base(message: "Plan dependency validation occurred, please try again.", innerException)
        { }
    }
}