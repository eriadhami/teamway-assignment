using WorkPlan.Api.Models.Shifts;
using WorkPlan.Api.Models.Shifts.Exceptions;

namespace WorkPlan.Api.Services.Foundations.Shifts;

public partial class ShiftService
{
    private void ValidateShift(Shift shift)
    {
        ValidateShiftIsNotNull(shift);

        Validate(
                (Rule: IsInvalid(shift.ID), Parameter: nameof(Shift.ID)),
                (Rule: IsInvalid(shift.Name), Parameter: nameof(Shift.Name)),
                
                (Rule: IsSameOrBefore(
                    firstTime: shift.EndHour,
                    secondTime: shift.StartHour,
                    secondTimeName: nameof(Shift.StartHour)),
                Parameter: nameof(Shift.EndHour)));
    }

    private static void ValidateShiftIsNotNull(Shift shift)
    {
        if (shift is null)
        {
            throw new NullShiftException();
        }
    }

    private static void Validate(params (dynamic Rule, string Parameter)[] validations)
    {
        var invalidShiftException =
            new InvalidShiftException();

        foreach ((dynamic rule, string parameter) in validations)
        {
            if (rule.Condition)
            {
                invalidShiftException.UpsertDataList(
                    key: parameter,
                    value: rule.Message);
            }
        }

        invalidShiftException.ThrowIfContainsErrors();
    }

    private void ValidateShiftId(Guid shiftId) =>
        Validate((Rule: IsInvalid(shiftId), Parameter: nameof(Shift.ID)));

    private void ValidateStorageShift(Shift maybeShift, Guid shiftId)
    {
        throw new NotImplementedException();
    }

    private static dynamic IsInvalid(Guid id) => new
    {
        Condition = id == Guid.Empty,
        Message = "Id is required"
    };

    private static dynamic IsInvalid(string text) => new
    {
        Condition = string.IsNullOrWhiteSpace(text),
        Message = "Text is required"
    };

    private static dynamic IsSameOrBefore(TimeOnly firstTime, TimeOnly secondTime, string secondTimeName) => new
    {
        Condition = firstTime <= secondTime,
        Message = $"Time is the same as or before the {secondTimeName}"
    };
}