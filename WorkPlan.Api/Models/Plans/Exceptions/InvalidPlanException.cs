using Xeptions;

namespace WorkPlan.Api.Models.Plans.Exceptions;

public class InvalidPlanException : Xeption
{
    public InvalidPlanException()
            : base(message: "Invalid plan. Please correct the errors and try again."){ }
}