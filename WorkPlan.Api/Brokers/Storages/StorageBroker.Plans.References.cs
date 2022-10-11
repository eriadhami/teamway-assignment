using Microsoft.EntityFrameworkCore;
using WorkPlan.Api.Models.Plans;

namespace WorkPlan.Api.Brokers.Storages;

public partial class StorageBroker
{
    private static void SetPlanReferences(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Plan>()
            .HasOne(plan => plan.Worker)
            .WithMany(worker => worker.Plans)
            .HasForeignKey(plan => plan.WorkerID)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Plan>()
            .HasOne(plan => plan.Shift)
            .WithMany(shift => shift.Plans)
            .HasForeignKey(plan => plan.ShiftID)
            .OnDelete(DeleteBehavior.NoAction);
    }
}