using Xeptions;

namespace WorkPlan.Api.Models.Workers.Exceptions;

public class AlreadyExistsWorkerException : Xeption
{
    public AlreadyExistsWorkerException(Exception innerException)
        : base(message: "Worker with the same id already exists.", innerException)
    { }
}