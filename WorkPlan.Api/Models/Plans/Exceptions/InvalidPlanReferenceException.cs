using Xeptions;

namespace WorkPlan.Api.Models.Plans.Exceptions;
public class InvalidPlanReferenceException : Xeption
{
    public InvalidPlanReferenceException(Exception innerException)
        : base(message: "Invalid plan reference error occurred.", innerException)
    { }
}