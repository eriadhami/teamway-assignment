using WorkPlan.Api.Models.Shifts;
using WorkPlan.Api.Brokers.Loggings;
using WorkPlan.Api.Brokers.Storages;

namespace WorkPlan.Api.Services.Foundations.Shifts;

public partial class ShiftService : IShiftService
{
    private readonly IStorageBroker storageBroker;
    private readonly ILoggingBroker loggingBroker;

    public ShiftService(
        IStorageBroker storageBroker,
        ILoggingBroker loggingBroker)
    {
        this.storageBroker = storageBroker;
        this.loggingBroker = loggingBroker;
    }
    public ValueTask<Shift> AddShiftAsync(Shift shift) =>
        TryCatch(async () =>
        {
            ValidateShift(shift);

            return await this.storageBroker.InsertShiftAsync(shift);
        });
    
    public IQueryable<Shift> RetrieveAllShifts() =>
        TryCatch(() => this.storageBroker.SelectAllShifts());

    public ValueTask<Shift> RetrieveShiftByIdAsync(Guid shiftId) =>
        TryCatch(async () =>
        {
            Shift maybeShift = await this.storageBroker
                .SelectShiftByIdAsync(shiftId);

            return maybeShift;
        });
}