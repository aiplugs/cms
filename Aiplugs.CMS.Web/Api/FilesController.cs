using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Web.ViewModels.StorageViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aiplugs.CMS.Web.Api {
  
  [Route("api/[controller]")]
  public class FilesController : Controller
  {
    private IStorageService _storage;
    public FilesController(IStorageService storage)
    {
      _storage = storage;
    }

    [HttpGet("{*id}")]
    public dynamic Get(string id)
    {
      var file = _storage.GetFile(id);
      return File(file.Binary, file.ContentType);
    }

    [HttpPost("{*id}")]
    public async Task<IActionResult> Post(string id, IEnumerable<IFormFile> files)
    {
      var folder = _storage.GetFolder(id);
      var errors = new List<string>();
      foreach (var file in files.Where(f => f.Length > 0))
      {
        if (folder.Files.Any(f => f.Name == file.Name))
        {
          errors.Add(file.FileName);
          continue;
        }
        else
        {
          using (var stream = new MemoryStream())
          {
            await file.CopyToAsync(stream);
            try
            {
              _storage.Add(folder, file.FileName, file.ContentType, stream.ToArray());
            }
            catch (Exception)
            {
              errors.Add(file.FileName);
            }
          }
        }
      }
      return Ok(new { count = files.Count() - errors.Count, size = files.Sum(f => f.Length), errors });
    }

    [HttpPut("{*id}")]
    public IActionResult Put(string id, [FromBody]RenameViewModel model)
    {
      if (ModelState.IsValid == false)
        return BadRequest(ModelState);

      var file = _storage.GetFile(id);
      if (file == null)
        return NotFound();

      _storage.Rename(file, model.Name);

      return Ok();
    }

    [HttpDelete("{*id}")]
    public IActionResult Delete(string id)
    {
      var file = _storage.GetFile(id);
      if (file == null)
        return NotFound();

      _storage.Remove(file);

      return Ok();
    }
  }
}