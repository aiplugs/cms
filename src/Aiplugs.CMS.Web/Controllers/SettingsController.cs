using System.Collections.Generic;
using System.Threading.Tasks;
using Aiplugs.CMS.Web.Filters;
using Aiplugs.CMS.Web.ViewModels.SettingsViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Web.Controllers
{
    [Authorize]
    [Route("settings")]
    [ServiceFilter(typeof(SharedDataLoad))]
    public class SettingsController : Controller
    {
        private ISettingsService _settingsService;
        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }
        public IActionResult AppSettings()
        {
            return View();
        }
        public IActionResult Users()
        {
            return View();
        }

        [HttpGet("collections")]
        public async Task<IActionResult> Collections()
        {
            var collections = await _settingsService.GetCollectionsAsync();
            return View(collections);
        }

        [HttpGet("collections/@new")]
        public IActionResult NewCollection()
        {
            return View("Collection", new CollectionViewModel());
        }

        [HttpGet("collections/{name}")]
        public async Task<IActionResult> Collection(string name)
        {
            var collection = await _settingsService.FindCollectionAsync(name);
            return View(new CollectionViewModel
            {
                Name = collection.Name,
                Schema = collection.Schema,
                TitlePath = collection.TitlePath,
                PreviewTemplate = collection.PreviewTemplate,
                DisplayName = collection.DisplayName,
                DisplayOrder = collection.DisplayOrder,
                Procedures = collection.Procedures ?? new ProcedureInfo[0]
            });
        }

        [HttpPost("collections/@new")]
        public async Task<IActionResult> AppendCollection([FromForm]CollectionViewModel model)
        {
            var collection = new Collection
            {
                Name = model.Name,
                DisplayName = model.DisplayName,
                DisplayOrder = model.DisplayOrder,
                TitlePath = model.TitlePath,
                PreviewTemplate = model.PreviewTemplate,
                Schema = model.Schema,
                Procedures = model.Procedures ?? new ProcedureInfo[0]
            };

            await _settingsService.AddAsync(collection);

            return RedirectToAction(nameof(Collections));
        }

        [HttpPost("collections/{name}")]
        public async Task<IActionResult> UpdateCollection(string name, [FromForm]Collection collection)
        {
            if (!ModelState.IsValid)
                return View(collection);

            await _settingsService.UpdateAsync(collection);

            return RedirectToAction("Collection", new { name });
        }
    }
}