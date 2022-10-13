using Xeptions;

namespace WorkPlan.Api.Models.Plans.Exceptions;

public class AlreadyExistsPlanException : Xeption
{
    public AlreadyExistsPlanException(Exception innerException)
        : base(message: "Plan with the same id already exists.", innerException)
    { }
}