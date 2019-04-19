using Aiplugs.CMS.Web.Filters;
using Aiplugs.CMS.Web.ViewModels.TemplateViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Web.Controllers
{
    [ServiceFilter(typeof(SharedDataLoad))]
    public class TemplatesController : Controller
    {
        private readonly ITemplateService _service;
        public TemplatesController(ITemplateService service)
        {
            _service = service;
        }

        [HttpGet("/templates")]
        public async Task<IActionResult> List()
        {
            var templates = await _service.GetAsync();
            return View(templates);
        }

        [HttpGet("/api/templates")]
        public async Task<IActionResult> Autocomplete([FromQuery]string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return Json(new object[0]);

            var templates = (await _service.GetAsync())
                .Where(t => t.Name.ToLower().Contains(keyword.ToLower()))
                .Select(t => new { label = t.Name, value = t.Name });

            return Json(templates);
        }

        [HttpGet("/templates/@new")]
        public IActionResult New()
        {
            return View(new NewTemplateViewModel());
        }

        [HttpPost("/templates")]
        public async Task<IActionResult> Create([FromForm]NewTemplateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(nameof(New), model);

            await _service.AddAsync(model.Name, model.Template);

            return RedirectToAction(nameof(List));
        }

        [HttpGet("/templates/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var template = await _service.LookupAsync(id);

            if (template == null)
                return NotFound();

            return View(new EditTemplateViewModel
            {
                Id = template.Id,
                CurrentId = template.CurrentId,
                Name = template.Name,
                Template = template.Text,
            });
        }

        [HttpPost("/templates/{id}")]
        public async Task<IActionResult> Update([FromRoute]string id, [FromForm]EditTemplateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(nameof(Edit), model);

            var template = await _service.LookupAsync(id);

            if (template == null)
                return NotFound();

            await _service.UpdateAsync(template.Id, model.CurrentId, model.Name, model.Template);

            return RedirectToAction(nameof(List));
        }
    }
}
