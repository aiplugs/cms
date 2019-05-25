using Aiplugs.CMS.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Web.Controllers
{
    public class PreviewController : Controller
    {
        private readonly ITemplateService _templateService;
        private readonly IStorageService _storageService;
        private readonly IDataService _dataService;
        private readonly ISettingsService _settingsService;
        private readonly IUserManageService _userManageService;

        public PreviewController(ITemplateService templateService, IStorageService storageService, IDataService dataService, ISettingsService settingsService, IUserManageService userManageService)
        {
            _templateService = templateService;
            _storageService = storageService;
            _dataService = dataService;
            _settingsService = settingsService;
            _userManageService = userManageService;
        }

        [HttpGet("/preview/files/{*path}")]
        public async Task<IActionResult> File([FromRoute]string path)
        {
            var file = await _storageService.FindFileAsync(path);

            if (file == null)
                return NotFound();

            var model = new FilePreviewViewModel { File = file, Path = path };

            var templateName = nameof(File);
            var template = await _templateService.FindAsync(templateName);

            if (template == null)
                return View(model);

            var result = await _templateService.RenderAsync(nameof(File), model);

            return Content(result, "text/html");
        }

        [HttpGet("/preview/data/{collectionName}/{id}")]
        public async Task<IActionResult> Data([FromRoute]string collectionName, [FromRoute]string id)
        {
            var collection = await _settingsService.FindCollectionAsync(collectionName);
            if (collection == null)
                return NotFound();

            var item = await _dataService.LookupAsync(id);
            if (item == null)
                return NotFound();

            var model = new DataPreviewViewModel
            { 
                Item = item, 
                Collection = collection,
                ResolveUserNameAsync = async userId => (await _userManageService.FindUserAsync(userId))?.Name
            };

            if (string.IsNullOrEmpty(collection.PreviewTemplate))
                return View(model);

            var template = await _templateService.FindAsync(collection.PreviewTemplate);

            if (template == null)
                return View(model);

            var result = await _templateService.RenderAsync(collection.PreviewTemplate, model);

            return Content(result, "text/html");
        }
    }
}
