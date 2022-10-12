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
}