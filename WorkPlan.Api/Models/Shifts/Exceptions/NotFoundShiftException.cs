using Xeptions;

namespace WorkPlan.Api.Models.Shifts.Exceptions;

public class NotFoundShiftException : Xeption
{
    public NotFoundShiftException(Guid shiftId)
        : base(message: $"Couldn't find shift with id: {shiftId}.")
    { }
}