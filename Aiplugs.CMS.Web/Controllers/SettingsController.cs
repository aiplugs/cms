using System.Collections.Generic;
using System.Threading.Tasks;
using Aiplugs.CMS.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Web.Controllers
{
    // [Authorize]
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
            return View("Collection", new Collection());
        }

        [HttpGet("collections/{name}")]
        public async Task<IActionResult> Collection(string name)
        {
            var collection = await _settingsService.FindCollectionAsync(name);
            return View(collection);
        }

        [HttpPost("collections/@new")]
        public async Task<IActionResult> AppendCollection([FromForm]Collection collection)
        {
            if ((await _settingsService.ValidateAsync(collection)) == false)
                return BadRequest();

            await _settingsService.AddAsync(collection);

            return RedirectToAction("collections");
        }

        [HttpPost("collections/{name}")]
        public async Task<IActionResult> UpdateCollection(string name, [FromForm]Collection collection)
        {
            if ((await _settingsService.ValidateAsync(collection)) == false)
                return BadRequest();

            await _settingsService.UpdateAsync(collection);

            return RedirectToAction("Collection", new { name });
        }
    }
}