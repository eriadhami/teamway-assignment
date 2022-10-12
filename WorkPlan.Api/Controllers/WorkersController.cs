using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using WorkPlan.Api.Models.Workers;
using WorkPlan.Api.Models.Workers.Exceptions;
using WorkPlan.Api.Services.Foundations.Workers;

namespace WorkPlan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkersController : RESTFulController
{
    private readonly IWorkerService workerService;

    public WorkersController(IWorkerService workerService) =>
        this.workerService = workerService;

    [HttpPost]
    public async ValueTask<ActionResult<Worker>> PostWorkerAsync(Worker worker)
    {
        try
        {
            Worker addedWorker =
                await this.workerService.AddWorkerAsync(worker);

            return Created(addedWorker);
        }
        catch (WorkerValidationException workerValidationException)
        {
            return BadRequest(workerValidationException.InnerException);
        }
        catch (WorkerDependencyValidationException workerValidationException)
            when (workerValidationException.InnerException is InvalidWorkerReferenceException)
        {
            return FailedDependency(workerValidationException.InnerException);
        }
        catch (WorkerDependencyValidationException workerDependencyValidationException)
            when (workerDependencyValidationException.InnerException is AlreadyExistsWorkerException)
        {
            return Conflict(workerDependencyValidationException.InnerException);
        }
        catch (WorkerDependencyException workerDependencyException)
        {
            return InternalServerError(workerDependencyException);
        }
        catch (WorkerServiceException workerServiceException)
        {
            return InternalServerError(workerServiceException);
        }
    }

    [HttpGet]
    public ActionResult<IQueryable<Worker>> GetAllWorkers()
    {
        try
        {
            IQueryable<Worker> retrievedWorkers =
                this.workerService.RetrieveAllWorkers();

            return Ok(retrievedWorkers);
        }
        catch (WorkerDependencyException workerDependencyException)
        {
            return InternalServerError(workerDependencyException);
        }
        catch (WorkerServiceException workerServiceException)
        {
            return InternalServerError(workerServiceException);
        }
    }

    [HttpGet("{workerId}")]
    public async ValueTask<ActionResult<Worker>> GetWorkerByIdAsync(Guid workerId)
    {
        try
        {
            Worker worker = await this.workerService.RetrieveWorkerByIdAsync(workerId);

            return Ok(worker);
        }
        catch (WorkerValidationException workerValidationException)
            when (workerValidationException.InnerException is NotFoundWorkerException)
        {
            return NotFound(workerValidationException.InnerException);
        }
        catch (WorkerValidationException workerValidationException)
        {
            return BadRequest(workerValidationException.InnerException);
        }
        catch (WorkerDependencyException workerDependencyException)
        {
            return InternalServerError(workerDependencyException);
        }
        catch (WorkerServiceException workerServiceException)
        {
            return InternalServerError(workerServiceException);
        }
    }

    [HttpPut]
    public async ValueTask<ActionResult<Worker>> PutWorkerAsync(Worker worker)
    {
        try
        {
            Worker modifiedWorker =
                await this.workerService.ModifyWorkerAsync(worker);

            return Ok(modifiedWorker);
        }
        catch (WorkerValidationException workerValidationException)
            when (workerValidationException.InnerException is NotFoundWorkerException)
        {
            return NotFound(workerValidationException.InnerException);
        }
        catch (WorkerValidationException workerValidationException)
        {
            return BadRequest(workerValidationException.InnerException);
        }
        catch (WorkerDependencyValidationException workerValidationException)
            when (workerValidationException.InnerException is InvalidWorkerReferenceException)
        {
            return FailedDependency(workerValidationException.InnerException);
        }
        catch (WorkerDependencyValidationException workerDependencyValidationException)
            when (workerDependencyValidationException.InnerException is AlreadyExistsWorkerException)
        {
            return Conflict(workerDependencyValidationException.InnerException);
        }
        catch (WorkerDependencyException workerDependencyException)
        {
            return InternalServerError(workerDependencyException);
        }
        catch (WorkerServiceException workerServiceException)
        {
            return InternalServerError(workerServiceException);
        }
    }

    [HttpDelete("{workerId}")]
    public async ValueTask<ActionResult<Worker>> DeleteWorkerByIdAsync(Guid workerId)
    {
        try
        {
            Worker deletedWorker =
                await this.workerService.RemoveWorkerByIdAsync(workerId);

            return Ok(deletedWorker);
        }
        catch (WorkerValidationException workerValidationException)
            when (workerValidationException.InnerException is NotFoundWorkerException)
        {
            return NotFound(workerValidationException.InnerException);
        }
        catch (WorkerValidationException workerValidationException)
        {
            return BadRequest(workerValidationException.InnerException);
        }
        catch (WorkerDependencyValidationException workerDependencyValidationException)
            when (workerDependencyValidationException.InnerException is LockedWorkerException)
        {
            return Locked(workerDependencyValidationException.InnerException);
        }
        catch (WorkerDependencyValidationException workerDependencyValidationException)
        {
            return BadRequest(workerDependencyValidationException);
        }
        catch (WorkerDependencyException workerDependencyException)
        {
            return InternalServerError(workerDependencyException);
        }
        catch (WorkerServiceException workerServiceException)
        {
            return InternalServerError(workerServiceException);
        }
    }
}