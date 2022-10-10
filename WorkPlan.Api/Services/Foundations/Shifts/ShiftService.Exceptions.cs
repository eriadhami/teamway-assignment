using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WorkPlan.Api.Models.Shifts;
using WorkPlan.Api.Models.Shifts.Exceptions;
using Xeptions;

namespace WorkPlan.Api.Services.Foundations.Shifts;

public partial class ShiftService
{
    private delegate ValueTask<Shift> ReturningShiftFunction();

    private async ValueTask<Shift> TryCatch(ReturningShiftFunction returningShiftFunction)
    {
        try
        {
            return await returningShiftFunction();
        }
        catch (NullShiftException nullShiftException)
        {
            throw CreateAndLogValidationException(nullShiftException);
        }
    }

    private ShiftValidationException CreateAndLogValidationException(Xeption exception)
    {
        var shiftValidationException = new ShiftValidationException(exception);
        this.loggingBroker.LogError(shiftValidationException);

        return shiftValidationException;
    }
}