using Xeptions;

namespace WorkPlan.Api.Models.Workers.Exceptions;

public class WorkerDependencyException : Xeption
{
    public WorkerDependencyException(Exception innerException)
            : base(message: "Worker dependency error occurred, contact support.", innerException)
        { }
}