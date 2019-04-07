using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Aiplugs.CMS
{
    public interface IStorageService
    {
        Task<IFolder> GetHomeAsync();
        Task<IFolder> FindFolderAsync(string path);
        Task<IEnumerable<IFolder>> GetFoldersAsync(string path, string skipToken = null, int limit = 100);
        Task<IEnumerable<IFolder>> GetFoldersAsync(IFolder parent, string skipToken = null, int limit = 100);
        Task<IFile> FindFileAsync(string path);
        Task<IEnumerable<IFile>> GetFilesAsync(string path, string skipToken = null, int limit = 100);
        Task<IEnumerable<IFile>> GetFilesAsync(IFolder parent, string skipToken = null, int limit = 100);
        Task<IFolder> LookupFolderAsync(string id);
        Task<IFile> LookupFileAsync(string id);
        Stream OpenFile(IFile file);

        Task<IFolder> AddFolderAsync(IFolder folder, string name);
        Task<IFile> AddFileAsync(IFolder folder, string name, string contentType, Stream binary);
        Task ReplaceFileAsync(IFile file, string contentType, Stream binary);
        

        Task MoveAsync(IFolder folder, IFolder dst, string name);
        Task MoveAsync(IFile file, IFolder dst, string name);

        Task RemoveAsync(IFolder folder);
        Task RemoveAsync(IFile file);
    }
}