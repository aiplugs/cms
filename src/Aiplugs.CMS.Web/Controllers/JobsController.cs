using System.Linq;
using Aiplugs.CMS.Core.Services;
using Aiplugs.CMS.Web.Attributes;
using Aiplugs.CMS.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Aiplugs.CMS.Web.ViewModels;
using System.Net;
using Aiplugs.CMS.Core.Models;

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

        public enum Style { Default, NoLayout, NoBlade }

        [HttpPost("/jobs")]
        public async Task<IActionResult> Execute([FromForm]string exec, [FromForm]string collection, [FromForm]string[] items, [FromForm]SearchMethod method, [FromForm]string query)
        {
            var id = await _procedureService.RegisterAsync(collection, exec, new ContextParameters
            {
                CollectionName = collection,
                Items = items,
                SearchMethod = method,
                SearchQuery = query,
            });

            if (string.IsNullOrEmpty(id))
                return StatusCode((int)HttpStatusCode.Conflict);

            return RedirectToAction(nameof(Item), new { id, style = Style.NoLayout });
        }

        [HttpPost("/jobs/{id}")]
        [SubmitAction("cancel")]
        public IActionResult Cancel([FromRoute]string id)
        {
            _service.Cancel(id);

            return RedirectToAction(nameof(Item), new { id });
        }

        [HttpGet("/jobs")]
        public async Task<IActionResult> List([FromQuery]bool desc = true, [FromQuery]string skipToken = null, [FromHeader(Name = "X-IC-Request")]bool isIntercooler = false)
        {
            var model = new JobViewModel { Desc = desc };

            model.Jobs = await _service.GetAsync(null, desc, skipToken, model.Limit);

            if (!desc)
                model.Jobs = model.Jobs.Reverse();

            if (isIntercooler)
                return View("ListNoBlade", model);

            return View(model);
        }

        [HttpGet("/jobs/{id}")]
        public async Task<IActionResult> Item([FromRoute]string id, [FromQuery]Style style = Style.Default, [FromHeader(Name = "X-IC-Request")]bool isIntercooler = false)
        {
            var model = new JobViewModel
            {
                Job = await _service.FindAsync(id)
            };

            if (style == Style.NoBlade)
                return View("ItemNoBlade", model.Job);

            if (isIntercooler)
                return View("ItemNoLayout", model.Job);

            model.Jobs = await _service.GetAsync(null, true, model.PrevSkipToken(), model.Limit);
            
            return View("List", model);
        }
    }
}
