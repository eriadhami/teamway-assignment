using Xeptions;

namespace WorkPlan.Api.Models.Workers.Exceptions;

public class InvalidWorkerException : Xeption
{
    public InvalidWorkerException()
            : base(message: "Invalid worker. Please correct the errors and try again."){ }
}