using Aiplugs.CMS.Web.ViewModels.StorageViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Web.Services
{
    public class StoragePagingService : IStoragePagingService
    {
        private readonly IStorageService _storage;

        public StoragePagingService(IStorageService storage)
        {
            _storage = storage;
        }

        public async Task<PageViewModel> GetPageAsync(IFolder parent, string skipToken = null, int limit = 100, bool dirOnly = false)
        {
            var page = new PageViewModel
            {
                Path = parent.Path,
                Folders = Enumerable.Empty<FolderViewModel>(),
                Files = Enumerable.Empty<FileViewModel>()
            };
            var dir = skipToken == null || skipToken.StartsWith("d");
            var item = !dir;

            skipToken = skipToken?.Substring(1);

            if (dir)
            {
                var len = limit;
                var getLen = len + 1;
                var result = (await _storage.GetFoldersAsync(parent, skipToken, getLen)).ToArray();
                var folders = result.Take(len).ToArray();

                item = result.Length < limit;

                page.Folders = folders.Select(f => new FolderViewModel { Name = f.Name }).ToArray();
                page.SkipToken = result.Length == getLen ? $"d{folders.Last().Id}" : null;
            }


            if (item || !dirOnly)
            {
                var len = limit - page.Folders.Count();
                var getLen = len + 1;
                var result = (await _storage.GetFilesAsync(parent, skipToken, getLen)).ToArray();
                var files = result.Take(len).ToArray();

                page.Files = files.Select(f => new FileViewModel { Name = f.Name, Size = f.Size, LastModifiedAt = f.LastModifiedAt, LastModifiedBy = f.LastModifiedBy });
                page.SkipToken = result.Length == getLen ? $"f{files.Last().Id}" : null;
            }
            
            return page;
        }
    }
}
