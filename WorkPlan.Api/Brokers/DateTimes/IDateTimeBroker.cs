namespace WorkPlan.Api.Brokers.DateTimes;

public interface IDateTimeBroker
{
    DateTimeOffset GetCurrentDateTimeOffset();
}