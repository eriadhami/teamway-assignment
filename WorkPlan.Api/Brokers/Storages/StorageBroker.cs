using EFxceptions;
using Microsoft.EntityFrameworkCore;

namespace WorkPlan.Api.Brokers.Storages;

public partial class StorageBroker : EFxceptionsContext, IStorageBroker
{
    private readonly IConfiguration configuration;

    public StorageBroker(IConfiguration configuration)
    {
        this.configuration = configuration;
        this.Database.Migrate();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = this.configuration
            .GetConnectionString(name: "DefaultConnection");

        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SetPlanReferences(modelBuilder);
        }

    protected override void ConfigureConventions(ModelConfigurationBuilder modelConfigurationBuilder)
    {
        AddShiftsConfigurations(modelConfigurationBuilder);
        AddPlansConfigurations(modelConfigurationBuilder);
        base.ConfigureConventions(modelConfigurationBuilder);
    }
}