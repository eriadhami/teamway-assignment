using WorkPlan.Api.Models.Workers;
using WorkPlan.Api.Models.Workers.Exceptions;

namespace WorkPlan.Api.Services.Foundations.Workers;

public partial class WorkerService
{
    private void ValidateWorker(Worker worker)
    {
        ValidateWorkerIsNotNull(worker);

        Validate(
                (Rule: IsInvalid(worker.ID), Parameter: nameof(Worker.ID)),
                (Rule: IsInvalid(worker.FirstName), Parameter: nameof(Worker.FirstName)),
                (Rule: IsInvalid(worker.LastName), Parameter: nameof(Worker.LastName)));
    }

    private static void ValidateWorkerIsNotNull(Worker worker)
    {
        if (worker is null)
        {
            throw new NullWorkerException();
        }
    }

    private static void Validate(params (dynamic Rule, string Parameter)[] validations)
    {
        var invalidWorkerException =
            new InvalidWorkerException();

        foreach ((dynamic rule, string parameter) in validations)
        {
            if (rule.Condition)
            {
                invalidWorkerException.UpsertDataList(
                    key: parameter,
                    value: rule.Message);
            }
        }

        invalidWorkerException.ThrowIfContainsErrors();
    }

    private void ValidateStorageWorker(Worker maybeWorker, Guid workerId)
    {
        if (maybeWorker is null)
        {
            throw new NotFoundWorkerException(workerId);
        }
    }

    private void ValidateWorkerId(Guid workerId) =>
        Validate((Rule: IsInvalid(workerId), Parameter: nameof(Worker.ID)));

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
}