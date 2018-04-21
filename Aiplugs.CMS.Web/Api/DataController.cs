using System;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Web.Api 
{
  [Route("api/[controller]")]
  public class DataController : Controller
  {
    private readonly ISettingsService _settings;
    private readonly IDataService _data;
    public DataController(ISettingsService settingsService,IDataService dataService) 
    {
      _settings = settingsService;
      _data = dataService;
    }
    
    [HttpGet("{name}")]
    public async Task<IActionResult> Get(string name, bool desc = true, long? skipToken = null, int limit = 20) 
    {
      var query = await _data.SearchAsync(name);
      
      if (desc)
        query = query.OrderByDescending(item => item.Id);      
      else
        query = query.OrderBy(item => item.Id);
      
      if (skipToken.HasValue) 
        query = query.Where(item => item.Id > skipToken);
      
      var items = query.Take(limit).ToArray();
      var first = items.FirstOrDefault();
      var last = items.LastOrDefault();

      return Json(new ApiResultViewModel<JObject>
      {
        Result = items.Select(item => item.Data),
        Prev = first != null ? Url.Action("Get", new {desc=false, skipToken=first.Id, limit}) : null,
        Next = last != null ? Url.Action("Get", new {desc=true, skipToken=last.Id, limit}) : null,
      });
    }
    [HttpGet("{name}/{id}")]
    public async Task<IActionResult> Get(string name, long id)
    {
      var data = await _data.LookupAsync(id);
      if (data == null)
        return NotFound();

      return Ok(data);
    }

    [HttpPost("{name}")]
    public async Task<IActionResult> Post(string name, [FromBody]JObject model)
    {
      if ((await _data.ValidateAsync(name, model)) == false)
        return BadRequest();
      
      var id = await _data.AddAsync(name, model);

      return Created(Url.Action("Data", "Collections", new {id}), null);
    }

    [HttpPut("{name}")]
    public async Task<IActionResult> Put(string name, long id, [FromBody]JObject model)
    {
      if ((await _data.ValidateAsync(name, model)) == false)
        return BadRequest();

      if ((await _data.LookupAsync(id)) == null)
        return NotFound();

      await _data.UpdateAsync(id, model);

      return NoContent();
    }

    [HttpDelete("{name}")]
    public IActionResult Delete(string name, long id)
    {
      throw new NotImplementedException();
    }
  }
}