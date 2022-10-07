using Xeptions;

namespace WorkPlan.Api.Models.Workers.Exceptions;
public class FailedWorkerServiceException : Xeption
{
    public FailedWorkerServiceException(Exception innerException)
        : base(message: "Failed worker service error occurred, please contact support.", innerException)
    { }
}