using Microsoft.AspNetCore.Mvc;

namespace Aiplugs.CMS.Web.Controllers
{
    public class ProcedureController : Controller
    {
        [HttpGet("/api/procedures")]
        public IActionResult List()
        {
            var data = new[]
            {
                new ProcedureInfo
                {
                    Name = "CustomSample",
                    TypeName = "Aiplugs.CMS.Functions.CustomSample",
                    DllPath = "/packages/Aiplugs.CMS.Functions/Aiplugs.CMS.Functions.dll"
                }
            };
            return Json(data);
        }
    }
}
