using Aiplugs.CMS.Core.Services;
using Aiplugs.CMS.Web.Attributes;
using Aiplugs.CMS.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Web.Controllers
{
    [ServiceFilter(typeof(SharedDataLoad))]
    public class JobsController : Controller
    {
        private readonly IJobService _service;
        private readonly IJobRegistory _registory;
        private readonly IProcedureService _procedureService;
        public JobsController(IJobService service, IJobRegistory registory, IProcedureService procedureService)
        {
            _service = service;
            _registory = registory;
            _procedureService = procedureService;
        }

        [HttpPost("/jobs/{id}")]
        [SubmitAction("cancel")]
        public IActionResult Cancel([FromRoute]string id)
        {
            _service.Cancel(id);

            return RedirectToAction(nameof(Item), new { id });
        }

        [HttpGet("/jobs")]
        public async Task<IActionResult> List([FromQuery]string name)
        {
            var jobs = await _service.GetAsync(name);

            return View(jobs);
        }

        public enum Style { Default, NoBlade }

        [HttpGet("/jobs/{id}")]
        public async Task<IActionResult> Item([FromRoute]string id, [FromQuery]Style style = Style.Default)
        {
            var job = await _service.FindAsync(id);
            var template = style == Style.NoBlade ? "NoBladeItem" : "Item";
            return View(template, job);
        }
    }
}
