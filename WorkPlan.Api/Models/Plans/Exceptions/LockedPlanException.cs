using Xeptions;

namespace WorkPlan.Api.Models.Plans.Exceptions;
public class LockedPlanException : Xeption
{
    public LockedPlanException(Exception innerException)
        : base(message: "Locked plan record exception, please try again later", innerException)
    { }
}