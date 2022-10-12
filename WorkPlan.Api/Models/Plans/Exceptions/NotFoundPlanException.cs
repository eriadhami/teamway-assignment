using Xeptions;

namespace WorkPlan.Api.Models.Plans.Exceptions;

public class NotFoundPlanException : Xeption
{
    public NotFoundPlanException(Guid planId)
        : base(message: $"Couldn't find plan with id: {planId}.")
    { }
}