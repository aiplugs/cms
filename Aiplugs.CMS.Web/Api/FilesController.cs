using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Web.ViewModels.StorageViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Aiplugs.CMS.Web.Api
{

    [Route("api/[controller]")]
    public class FilesController : Controller
    {
        private IStorageService _storage;
        public FilesController(IStorageService storage)
        {
            _storage = storage;
        }

        [HttpGet("{*path}")]
        public async Task<IActionResult> Get(string path)
        {
            var file = await _storage.FindFileAsync(path);

            if (file == null)
                return NotFound();

            return File(_storage.OpenFile(file), file.ContentType);
        }

        [HttpPost("{*path}")]
        public async Task<IActionResult> Post(string path, IEnumerable<IFormFile> files)
        {
            var folder = await _storage.FindFolderAsync(path);
            if (folder == null)
                return NotFound();

            var children = await _storage.GetFilesAsync(folder);
            var errors = new List<string>();
            foreach (var file in files.Where(f => f.Length > 0))
            {
                if (children.Any(f => f.Name == file.Name))
                {
                    errors.Add(file.FileName);
                    continue;
                }
                else
                {
                    try
                    {
                        using (var stream = file.OpenReadStream())
                        {
                            await _storage.AddFileAsync(folder, file.FileName, file.ContentType, stream);
                        }
                    }
                    catch (Exception)
                    {
                        errors.Add(file.FileName);
                    }
                }
            }
            return Ok(new FileUploadViewModel
            { 
                Count = files.Count() - errors.Count,
                Size = files.Sum(f => f.Length), 
                Errors = errors
            });
        }

        [HttpPut("{*path}")]
        public async Task<IActionResult> Patch(string path, [FromBody]RenameViewModel model)
        {
            if (ModelState.IsValid == false)
                return BadRequest(ModelState);

            var file = await _storage.FindFileAsync(path);
            if (file == null)
                return NotFound();

            var dst = await _storage.FindFolderAsync(model.Destination);

            if (dst == null)
                return NotFound();

            await _storage.MoveAsync(file, dst, model.Name);

            return Ok();
        }

        [HttpDelete("{*path}")]
        public async Task<IActionResult> Delete(string path)
        {
            var file = await _storage.FindFileAsync(path);
            if (file == null)
                return NotFound();

            await _storage.RemoveAsync(file);

            return Ok();
        }
    }
}