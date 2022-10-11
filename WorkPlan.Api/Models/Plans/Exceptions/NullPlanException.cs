using Xeptions;

namespace WorkPlan.Api.Models.Plans.Exceptions;

public class NullPlanException : Xeption
{
    public NullPlanException()
            : base(message: "Plan is null.") { }
}