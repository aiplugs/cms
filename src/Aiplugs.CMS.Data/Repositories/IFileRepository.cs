using Aiplugs.CMS.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Data.Repositories
{
    public interface IFileRepository
    {
        Task<File> LookupAsync(string id);

        Task<IEnumerable<File>> GetChildrenAsync(string folderId, string skipToken, int limit);

        Task<File> FindChildAsync(string folderId, string name);

        Task<File> AddAsync(string folderId, string name, string binaryPath, string contentType, long size, string userId, DateTimeOffset datetime);

        Task UpdateMetaAsync(string id, string contentType, long size, string userId, DateTimeOffset datetime);

        Task UpdatePathAsync(string id, string folderId, string name, string userId, DateTimeOffset datetime);

        Task RemoveAsync(string id);
    }
}
