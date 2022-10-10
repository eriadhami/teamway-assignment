using Xeptions;

namespace WorkPlan.Api.Models.Shifts.Exceptions;

public class InvalidShiftException : Xeption
{
    public InvalidShiftException()
            : base(message: "Invalid shift. Please correct the errors and try again."){ }
}