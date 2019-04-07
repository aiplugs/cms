using Aiplugs.CMS.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Web.Api
{
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        private readonly ISettingsService _settings;
        private readonly IDataService _data;
        public DataController(ISettingsService settingsService, IDataService dataService)
        {
            _settings = settingsService;
            _data = dataService;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> Get(string name, bool desc = true, string skipToken = null, int limit = 20)
        {
            var items = await _data.SearchAsync(name, null, skipToken, limit, desc);
            var first = items.FirstOrDefault();
            var last = items.LastOrDefault();

            return Json(new ApiResultViewModel<JToken>
            {
                Result = items.Select(item => item.Data),
                Prev = first != null ? Url.Action("Get", new { desc = !desc, skipToken = first.Id, limit }) : null,
                Next = last != null ? Url.Action("Get", new { desc = !desc, skipToken = last.Id, limit }) : null,
            });
        }
        [HttpGet("{name}/{id}")]
        public async Task<IActionResult> Get(string name, string id)
        {
            var data = await _data.LookupAsync(id);
            if (data == null)
                return NotFound();

            return Ok(data);
        }

        [HttpPost("{name}")]
        public async Task<IActionResult> Post(string name, [FromBody]JToken model)
        {
            if ((await _data.ValidateAsync(name, model)) == false)
                return BadRequest();

            var id = await _data.AddAsync(name, model);

            return Created(Url.Action("Data", "Collections", new { id }), null);
        }

        [HttpPut("{name}")]
        public async Task<IActionResult> Put(string name, string id, [FromBody]JToken model)
        {
            if ((await _data.ValidateAsync(name, model)) == false)
                return BadRequest();

            var item = await _data.LookupAsync(id);
            if (item == null)
                return NotFound();

            await _data.UpdateAsync(id, model, item.CurrentId);

            return NoContent();
        }

        [HttpDelete("{name}")]
        public IActionResult Delete(string name, string id)
        {
            throw new NotImplementedException();
        }
    }
}