using Microsoft.AspNetCore.Mvc;

namespace WorkPlan.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public ActionResult<string> Get() => Ok("Teamway is awesome!");
}
