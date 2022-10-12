using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using WorkPlan.Api.Models.Shifts;
using WorkPlan.Api.Models.Shifts.Exceptions;
using WorkPlan.Api.Services.Foundations.Shifts;

namespace WorkPlan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShiftsController : RESTFulController
{
    private readonly IShiftService shiftService;

    public ShiftsController(IShiftService shiftService) =>
        this.shiftService = shiftService;

    [HttpPost]
    public async ValueTask<ActionResult<Shift>> PostShiftAsync(Shift shift)
    {
        try
        {
            Shift addedShift =
                await this.shiftService.AddShiftAsync(shift);

            return Created(addedShift);
        }
        catch (ShiftValidationException shiftValidationException)
        {
            return BadRequest(shiftValidationException.InnerException);
        }
        catch (ShiftDependencyValidationException shiftValidationException)
            when (shiftValidationException.InnerException is InvalidShiftReferenceException)
        {
            return FailedDependency(shiftValidationException.InnerException);
        }
        catch (ShiftDependencyValidationException shiftDependencyValidationException)
            when (shiftDependencyValidationException.InnerException is AlreadyExistsShiftException)
        {
            return Conflict(shiftDependencyValidationException.InnerException);
        }
        catch (ShiftDependencyException shiftDependencyException)
        {
            return InternalServerError(shiftDependencyException);
        }
        catch (ShiftServiceException shiftServiceException)
        {
            return InternalServerError(shiftServiceException);
        }
    }

    [HttpGet]
    public ActionResult<IQueryable<Shift>> GetAllShifts()
    {
        try
        {
            IQueryable<Shift> retrievedShifts =
                this.shiftService.RetrieveAllShifts();

            return Ok(retrievedShifts);
        }
        catch (ShiftDependencyException shiftDependencyException)
        {
            return InternalServerError(shiftDependencyException);
        }
        catch (ShiftServiceException shiftServiceException)
        {
            return InternalServerError(shiftServiceException);
        }
    }

    [HttpGet("{shiftId}")]
    public async ValueTask<ActionResult<Shift>> GetShiftByIdAsync(Guid shiftId)
    {
        try
        {
            Shift shift = await this.shiftService.RetrieveShiftByIdAsync(shiftId);

            return Ok(shift);
        }
        catch (ShiftValidationException shiftValidationException)
            when (shiftValidationException.InnerException is NotFoundShiftException)
        {
            return NotFound(shiftValidationException.InnerException);
        }
        catch (ShiftValidationException shiftValidationException)
        {
            return BadRequest(shiftValidationException.InnerException);
        }
        catch (ShiftDependencyException shiftDependencyException)
        {
            return InternalServerError(shiftDependencyException);
        }
        catch (ShiftServiceException shiftServiceException)
        {
            return InternalServerError(shiftServiceException);
        }
    }

    [HttpPut]
    public async ValueTask<ActionResult<Shift>> PutShiftAsync(Shift shift)
    {
        try
        {
            Shift modifiedShift =
                await this.shiftService.ModifyShiftAsync(shift);

            return Ok(modifiedShift);
        }
        catch (ShiftValidationException shiftValidationException)
            when (shiftValidationException.InnerException is NotFoundShiftException)
        {
            return NotFound(shiftValidationException.InnerException);
        }
        catch (ShiftValidationException shiftValidationException)
        {
            return BadRequest(shiftValidationException.InnerException);
        }
        catch (ShiftDependencyValidationException shiftValidationException)
            when (shiftValidationException.InnerException is InvalidShiftReferenceException)
        {
            return FailedDependency(shiftValidationException.InnerException);
        }
        catch (ShiftDependencyValidationException shiftDependencyValidationException)
            when (shiftDependencyValidationException.InnerException is AlreadyExistsShiftException)
        {
            return Conflict(shiftDependencyValidationException.InnerException);
        }
        catch (ShiftDependencyException shiftDependencyException)
        {
            return InternalServerError(shiftDependencyException);
        }
        catch (ShiftServiceException shiftServiceException)
        {
            return InternalServerError(shiftServiceException);
        }
    }

    [HttpDelete("{shiftId}")]
    public async ValueTask<ActionResult<Shift>> DeleteShiftByIdAsync(Guid shiftId)
    {
        try
        {
            Shift deletedShift =
                await this.shiftService.RemoveShiftByIdAsync(shiftId);

            return Ok(deletedShift);
        }
        catch (ShiftValidationException shiftValidationException)
            when (shiftValidationException.InnerException is NotFoundShiftException)
        {
            return NotFound(shiftValidationException.InnerException);
        }
        catch (ShiftValidationException shiftValidationException)
        {
            return BadRequest(shiftValidationException.InnerException);
        }
        catch (ShiftDependencyValidationException shiftDependencyValidationException)
            when (shiftDependencyValidationException.InnerException is LockedShiftException)
        {
            return Locked(shiftDependencyValidationException.InnerException);
        }
        catch (ShiftDependencyValidationException shiftDependencyValidationException)
        {
            return BadRequest(shiftDependencyValidationException);
        }
        catch (ShiftDependencyException shiftDependencyException)
        {
            return InternalServerError(shiftDependencyException);
        }
        catch (ShiftServiceException shiftServiceException)
        {
            return InternalServerError(shiftServiceException);
        }
    }
}