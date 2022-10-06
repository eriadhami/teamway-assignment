using Xeptions;

namespace WorkPlan.Api.Models.Workers.Exceptions;

public class NullWorkerException : Xeption
{
    public NullWorkerException()
            : base(message: "Worker is null.") { }
}