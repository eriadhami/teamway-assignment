using Xeptions;

namespace WorkPlan.Api.Models.Shifts.Exceptions;
public class LockedShiftException : Xeption
{
    public LockedShiftException(Exception innerException)
        : base(message: "Locked shift record exception, please try again later", innerException)
    { }
}