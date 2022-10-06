using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using WorkPlan.Api.Brokers.DateTimes;
using WorkPlan.Api.Brokers.Loggings;
using WorkPlan.Api.Brokers.Storages;
using WorkPlan.Api.Models.Workers;
using WorkPlan.Api.Services.Foundations.Workers;
using Xeptions;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Workers;

public partial class WorkerServiceTests
{
    private readonly Mock<IStorageBroker> storageBrokerMock;
    private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
    private readonly Mock<ILoggingBroker> loggingBrokerMock;
    private readonly IWorkerService workerService;

    public WorkerServiceTests()
    {
        this.storageBrokerMock = new Mock<IStorageBroker>();
        this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
        this.loggingBrokerMock = new Mock<ILoggingBroker>();

        this.workerService = new WorkerService(
            storageBroker: this.storageBrokerMock.Object,
            dateTimeBroker: this.dateTimeBrokerMock.Object,
            loggingBroker: this.loggingBrokerMock.Object);
    }

    private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

    private static Worker CreateRandomWorker(DateTimeOffset dates) =>
            CreateWorkerFiller(dates).Create();

    private static Filler<Worker> CreateWorkerFiller(DateTimeOffset dates)
        {
            var filler = new Filler<Worker>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates);

            return filler;
        }
    
    private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

    private static SqlException GetSqlException() =>
            (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));
}