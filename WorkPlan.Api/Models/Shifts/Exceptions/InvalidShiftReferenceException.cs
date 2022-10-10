using Xeptions;

namespace WorkPlan.Api.Models.Shifts.Exceptions;
public class InvalidShiftReferenceException : Xeption
{
    public InvalidShiftReferenceException(Exception innerException)
        : base(message: "Invalid shift reference error occurred.", innerException)
    { }
}