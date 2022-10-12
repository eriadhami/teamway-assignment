using Xeptions;

namespace WorkPlan.Api.Models.Plans.Exceptions;

public class FailedPlanStorageException : Xeption
{
    public FailedPlanStorageException(Exception innerException)
            : base(message: "Failed plan storage error occurred, please contact support.", innerException)
        { }
}