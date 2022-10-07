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
    
    private static Worker CreateRandomWorker() =>
        CreateWorkerFiller().Create();

    private static Filler<Worker> CreateWorkerFiller() => new Filler<Worker>();
    
    private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
        actualException => actualException.SameExceptionAs(expectedException);

    private static SqlException GetSqlException() =>
        (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

    private static string GetRandomMessage() =>
        new MnemonicString(wordCount: GetRandomNumber()).GetValue();

    private static int GetRandomNumber() =>
        new IntRange(min: 2, max: 10).GetValue();
}