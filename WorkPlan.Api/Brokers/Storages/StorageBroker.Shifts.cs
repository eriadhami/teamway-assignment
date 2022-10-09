using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WorkPlan.Api.Models.Shifts;

namespace WorkPlan.Api.Brokers.Storages;

public partial class StorageBroker
{
    public DbSet<Shift> Shifts { get; set; }

    public async ValueTask<Shift> InsertShiftAsync(Shift shift)
    {
        EntityEntry<Shift> shiftEntityEntry = await this.Shifts.AddAsync(shift);
        return shiftEntityEntry.Entity;
    }

    public IQueryable<Shift> SelectAllShifts() => this.Shifts.AsQueryable();

    public async ValueTask<Shift> SelectShiftByIdAsync(Guid shiftId)
    {
        this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        return await Shifts.FindAsync(shiftId);
    }

    public async ValueTask<Shift> UpdateShiftAsync(Shift shift)
    {
        EntityEntry<Shift> shiftEntityEntry = this.Shifts.Update(shift);
        await this.SaveChangesAsync();

        return shiftEntityEntry.Entity;
    }

    public async ValueTask<Shift> DeleteShiftAsync(Shift shift)
    {
        EntityEntry<Shift> shiftEntityEntry = this.Shifts.Remove(shift);
        await this.SaveChangesAsync();

        return shiftEntityEntry.Entity;
    }
}