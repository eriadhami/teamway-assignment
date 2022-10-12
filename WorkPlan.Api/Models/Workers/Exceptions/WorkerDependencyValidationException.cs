using Xeptions;

namespace WorkPlan.Api.Models.Workers.Exceptions
{
    public class WorkerDependencyValidationException : Xeption
    {
        public WorkerDependencyValidationException(Xeption innerException)
            : base(message: "Worker dependency validation occurred, please try again.", innerException)
        { }
    }
}