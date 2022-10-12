using Xeptions;

namespace WorkPlan.Api.Models.Shifts.Exceptions;

public class AlreadyExistsShiftException : Xeption
{
    public AlreadyExistsShiftException(Exception innerException)
        : base(message: "Shift with the same id already exists.", innerException)
    { }
}