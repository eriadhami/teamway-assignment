using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using WorkPlan.Api.Models.Plans;
using WorkPlan.Api.Models.Plans.Exceptions;
using WorkPlan.Api.Services.Foundations.Plans;

namespace WorkPlan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlansController : RESTFulController
{
    private readonly IPlanService planService;

    public PlansController(IPlanService planService) =>
        this.planService = planService;

    [HttpPost]
    public async ValueTask<ActionResult<Plan>> PostPlanAsync(Plan plan)
    {
        try
        {
            Plan addedPlan =
                await this.planService.AddPlanAsync(plan);

            return Created(addedPlan);
        }
        catch (PlanValidationException planValidationException)
        {
            return BadRequest(planValidationException.InnerException);
        }
        catch (PlanDependencyValidationException planValidationException)
            when (planValidationException.InnerException is InvalidPlanReferenceException)
        {
            return FailedDependency(planValidationException.InnerException);
        }
        catch (PlanDependencyValidationException planDependencyValidationException)
            when (planDependencyValidationException.InnerException is AlreadyExistsPlanException)
        {
            return Conflict(planDependencyValidationException.InnerException);
        }
        catch (PlanDependencyValidationException planValidationException)
            when (planValidationException.InnerException is DuplicatePlanWorkerDateException)
        {
            return UnprocessableEntity(planValidationException.InnerException);
        }
        catch (PlanDependencyException planDependencyException)
        {
            return InternalServerError(planDependencyException);
        }
        catch (PlanServiceException planServiceException)
        {
            return InternalServerError(planServiceException);
        }
    }

    [HttpGet]
    public ActionResult<IQueryable<Plan>> GetAllPlans()
    {
        try
        {
            IQueryable<Plan> retrievedPlans =
                this.planService.RetrieveAllPlans();

            return Ok(retrievedPlans);
        }
        catch (PlanDependencyException planDependencyException)
        {
            return InternalServerError(planDependencyException);
        }
        catch (PlanServiceException planServiceException)
        {
            return InternalServerError(planServiceException);
        }
    }

    [HttpGet("{planId}")]
    public async ValueTask<ActionResult<Plan>> GetPlanByIdAsync(Guid planId)
    {
        try
        {
            Plan plan = await this.planService.RetrievePlanByIdAsync(planId);

            return Ok(plan);
        }
        catch (PlanValidationException planValidationException)
            when (planValidationException.InnerException is NotFoundPlanException)
        {
            return NotFound(planValidationException.InnerException);
        }
        catch (PlanValidationException planValidationException)
        {
            return BadRequest(planValidationException.InnerException);
        }
        catch (PlanDependencyException planDependencyException)
        {
            return InternalServerError(planDependencyException);
        }
        catch (PlanServiceException planServiceException)
        {
            return InternalServerError(planServiceException);
        }
    }

    [HttpPut]
    public async ValueTask<ActionResult<Plan>> PutPlanAsync(Plan plan)
    {
        try
        {
            Plan modifiedPlan =
                await this.planService.ModifyPlanAsync(plan);

            return Ok(modifiedPlan);
        }
        catch (PlanValidationException planValidationException)
            when (planValidationException.InnerException is NotFoundPlanException)
        {
            return NotFound(planValidationException.InnerException);
        }
        catch (PlanValidationException planValidationException)
        {
            return BadRequest(planValidationException.InnerException);
        }
        catch (PlanDependencyValidationException planValidationException)
            when (planValidationException.InnerException is InvalidPlanReferenceException)
        {
            return FailedDependency(planValidationException.InnerException);
        }
        catch (PlanDependencyValidationException planDependencyValidationException)
            when (planDependencyValidationException.InnerException is AlreadyExistsPlanException)
        {
            return Conflict(planDependencyValidationException.InnerException);
        }
        catch (PlanDependencyException planDependencyException)
        {
            return InternalServerError(planDependencyException);
        }
        catch (PlanServiceException planServiceException)
        {
            return InternalServerError(planServiceException);
        }
    }

    [HttpDelete("{planId}")]
    public async ValueTask<ActionResult<Plan>> DeletePlanByIdAsync(Guid planId)
    {
        try
        {
            Plan deletedPlan =
                await this.planService.RemovePlanByIdAsync(planId);

            return Ok(deletedPlan);
        }
        catch (PlanValidationException planValidationException)
            when (planValidationException.InnerException is NotFoundPlanException)
        {
            return NotFound(planValidationException.InnerException);
        }
        catch (PlanValidationException planValidationException)
        {
            return BadRequest(planValidationException.InnerException);
        }
        catch (PlanDependencyValidationException planDependencyValidationException)
            when (planDependencyValidationException.InnerException is LockedPlanException)
        {
            return Locked(planDependencyValidationException.InnerException);
        }
        catch (PlanDependencyValidationException planDependencyValidationException)
        {
            return BadRequest(planDependencyValidationException);
        }
        catch (PlanDependencyException planDependencyException)
        {
            return InternalServerError(planDependencyException);
        }
        catch (PlanServiceException planServiceException)
        {
            return InternalServerError(planServiceException);
        }
    }
}