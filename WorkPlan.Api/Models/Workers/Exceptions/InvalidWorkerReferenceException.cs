using Xeptions;

namespace WorkPlan.Api.Models.Workers.Exceptions;
public class InvalidWorkerReferenceException : Xeption
{
    public InvalidWorkerReferenceException(Exception innerException)
        : base(message: "Invalid worker reference error occurred.", innerException)
    { }
}