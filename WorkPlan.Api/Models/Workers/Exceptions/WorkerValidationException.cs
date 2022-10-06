using Xeptions;

namespace WorkPlan.Api.Models.Workers.Exceptions;

public class WorkerValidationException : Xeption
{
    public WorkerValidationException(Xeption innerException)
        : base(message: "Worker validation errors occurred, please try again.",
                innerException){ }
}