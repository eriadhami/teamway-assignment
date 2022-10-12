using Xeptions;

namespace WorkPlan.Api.Models.Plans.Exceptions;

public class PlanDependencyException : Xeption
{
    public PlanDependencyException(Exception innerException)
            : base(message: "Plan dependency error occurred, contact support.", innerException)
        { }
}