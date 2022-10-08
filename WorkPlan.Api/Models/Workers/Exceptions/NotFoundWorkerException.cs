using Xeptions;

namespace WorkPlan.Api.Models.Workers.Exceptions;

public class NotFoundWorkerException : Xeption
{
    public NotFoundWorkerException(Guid workerId)
        : base(message: $"Couldn't find worker with id: {workerId}.")
    { }
}