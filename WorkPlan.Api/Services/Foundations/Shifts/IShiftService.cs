using WorkPlan.Api.Models.Shifts;

namespace WorkPlan.Api.Services.Foundations.Shifts;

public interface IShiftService
{
    ValueTask<Shift> AddShiftAsync(Shift shift);
    IQueryable<Shift> RetrieveAllShifts();
}