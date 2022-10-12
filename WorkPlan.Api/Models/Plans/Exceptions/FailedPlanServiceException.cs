using Xeptions;

namespace WorkPlan.Api.Models.Plans.Exceptions;
public class FailedPlanServiceException : Xeption
{
    public FailedPlanServiceException(Exception innerException)
        : base(message: "Failed plan service error occurred, please contact support.", innerException)
    { }
}