using Xeptions;

namespace WorkPlan.Api.Models.Shifts.Exceptions;

public class NullShiftException : Xeption
{
    public NullShiftException()
            : base(message: "Shift is null.") { }
}