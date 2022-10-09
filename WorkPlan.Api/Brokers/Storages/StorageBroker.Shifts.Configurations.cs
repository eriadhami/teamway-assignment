using Microsoft.EntityFrameworkCore;
using WorkPlan.Api.Helpers;

namespace WorkPlan.Api.Brokers.Storages;

public partial class StorageBroker
{
    private static void AddShiftsConfigurations(
        ModelConfigurationBuilder modelConfigurationBuilder)
    {
        modelConfigurationBuilder.Properties<TimeOnly>()
            .HaveConversion<TimeOnlyConverter>()
            .HaveColumnType("time");
    }
}