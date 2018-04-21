using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Web.Filters;
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
        public CollectionsController(ISettingsService settingsService, IDataService dataService)
        {
            _settings = settingsService;
            _data = dataService;
        }
        [HttpGet("{name}")]
        public async Task<IActionResult> List(string name, [FromQuery]int skipToken = 0, [FromQuery]int limit = 10)
        {
            var items = await _data.SearchAsync(name, null, skipToken, limit);
            var first = items.FirstOrDefault();
            var last = items.Count() == limit ? items.LastOrDefault() : null;

            return View(new CollectionViewModel
            {
                Items = items,
                CollectionName = name,
            });
        }

        [HttpGet("{name}/{id}")]
        public async Task<IActionResult> Data(string name, long id)
        {
            var item = await _data.LookupAsync(id);
            if (item == null)
                return NotFound();

            return View(new DataViewModel
            {
                Uri = Url.Content($"~/api/data/{name}/{id}"),
                Data = item.Data
            });
        }

        [HttpGet("{name}/@new")]
        public IActionResult NewData(string name)
        {
            return View("Data", new DataViewModel
            {
                Uri = Url.Content($"~/api/data/{name}")
            });
        }

        [HttpGet("{name}/$schema")]
        public async Task<ContentResult> Schema(string name)
        {
            var collection = await _settings.FindCollectionAsync(name);
            return Content(collection.Schema, "application/json");
        }
    }
}