using WorkPlan.Api.Models.Shifts;
using WorkPlan.Api.Models.Shifts.Exceptions;

namespace WorkPlan.Api.Services.Foundations.Shifts;

public partial class ShiftService
{
    private void ValidateShift(Shift shift)
    {
        ValidateShiftIsNotNull(shift);
    }

    private static void ValidateShiftIsNotNull(Shift shift)
    {
        throw new NotImplementedException();
    }
}