using Xeptions;

namespace WorkPlan.Api.Models.Shifts.Exceptions;

public class FailedShiftStorageException : Xeption
{
    public FailedShiftStorageException(Exception innerException)
            : base(message: "Failed shift storage error occurred, please contact support.", innerException)
        { }
}