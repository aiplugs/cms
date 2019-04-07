using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Aiplugs.CMS.Core.Models;
using Aiplugs.CMS.Core.Services;
using Aiplugs.CMS.Web.Attributes;
using Aiplugs.CMS.Web.Extensions;
using Aiplugs.CMS.Web.Filters;
using Aiplugs.CMS.Web.Models;
using Aiplugs.CMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Web.Controllers
{
    [Authorize]
    [Route("collections")]
    [ServiceFilter(typeof(SharedDataLoad))]
    public class CollectionsController : Controller
    {
        private readonly ISettingsService _settings;
        private readonly IDataService _data;
        private readonly IProcedureService _procedureService;
        public CollectionsController(ISettingsService settingsService, IDataService dataService, IProcedureService procedureService)
        {
            _settings = settingsService;
            _data = dataService;
            _procedureService = procedureService;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> List(string name, [FromQuery]SearchMethod method = SearchMethod.Simple, [FromQuery]string query = null, [FromQuery]string skipToken = null, [FromQuery]int limit = 10)
        {
            var collection = await _settings.FindCollectionAsync(name);
            var items = method == SearchMethod.Simple 
                        ? await _data.SearchAsync(name, query, skipToken, limit)
                        : await _data.QueryAsync(name, query, skipToken, limit);
            var first = items.FirstOrDefault();
            var last = items.Count() == limit ? items.LastOrDefault() : null;

            return View(new CollectionViewModel
            {
                SearchMethod = method,
                SeachQuery = query,
                TitlePath = collection.TitlePath,
                Items = items,
                CollectionName = name,
                Procedures = collection.Procedures ?? new ProcedureInfo[0]
            });
        }

        [HttpGet("{name}/{id}/view")]
        public async Task<IActionResult> Data(string name, string id)
        {
            var collection = await _settings.FindCollectionAsync(name);
            var item = await _data.LookupAsync(id);
            if (item == null)
                return NotFound();

            return View(new DataViewModel
            {
                CollectionName = name,
                Item = item
            });
        }

        [HttpGet("{name}/{id}")]
        public async Task<IActionResult> Edit(string name, string id)
        {
            var collection = await _settings.FindCollectionAsync(name);
            var item = await _data.LookupAsync(id);
            if (item == null)
                return NotFound();

            return View(new EditViewModel
            {
                CollectionName = name,
                Uri = Url.Content($"~/api/data/{name}/{id}"),
                Schema = new CMSSchema(collection.Schema),
                Data = item.Data
            });
        }

        [HttpPost("{name}/{id}")]
        public async Task<IActionResult> UpdateData(string name, string id)
        {
            var item = await _data.LookupAsync(id);
            if (item == null)
                return NotFound();

            var collection = await _settings.FindCollectionAsync(name);
            var data = HttpContext.Request.Form.ToJToken(collection.Schema);

            if (!await _data.ValidateAsync(name, data))
                return BadRequest();

            await _data.UpdateAsync(id, data, item.CurrentId);

            return RedirectToAction("Edit", new { name, id });
        }

        [HttpGet("{name}/@new")]
        public async Task<IActionResult> NewDataAsync([FromRoute]string name)
        {
            var collection = await _settings.FindCollectionAsync(name);
            return View("Edit", new EditViewModel
            {
                CollectionName = name,
                Uri = Url.Content($"~/api/data/{name}"),
                Schema = new CMSSchema(collection.Schema),
                Data = null
            });
        }

        [HttpPost("{name}/@new")]
        public async Task<IActionResult> CreateNewDataAsync(string name)
        {
            var collection = await _settings.FindCollectionAsync(name);
            var data = HttpContext.Request.Form.ToJToken(collection.Schema);

            if (!await _data.ValidateAsync(name, data))
                return BadRequest();

            var id = await _data.AddAsync(name, data);

            return RedirectToAction("Edit", new { name, id });
        }

        [HttpGet("{name}/$schema")]
        public async Task<ContentResult> Schema(string name)
        {
            var collection = await _settings.FindCollectionAsync(name);
            return Content(collection.Schema, "application/json");
        }

        [HttpPost("{name}")]
        [SubmitAction("exec")]
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

            return RedirectToAction("Item", "Jobs", new { id });
        }
    }
}