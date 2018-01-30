using System;
using System.Linq;
using Aiplugs.CMS.Web.Models;
using Aiplugs.CMS.Web.ViewModels.StorageViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aiplugs.CMS.Web.Api {
  
  [Route("api/[controller]")]
  public class FoldersController : Controller
  {
    private IStorageService _storage;
    public FoldersController(IStorageService storage)
    {
      _storage = storage;
    }
    
    [HttpGet("{*id}")]
    public IActionResult Get(string id)
    {
      var folder = _storage.GetFolder(id);
      if (folder == null)
        return NotFound();

      return Ok(new {
        Folders = folder.Children.Select(f => new FolderViewModel { Name = f.Name, LastModifiedAt = DateTime.UtcNow }),
        Files = folder.Files.Select(f => new FileViewModel
        {
          Name = f.Name,
          Size = f.Size,
          LastModifiedAt = f.LastModifiedAt,
        })
      });
    }

    [HttpPost("{*id}")]
    public IActionResult Post(string id, [FromBody]AppendFolderViewModel model)
    {
      if(ModelState.IsValid == false)
        return BadRequest(ModelState);

      var folder = _storage.GetFolder(id);

      if (folder == null)
        return NotFound();

      _storage.Add(folder, model.Name);

      return Ok();
    }

    [HttpPut("{*id}")]
    public IActionResult Put(string id, [FromBody]RenameViewModel model)
    {
      if(ModelState.IsValid == false)
        return BadRequest(ModelState);

      var folder = _storage.GetFolder(id);
      
      if (folder == null)
        return NotFound();

      _storage.Rename(folder, model.Name);

      return Ok();
    }

    [HttpDelete("{*id}")]
    public IActionResult Delete(string id)
    {
      var folder = (Folder)_storage.GetFolder(id);
      if (folder == null)
        return NotFound();

      if (folder.IsHome)
        return BadRequest("Cannot delete home folder");

      _storage.Remove(folder);

      return Ok();
    }
  }
}