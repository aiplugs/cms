using Aiplugs.CMS.Functions;
using Microsoft.AspNetCore.Mvc;

namespace Aiplugs.CMS.Web.Api
{
  [Route("api/[controller]")]
  public class JobsController : Controller
  {
    private IJobService _jobService;
    public JobsController(IJobService jobService)
    {
      _jobService = jobService;
    }

    [HttpPost()]
    public IActionResult Post()
    {
      string controller;
      string procedure;
      if (/* has runnning*/)
        return new StatusCodeResult(409);
      
      _jobService.Create
    }
  }
}