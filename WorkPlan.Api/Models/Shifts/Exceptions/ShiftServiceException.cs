using Xeptions;

namespace WorkPlan.Api.Models.Shifts.Exceptions;

public class ShiftServiceException : Xeption
{
    public ShiftServiceException(Xeption innerException)
        : base(message: "Shift service error occurred, contact support.", innerException)
    { }
}