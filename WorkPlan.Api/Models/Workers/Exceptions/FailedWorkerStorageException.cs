using Xeptions;

namespace WorkPlan.Api.Models.Workers.Exceptions;

public class FailedWorkerStorageException : Xeption
{
    public FailedWorkerStorageException(Exception innerException)
            : base(message: "Failed worker storage error occurred, please contact support.", innerException)
        { }
}