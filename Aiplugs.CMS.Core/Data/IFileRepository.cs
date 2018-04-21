using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Core.Data
{
    public interface IFileRepository
    {
        Task<File> LookupAsync(long id);
        Task<IEnumerable<File>> GetChildrenAsync(long folderId, long? skipToken, int limit);
        Task<File> FindChildAsync(long folderId, string name);
        Task<long> AddAsync(long folderId, string name, string binaryPath, string  contentType, long size, string lastModifiedBy, DateTime lastModifiedAt);
        Task UpdateAsync(long id, long folderId, string name, string  contentType, long size, string lastModifiedBy, DateTime lastModifiedAt);
        Task RemoveAsync(long id);
    }
}