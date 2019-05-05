using Aiplugs.CMS.Web.Attributes;
using Aiplugs.CMS.Web.Filters;
using Aiplugs.CMS.Web.Services;
using Aiplugs.CMS.Web.ViewModels.StorageViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Web.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(SharedDataLoad))]
    public class FilesController : Controller
    {
        const string ERROR_MESSAGES = "ErrorMessages";
        const string SUCCESS_MESSAGES = "SuccessMessages";
        private readonly IStorageService _storage;
        private readonly IStoragePagingService _pagedStorage;
        
        public FilesController(IStorageService storage, IStoragePagingService pagedStorage)
        {
            _storage = storage;
            _pagedStorage = pagedStorage;
        }

        public enum Style { Default, NoLayout }
        public enum Select { None, Folder, File }

        [HttpGet("/files/{*path}")]
        public async Task<IActionResult> Index(
            [FromRoute]string path,
            [FromQuery]string type = null, 
            [FromQuery]string skipToken = null, 
            [FromQuery]int limit = 30, 
            [FromQuery]Style style = Style.Default,
            [FromQuery]Select select = Select.None)
        {
            var folder = await _storage.FindFolderAsync(path??string.Empty);

            if (folder == null)
                return NotFound();

            var page = await _pagedStorage.GetPageAsync(folder, type, skipToken, limit, select == Select.Folder);

            ViewData[ERROR_MESSAGES] = TempData[ERROR_MESSAGES];
            ViewData[SUCCESS_MESSAGES] = TempData[ERROR_MESSAGES];

            var viewName = style == Style.NoLayout && select == Select.Folder ? "FolderSelectNoLayout"
                         : style == Style.Default  && select == Select.Folder ? "FolderSelect"
                         : style == Style.NoLayout && select == Select.File   ? "FileSelectNoLayout"
                         : style == Style.Default  && select == Select.File   ? "FileSelect"
                         : style == Style.NoLayout && select == Select.None   ? "IndexNoLayout"
                         : "Index";

            return View(viewName, page);
        }

        private IActionResult RedirectToIndexWithQueryString()
            => LocalRedirect(Request.Path + HttpContext.Request.QueryString.Value);

        [HttpPost("/files/{*path}")]
        [SubmitAction("mkdir")]
        public async Task<IActionResult> MakeDirectory([FromRoute]string path, [FromForm]AppendFolderViewModel model)
        {
            if (!ModelState.IsValid)
                TempData[ERROR_MESSAGES] = "InvalidRequest";

            var folder = await _storage.FindFolderAsync(path ?? string.Empty);

            if (folder == null)
                TempData[ERROR_MESSAGES] = $"NotFound:{path}";

            await _storage.AddFolderAsync(folder, model.Name);

            TempData[SUCCESS_MESSAGES] = $"Mkdir:{path}";

            return RedirectToIndexWithQueryString();
        }

        [HttpPost("/files/{*path}")]
        [SubmitAction("mv")]
        public async Task<IActionResult> Move([FromRoute]string path, [FromForm]MoveViewModel model)
        {
            path = (path ?? string.Empty).TrimEnd('/');

            if (!ModelState.IsValid)
            {
                TempData[ERROR_MESSAGES] = "InvalidRequest";
                return RedirectToIndexWithQueryString();
            }

            var dst = await _storage.FindFolderAsync(model.Destination);

            if (dst == null)
            {
                TempData[ERROR_MESSAGES] = $"NotFound:{model.Destination}";
                return RedirectToIndexWithQueryString();
            }

            if (model.Folders != null && model.Folders.Count() == 1)
            {
                var srcPath = $"{path}/{model.Folders.First()}";
                var src = await _storage.FindFolderAsync(srcPath);

                await _storage.MoveAsync(src, dst, model.Name);
            }

            else if (model.Files != null && model.Files.Count() == 1)
            {
                var srcPath = $"{path}/{model.Files.First()}";
                var src = await _storage.FindFileAsync(srcPath);

                await _storage.MoveAsync(src, dst, model.Name);
            }

            else
            {
                TempData[ERROR_MESSAGES] = $"NotFound:{path}";
            }

            return RedirectToIndexWithQueryString();
        }

        [HttpPost("/files/{*path}")]
        [SubmitAction("rm")]
        public async Task<IActionResult> Remove([FromRoute]string path, [FromForm]RemoveViewModel model)
        {
            path = (path ?? string.Empty).TrimEnd('/');

            if (!ModelState.IsValid)
            {
                TempData[ERROR_MESSAGES] = "InvalidRequest";
                return RedirectToIndexWithQueryString();
            }

            var folder = await _storage.FindFolderAsync(path);

            if (folder == null)
            {
                TempData[ERROR_MESSAGES] = $"NotFound:{path}";
                return RedirectToIndexWithQueryString();
            }

            if (model.Folders != null && model.Folders.Count() == 1)
            {
                var srcPath = $"{path}/{model.Folders.First()}";
                var src = await _storage.FindFolderAsync(srcPath);

                if (src != null)
                    await _storage.RemoveAsync(src);
            }

            else if (model.Files != null && model.Files.Count() == 1)
            {
                var srcPath = $"{path}/{model.Files.First()}";
                var src = await _storage.FindFileAsync(srcPath);

                if (src != null)
                    await _storage.RemoveAsync(src);
            }

            else
            {
                TempData[ERROR_MESSAGES] = $"NotFound:{path}";
            }

            return RedirectToIndexWithQueryString();
        }
        
        [HttpPost("/files/{*path}")]
        [SubmitAction("upload")]
        public async Task<IActionResult> Upload([FromRoute]string path, [FromForm]IEnumerable<IFormFile> files)
        {
            if (!ModelState.IsValid)
            {
                TempData[ERROR_MESSAGES] = "InvalidRequest";
                return RedirectToIndexWithQueryString();
            }
            
            var folder = await _storage.FindFolderAsync(path ?? string.Empty);

            if (folder == null)
            {
                TempData[ERROR_MESSAGES] = $"NotFound:{path}";
                return RedirectToIndexWithQueryString();
            }
            
            var children = await _storage.GetFilesAsync(folder);
            var errors = new List<string>();
            var successes = new List<string>();
            foreach (var file in files)
            {
                var name = file.FileName ?? file.Name;
                if (children.Any(f => f.Name == name))
                {
                    errors.Add($"Conflict:{name}");
                    continue;
                }

                try
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var added = await _storage.AddFileAsync(folder, name, file.ContentType, stream);
                        successes.Add($"Upload:{added.Name}");
                    }
                }
                catch (Exception)
                {
                    errors.Add($"Unknown:{name}");
                }
            }

            if (errors.Count > 0)
                TempData[ERROR_MESSAGES] = TempData[ERROR_MESSAGES] + "," + string.Join(",", errors);

            if (successes.Count > 0)
                TempData[SUCCESS_MESSAGES] = string.Join(",", successes);

            return RedirectToIndexWithQueryString();
        }

        [HttpGet("/raw/{*path}")]
        public async Task<IActionResult> Raw([FromRoute]string path)
        {
            var file = await _storage.FindFileAsync(path);

            if (file == null)
                return NotFound();
            
            var stream = _storage.OpenFile(file);

            return File(stream, file.ContentType);
        }

        [HttpGet("/details/files/{*path}")]
        public async Task<IActionResult> Preview([FromRoute]string path, [FromQuery]Select select = Select.None)
        {
            var file = await _storage.FindFileAsync(path);
            if (file == null)
                return NotFound();

            var model = (file, path);

            if (select == Select.File)
                return View("FileSelectPreview", model);

            return View("FilePreview", model);
        }
    }
}