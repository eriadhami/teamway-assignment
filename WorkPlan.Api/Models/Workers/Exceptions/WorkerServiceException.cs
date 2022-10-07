using Xeptions;

namespace WorkPlan.Api.Models.Workers.Exceptions;

public class WorkerServiceException : Xeption
{
    public WorkerServiceException(Xeption innerException)
        : base(message: "Worker service error occurred, contact support.", innerException)
    { }
}