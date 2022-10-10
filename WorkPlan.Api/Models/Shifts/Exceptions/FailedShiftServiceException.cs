using Xeptions;

namespace WorkPlan.Api.Models.Shifts.Exceptions;
public class FailedShiftServiceException : Xeption
{
    public FailedShiftServiceException(Exception innerException)
        : base(message: "Failed shift service error occurred, please contact support.", innerException)
    { }
}