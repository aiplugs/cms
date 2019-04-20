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
        public async Task<IActionResult> List(string name, [FromQuery]SearchMethod method = SearchMethod.Simple, [FromQuery]string query = null, [FromQuery]string skipToken = null, [FromQuery]int limit = 20, [FromQuery]RenderStyle style = RenderStyle.Default)
        {
            var collection = await _settings.FindCollectionAsync(name);
            var items = method == SearchMethod.Simple 
                        ? await _data.SearchAsync(name, query, skipToken, limit)
                        : await _data.QueryAsync(name, query, skipToken, limit);
            var first = items.FirstOrDefault();
            var last = items.Count() == limit ? items.LastOrDefault() : null;

            var model = new CollectionViewModel
            {
                SearchMethod = method,
                SeachQuery = query,
                TitlePath = collection.TitlePath,
                Items = items,
                CollectionName = name,
                Procedures = collection.Procedures ?? new ProcedureInfo[0],
                Limit = limit
            };

            if (style == RenderStyle.NoBlade)
                return View("ListNoBlade", model);

            return View(model);
        }

        [HttpGet("{name}/{id}")]
        public async Task<IActionResult> Item(string name, string id,  [FromHeader(Name = "X-IC-Request")]bool isIntercooler = false)
        {
            var collection = await _settings.FindCollectionAsync(name);
            var item = await _data.LookupAsync(id);
            if (item == null)
                return NotFound();

            var model = new DataViewModel
            {
                CollectionName = name,
                Item = item
            };

            if (!isIntercooler)
            {
                var limit = 20;
                var items = await _data.SearchAsync(name, null, id.Substring(0, id.Length-1), limit);
                return View("List", new CollectionViewModel {
                    SearchMethod = SearchMethod.Simple,
                    TitlePath = collection.TitlePath,
                    Items = items,
                    CollectionName = name,
                    Procedures = collection.Procedures ?? new ProcedureInfo[0],
                    Data = model,
                    Limit = limit
                });
            }

            return View(model);
        }

        [HttpGet("{name}/{id}/@edit")]
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

        [HttpPost("{name}/{id}/@edit")]
        public async Task<IActionResult> UpdateData(string name, string id)
        {
            var item = await _data.LookupAsync(id);
            if (item == null)
                return NotFound();

            var collection = await _settings.FindCollectionAsync(name);
            var data = HttpContext.Request.Form.ToJToken(collection.Schema);

            var (valid, errors) = await _data.ValidateAsync(name, data);
            if (!valid)
            {
                ViewData["Errors"] = errors;
                return View("Edit", new EditViewModel
                {
                    CollectionName = name,
                    Uri = Url.Content($"~/api/data/{name}/{id}"),
                    Schema = new CMSSchema(collection.Schema),
                    Data = data
                });
            }

            await _data.UpdateAsync(id, data, item.CurrentId, true);

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

            var (valid, errors) = await _data.ValidateAsync(name, data);
            if (!valid)
                return BadRequest();

            var id = await _data.AddAsync(name, data);

            return RedirectToAction("Item", new { name, id });
        }

        [HttpGet("{name}/$schema")]
        public async Task<ContentResult> Schema(string name)
        {
            var collection = await _settings.FindCollectionAsync(name);
            return Content(collection.Schema, "application/json");
        }
    }
}