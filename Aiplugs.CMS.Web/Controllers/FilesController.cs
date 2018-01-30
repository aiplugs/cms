using Aiplugs.CMS.Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Aiplugs.CMS.Web.Controllers
{
  [ServiceFilter(typeof(SharedDataLoad))]
  public class FilesController : Controller
  {
    public IActionResult Index() 
    {
      return View();
    }
  }
}