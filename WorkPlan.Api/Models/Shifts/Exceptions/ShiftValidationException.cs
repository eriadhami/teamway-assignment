using Xeptions;

namespace WorkPlan.Api.Models.Shifts.Exceptions;

public class ShiftValidationException : Xeption
{
    public ShiftValidationException(Xeption innerException)
        : base(message: "Shift validation errors occurred, please try again.",
                innerException){ }
}