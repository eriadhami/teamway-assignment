using Xeptions;

namespace WorkPlan.Api.Models.Shifts.Exceptions
{
    public class ShiftDependencyValidationException : Xeption
    {
        public ShiftDependencyValidationException(Xeption innerException)
            : base(message: "Shift dependency validation occurred, please try again.", innerException)
        { }
    }
}