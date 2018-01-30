using System.Linq;
using Aiplugs.CMS.Web.Filters;
using Aiplugs.CMS.Web.Services;
using Aiplugs.CMS.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Web.Controllers
{
  [Route("collections")]
  [ServiceFilter(typeof(SharedDataLoad))]
  [Authorize]
  public class CollectionsController : Controller
  {
    private readonly ISettingsService _settings;
    private readonly IDataService _data;
    private readonly IDataValidateService _validator;
    public CollectionsController(ISettingsService settingsService,IDataService dataService, IDataValidateService dataValidateService) 
    {
      _settings = settingsService;
      _data = dataService;
      _validator = dataValidateService;
    }
    [HttpGet("{name}")]
    public IActionResult List(string name, [FromQuery]int skipToken = 0, [FromQuery]int limit = 10) 
    {
      var items = _data.GetItems(name, skipToken, limit);
      var first = items.FirstOrDefault();
      var last = items.Count() == limit ? items.LastOrDefault() : null;

      return View(new CollectionViewModel{
        Items = items,
        CollectionName = name,
      });
    }

    [HttpGet("{name}/{id}")]
    public IActionResult Data(string name, long id) 
    {
      var data = _data.Find(id);
      if (data == null)
        return NotFound();

      return View(new DataViewModel 
      { 
        Uri = Url.Content($"~/api/data/{name}/{id}"),
        Data = data
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
    public ContentResult Schema(string name) 
    {
      var collection = _settings.GetCollection(name);
      return Content(collection.Schema, "application/json");
    }
  }
}