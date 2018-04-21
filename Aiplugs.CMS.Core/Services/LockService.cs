using System.IO;
using System.Threading.Tasks;
using Aiplugs.Functions.Core;

namespace Aiplugs.CMS.Core.Services
{
    public class LockService : ILockService
    {
        private readonly IStorageService _storage;
        public LockService(IStorageService storageService)
        {
            _storage = storageService;
        }
        
        public async Task<bool> LockAsync(string name)
        {
            var folder = await _storage.FindFolderAsync("/locks");
            if (folder == null)
            {
                var home = await _storage.GetHomeAsync();
                folder = await _storage.AddFolderAsync(home, "locks");
            }

            try
            {
                await _storage.AddFileAsync(folder, name, "text/plain", new MemoryStream(new byte[] { 0 }));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task UnlockAsync(string name)
        {
            var file = await _storage.FindFileAsync($"/locks/{name}");
            
            if (file == null)
                return;

            await _storage.RemoveAsync(file);
        }
    }
}