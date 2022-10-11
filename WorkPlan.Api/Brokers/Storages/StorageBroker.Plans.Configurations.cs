using Microsoft.EntityFrameworkCore;
using WorkPlan.Api.Helpers;

namespace WorkPlan.Api.Brokers.Storages;

public partial class StorageBroker
{
    private static void AddPlansConfigurations(
        ModelConfigurationBuilder modelConfigurationBuilder)
    {
        modelConfigurationBuilder.Properties<DateOnly>()
            .HaveConversion<DateOnlyConverter>()
            .HaveColumnType("date");
    }
}