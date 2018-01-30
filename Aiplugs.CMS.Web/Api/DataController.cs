using System;
using System.Linq;
using Aiplugs.CMS.Web.Services;
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
    private readonly IDataValidateService _validator;
    public DataController(ISettingsService settingsService,IDataService dataService, IDataValidateService dataValidateService) 
    {
      _settings = settingsService;
      _data = dataService;
      _validator = dataValidateService;
    }
    
    [HttpGet("{name}")]
    public IActionResult Get(string name, bool desc = true, long? skipToken = null, int limit = 20) 
    {
      var query = _data.GetItems(name);
      
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
    public IActionResult Get(string name, long id)
    {
      var data = _data.Find(id);
      if (data == null)
        return NotFound();

      return Ok(data);
    }

    [HttpPost("{name}")]
    public IActionResult Post(string name, [FromBody]JObject model)
    {
      if (_validator.ValidateCollection(name, model) == false)
        return BadRequest();
      
      var id = _data.Add(name, model);

      return Created(Url.Action("Data", "Collections", new {id}), null);
    }

    [HttpPut("{name}")]
    public IActionResult Put(string name, long id, [FromBody]JObject model)
    {
      if (_validator.ValidateCollection(name, model) == false)
        return BadRequest();

      if (_data.Find(id) == null)
        return NotFound();

      _data.Update(id, model);

      return NoContent();
    }

    [HttpDelete("{name}")]
    public IActionResult Delete(string name, long id)
    {
      throw new NotImplementedException();
    }
  }
}