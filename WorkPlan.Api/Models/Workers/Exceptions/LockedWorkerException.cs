using Xeptions;

namespace WorkPlan.Api.Models.Workers.Exceptions;
public class LockedWorkerException : Xeption
{
    public LockedWorkerException(Exception innerException)
        : base(message: "Locked worker record exception, please try again later", innerException)
    { }
}