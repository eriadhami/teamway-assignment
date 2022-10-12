using Xeptions;

namespace WorkPlan.Api.Models.Plans.Exceptions;

public class PlanServiceException : Xeption
{
    public PlanServiceException(Xeption innerException)
        : base(message: "Plan service error occurred, contact support.", innerException)
    { }
}