using System.Collections.Generic;
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
    public IActionResult Collections()
    {
      var collections = _settingsService.GetCollections();
      return View(collections);
    }

    [HttpGet("collections/@new")]
    public IActionResult NewCollection()
    {
      return View("Collection", new Collection());
    }

    [HttpGet("collections/{name}")]
    public IActionResult Collection(string name)
    {
      var collection = _settingsService.GetCollection(name);
      return View(collection);
    }

    [HttpPost("collections/@new")]
    public IActionResult AppendCollection([FromForm]Collection collection)
    {
      if (_settingsService.Validate(collection) == false) {
        return BadRequest();
      }
      
      _settingsService.Add(collection);

      return RedirectToAction("collections");
    }

    [HttpPost("collections/{name}")]
    public IActionResult UpdateCollection(string name, [FromForm]Collection collection)
    {
      if (_settingsService.Validate(collection) == false) {
        return BadRequest();
      }
      
      _settingsService.Update(collection);

      return RedirectToAction("Collection", new { name });
    }
  }
}