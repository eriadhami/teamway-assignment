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
}