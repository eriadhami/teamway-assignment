using Xeptions;

namespace WorkPlan.Api.Models.Shifts.Exceptions;

public class ShiftDependencyException : Xeption
{
    public ShiftDependencyException(Exception innerException)
            : base(message: "Shift dependency error occurred, contact support.", innerException)
        { }
}