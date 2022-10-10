using WorkPlan.Api.Models.Shifts;

namespace WorkPlan.Api.Brokers.Storages;

public partial interface IStorageBroker
{
    public ValueTask<Shift> InsertShiftAsync(Shift shift);
    public IQueryable<Shift> SelectAllShifts();
    public ValueTask<Shift> SelectShiftByIdAsync(Guid shiftId);
    public ValueTask<Shift> UpdateShiftAsync(Shift shift);
    public ValueTask<Shift> DeleteShiftAsync(Shift shift);
}