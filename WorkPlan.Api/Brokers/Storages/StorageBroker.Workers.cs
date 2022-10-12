using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using WorkPlan.Api.Models.Workers;

namespace WorkPlan.Api.Brokers.Storages;

public partial class StorageBroker
{
    public DbSet<Worker> Workers { get; set; }

    public async ValueTask<Worker> InsertWorkerAsync(Worker worker)
    {
        EntityEntry<Worker> workerEntityEntry = await this.Workers.AddAsync(worker);
        await this.SaveChangesAsync();
        
        return workerEntityEntry.Entity;
    }

    public IQueryable<Worker> SelectAllWorkers() => this.Workers.AsQueryable();

    public async ValueTask<Worker> SelectWorkerByIdAsync(Guid workerId)
    {
        this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        return await Workers.FindAsync(workerId);
    }

    public async ValueTask<Worker> UpdateWorkerAsync(Worker worker)
    {
        EntityEntry<Worker> workerEntityEntry = this.Workers.Update(worker);
        await this.SaveChangesAsync();

        return workerEntityEntry.Entity;
    }

    public async ValueTask<Worker> DeleteWorkerAsync(Worker worker)
    {
        EntityEntry<Worker> workerEntityEntry = this.Workers.Remove(worker);
        await this.SaveChangesAsync();

        return workerEntityEntry.Entity;
    }
}