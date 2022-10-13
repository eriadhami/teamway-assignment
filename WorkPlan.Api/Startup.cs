using Microsoft.OpenApi.Models;
using WorkPlan.Api.Brokers.DateTimes;
using WorkPlan.Api.Brokers.Loggings;
using WorkPlan.Api.Brokers.Storages;
using WorkPlan.Api.Helpers;
using WorkPlan.Api.Services.Foundations.Plans;
using WorkPlan.Api.Services.Foundations.Shifts;
using WorkPlan.Api.Services.Foundations.Workers;

namespace WorkPlan.Api;

public class Startup
{
    public Startup(IConfiguration configuration) =>
        Configuration = configuration;

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging();
        
        services.AddControllers()
                .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new TimeOnlyJsonConverter());
                    });

        services.AddDbContext<StorageBroker>();
        AddBrokers(services);
        AddServices(services);

        services.AddSwaggerGen(options =>
        {
            var openApiInfo = new OpenApiInfo
            {
                Title = "WorkPlan.Api",
                Version = "v1"
            };

            options.SwaggerDoc(
                name: "v1",
                info: openApiInfo);
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(
                    url: "/swagger/v1/swagger.json",
                    name: "WorkPlan.Api v1");
            });
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }

    private static void AddBrokers(IServiceCollection services)
    {
        services.AddScoped<IStorageBroker, StorageBroker>();
        services.AddTransient<ILoggingBroker, LoggingBroker>();
        services.AddTransient<IDateTimeBroker, DateTimeBroker>();
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddTransient<IWorkerService, WorkerService>();
        services.AddTransient<IShiftService, ShiftService>();
        services.AddTransient<IPlanService, PlanService>();
    }
}