using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WorkPlan.Api.Models.Shifts;
using WorkPlan.Api.Models.Shifts.Exceptions;
using Xeptions;

namespace WorkPlan.Api.Services.Foundations.Shifts;

public partial class ShiftService
{
    private delegate ValueTask<Shift> ReturningShiftFunction();
    private delegate IQueryable<Shift> ReturningShiftsFunction();

    private async ValueTask<Shift> TryCatch(ReturningShiftFunction returningShiftFunction)
    {
        try
        {
            return await returningShiftFunction();
        }
        catch (NullShiftException nullShiftException)
        {
            throw CreateAndLogValidationException(nullShiftException);
        }
        catch (InvalidShiftException invalidShiftException)
        {
            throw CreateAndLogValidationException(invalidShiftException);
        }
        catch (SqlException sqlException)
        {
            var failedShiftStorageException =
                new FailedShiftStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedShiftStorageException);
        }
        catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
        {
            var invalidShiftReferenceException =
                new InvalidShiftReferenceException(foreignKeyConstraintConflictException);

            throw CreateAndLogDependencyException(invalidShiftReferenceException);
        }
        catch (DbUpdateException databaseUpdateException)
        {
            var failedStorageShiftException =
                new FailedShiftStorageException(databaseUpdateException);

            throw CreateAndLogDependencyException(failedStorageShiftException);
        }
        catch (Exception serviceException)
        {
            var failedServiceShiftException =
                new FailedShiftServiceException(serviceException);

            throw CreateAndLogServiceException(failedServiceShiftException);
        }
    }

    private IQueryable<Shift> TryCatch(ReturningShiftsFunction returningShiftsFunction)
    {
        try
        {
            return returningShiftsFunction();
        }
        catch (SqlException sqlException)
        {
            var failedShiftStorageException =
                new FailedShiftStorageException(sqlException);

            throw CreateAndLogCriticalDependencyException(failedShiftStorageException);
        }
        catch (Exception serviceException)
        {
            var failedServiceShiftException =
                new FailedShiftServiceException(serviceException);

            throw CreateAndLogServiceException(failedServiceShiftException);
        }
    }

    private ShiftValidationException CreateAndLogValidationException(Xeption exception)
    {
        var shiftValidationException = new ShiftValidationException(exception);
        this.loggingBroker.LogError(shiftValidationException);

        return shiftValidationException;
    }

    private ShiftDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
    {
        var shiftDependencyException = new ShiftDependencyException(exception);
        this.loggingBroker.LogCritical(shiftDependencyException);

        return shiftDependencyException;
    }

    private ShiftDependencyException CreateAndLogDependencyException(Xeption exception)
    {
        var shiftDependencyException = new ShiftDependencyException(exception);
        this.loggingBroker.LogError(shiftDependencyException);

        return shiftDependencyException;
    }

    private ShiftServiceException CreateAndLogServiceException(Xeption exception)
    {
        var shiftServiceException = new ShiftServiceException(exception);
        this.loggingBroker.LogError(shiftServiceException);

        return shiftServiceException;
    }
}