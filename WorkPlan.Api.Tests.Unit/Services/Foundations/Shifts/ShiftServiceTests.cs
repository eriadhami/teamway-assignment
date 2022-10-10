using System.Linq.Expressions;
using Moq;
using Tynamix.ObjectFiller;
using WorkPlan.Api.Brokers.Loggings;
using WorkPlan.Api.Brokers.Storages;
using WorkPlan.Api.Models.Shifts;
using WorkPlan.Api.Services.Foundations.Shifts;
using Xeptions;

namespace WorkPlan.Api.Tests.Unit.Services.Foundations.Shifts;

public partial class ShiftServiceTests
{
    private readonly Mock<IStorageBroker> storageBrokerMock;
    private readonly Mock<ILoggingBroker> loggingBrokerMock;
    private readonly IShiftService shiftService;

    public ShiftServiceTests()
    {
        this.storageBrokerMock = new Mock<IStorageBroker>();
        this.loggingBrokerMock = new Mock<ILoggingBroker>();

        this.shiftService = new ShiftService(
            storageBroker: this.storageBrokerMock.Object,
            loggingBroker: this.loggingBrokerMock.Object);
    }
    
    private static Shift CreateRandomShift() =>
        CreateShiftFiller().Create();

    private static Filler<Shift> CreateShiftFiller()
    {
        var filler = new Filler<Shift>();

        filler.Setup()
            .OnType<TimeOnly>().Use(TimeOnly.MinValue);

        return filler;
    }

    private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
        actualException => actualException.SameExceptionAs(expectedException);
}