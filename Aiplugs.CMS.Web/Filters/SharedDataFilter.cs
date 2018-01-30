using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Aiplugs.CMS.Web.Filters
{
  public class SharedDataLoad : ActionFilterAttribute
  {
    private readonly ISettingsService _settings;
    public SharedDataLoad(ISettingsService settingsService)
    {
      _settings = settingsService;
    }
    public override void OnActionExecuting(ActionExecutingContext context)
    {
      var controller = context.Controller as Controller;
      controller.ViewBag.Collections = _settings.GetCollections();

    } 
  }
}