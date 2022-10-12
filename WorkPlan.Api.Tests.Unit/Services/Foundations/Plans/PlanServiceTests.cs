using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using WorkPlan.Api.Brokers.Loggings;
using WorkPlan.Api.Brokers.Storages;
using WorkPlan.Api.Models.Plans;
using WorkPlan.Api.Services.Foundations.Plans;
using Xeptions;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Plans;

public partial class PlanServiceTests
{
    private readonly Mock<IStorageBroker> storageBrokerMock;
    private readonly Mock<ILoggingBroker> loggingBrokerMock;
    private readonly IPlanService planService;

    public PlanServiceTests()
    {
        this.storageBrokerMock = new Mock<IStorageBroker>();
        this.loggingBrokerMock = new Mock<ILoggingBroker>();

        this.planService = new PlanService(
            storageBroker: this.storageBrokerMock.Object,
            loggingBroker: this.loggingBrokerMock.Object);
    }
    
    private static Plan CreateRandomPlan() =>
        CreatePlanFiller().Create();

    private static IQueryable<Plan> CreateRandomPlans()
    {
        return CreatePlanFiller()
                .Create(GetRandomNumber())
                .AsQueryable();
    }

    private static Filler<Plan> CreatePlanFiller()
    {
        var filler = new Filler<Plan>();

        filler.Setup()
            .OnType<DateOnly>().Use(DateOnly.MinValue)
            .OnProperty(plan => plan.Worker).IgnoreIt()
            .OnProperty(plan => plan.Shift).IgnoreIt();

        return filler;
    }

    private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
        actualException => actualException.SameExceptionAs(expectedException);

    private static SqlException GetSqlException() =>
        (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));
    
    private static string GetRandomMessage() =>
        new MnemonicString(wordCount: GetRandomNumber()).GetValue();

    private static int GetRandomNumber() =>
            new IntRange(min: 1, max: 23).GetValue();
}