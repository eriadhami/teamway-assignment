using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WorkPlan.Api.Models.Plans;

namespace WorkPlan.Api.Brokers.Storages;

public partial class StorageBroker
{
    public DbSet<Plan> Plans { get; set; }

    public async ValueTask<Plan> InsertPlanAsync(Plan plan)
    {
        EntityEntry<Plan> planEntityEntry = await this.Plans.AddAsync(plan);
        await this.SaveChangesAsync();
        
        return planEntityEntry.Entity;
    }

    public IQueryable<Plan> SelectAllPlans() => this.Plans.AsQueryable();

    public async ValueTask<Plan> SelectPlanByIdAsync(Guid planId)
    {
        this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        return await Plans.FindAsync(planId);
    }

    public async ValueTask<Plan> UpdatePlanAsync(Plan plan)
    {
        EntityEntry<Plan> planEntityEntry = this.Plans.Update(plan);
        await this.SaveChangesAsync();

        return planEntityEntry.Entity;
    }

    public async ValueTask<Plan> DeletePlanAsync(Plan plan)
    {
        EntityEntry<Plan> planEntityEntry = this.Plans.Remove(plan);
        await this.SaveChangesAsync();

        return planEntityEntry.Entity;
    }
}