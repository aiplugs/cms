using System;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Web.Models;
using Aiplugs.CMS.Web.ViewModels.StorageViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Aiplugs.CMS.Web.Api
{

    [Route("api/[controller]")]
    public class FoldersController : Controller
    {
        private IStorageService _storage;
        public FoldersController(IStorageService storage)
        {
            _storage = storage;
        }

        [HttpGet("{*path}")]
        public async Task<IActionResult> Get(string path)
        {
            var folder = await _storage.FindFolderAsync(path);

            if (folder == null)
                return NotFound();

            return Ok(new
            {
                Folders = (await _storage.GetFoldersAsync(folder)).Select(f => new FolderViewModel
                {
                    Name = f.Name,
                    LastModifiedAt = DateTime.UtcNow
                }),
                Files = (await _storage.GetFilesAsync(folder)).Select(f => new FileViewModel
                {
                    Name = f.Name,
                    Size = f.Size,
                    LastModifiedAt = f.LastModifiedAt,
                })
            });
        }

        [HttpPost("{*path}")]
        public async Task<IActionResult> Post(string path, [FromBody]AppendFolderViewModel model)
        {
            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            var folder = await _storage.FindFolderAsync(path);

            if (folder == null)
                return NotFound();

            await _storage.AddFolderAsync(folder, model.Name);

            return Ok();
        }

        [HttpPut("{*path}")]
        public async Task<IActionResult> Patch(string path, [FromBody]MoveViewModel model)
        {
            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            var src = await _storage.FindFolderAsync(path);

            if (src == null)
                return NotFound();

            var dst = await _storage.FindFolderAsync(model.Destination);

            if (dst == null)
                return NotFound();

            await _storage.MoveAsync(src, dst, model.Destination);

            return Ok();
        }

        [HttpDelete("{*path}")]
        public async Task<IActionResult> Delete(string path)
        {
            var folder = await _storage.FindFolderAsync(path);
            
            if (folder == null)
                return NotFound();

            if (folder.Path == "/")
                return BadRequest("Cannot delete home folder");

            await _storage.RemoveAsync(folder);

            return Ok();
        }
    }
}