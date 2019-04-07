using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Core.Models;
using Aiplugs.CMS.Data.Repositories;
using SysFile = System.IO.File;

namespace Aiplugs.CMS.Core.Services
{
    using File = Models.File;
    using Folder = Models.Folder;
    public class StorageService : IStorageService
    {
        private readonly string _home;
        private readonly IFileRepository _files;
        private readonly IFolderRepository _folders;
        private readonly IUserResolver _resolver;
        public StorageService(IAppConfiguration config, IFileRepository fileRepository, IFolderRepository folderRepository, IUserResolver userResolver)
        {
            _home = config.UploadRootPath;
            _files = fileRepository;
            _folders = folderRepository;
            _resolver = userResolver;
        }
        private string GetLast(string path)
        {
            return path.Split('/').Where(p => string.IsNullOrEmpty(p) == false).LastOrDefault() ?? "Home";
        }
        private string CreateBinaryPath()
        {
            var dir = Path.Combine(_home, $"{DateTime.UtcNow:yyyyMM}");
            
            if (Directory.Exists(dir) == false)
                Directory.CreateDirectory(dir);

            return Path.Combine(dir, Guid.NewGuid().ToString());
        }
        public async Task<IFile> AddFileAsync(IFolder folder, string name, string contentType, Stream binary)
        {
            if (folder == null)
                throw new ArgumentNullException(nameof(folder));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
                
            if (name.Contains("/"))
                throw new ArgumentException("Cannot use path delimiter('/')", nameof(name));
            
            if (string.IsNullOrEmpty(contentType))
                throw new ArgumentNullException(nameof(contentType));
            
            if (binary == null)
                throw new ArgumentNullException(nameof(binary));

            var userId =  _resolver.GetUserId();

            if (userId == null)
                throw new UnauthorizedAccessException();

            var path = CreateBinaryPath();
            using(var stream = SysFile.OpenWrite(path))
            {
                await binary.CopyToAsync(stream);
            }
            var size = new FileInfo(path).Length;
            var file = await _files.AddAsync(folder.Id, name, path, contentType, size, userId, DateTimeOffset.UtcNow);

            return Mapping.Mapper.Map<File>(file);
        }

        public async Task ReplaceFileAsync(IFile file, string contentType, Stream binary)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            if (string.IsNullOrEmpty(contentType))
                throw new ArgumentNullException(nameof(contentType));
            
            if (binary == null)
                throw new ArgumentNullException(nameof(binary));

            var userId =  _resolver.GetUserId();

            if (userId == null)
                throw new UnauthorizedAccessException();

            var _file = file as File;
            var path = ((File)file).BinaryPath;
            using(var stream = SysFile.OpenWrite(path))
            {
                await binary.CopyToAsync(stream);
            }
            var size = new FileInfo(path).Length;

            await _files.UpdateMetaAsync(file.Id, contentType, size, userId, DateTimeOffset.UtcNow);
        }

        public async Task<IFolder> AddFolderAsync(IFolder folder, string name)
        {
            if (folder == null)
                new ArgumentNullException(nameof(folder));

            if (string.IsNullOrWhiteSpace(name))
                new ArgumentNullException(nameof(name));
                
            if (name.Contains("/"))
                throw new ArgumentException("Cannot use path delimiter('/')", nameof(name));

            var path = folder.Path + name + "/";
            var added = await _folders.AddAsync(path);

            return Mapping.Mapper.Map<Folder>(added);
        }

        public async Task<IFile> FindFileAsync(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var names = path.Split('/').Where(s => string.IsNullOrEmpty(s) == false).ToArray();

            if (names.Length < 1)
                throw new ArgumentOutOfRangeException(nameof(path));

            var head = names.Take(names.Length - 1).ToArray();
            var tail = names.Last();
            var folderPath = $"/{string.Join("/", head)}/".Replace("//", "/");
            var folder = await _folders.GetAsync(folderPath);

            if (folder == null)
                return null;
            
            var file = await _files.FindChildAsync(folder.Id, tail);

            if (file == null)
                return null;

            return new File
            {
                Id = file.Id,
                Path = folder.Path + tail,
                Name = file.Name,
                ContentType = file.ContentType,
                Size = file.Size,
                LastModifiedBy = file.LastModifiedBy,
                LastModifiedAt = file.LastModifiedAt,
                BinaryPath = file.BinaryPath,
                FolderId = file.FolderId
            };
        }

        public async Task<IFolder> FindFolderAsync(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (path.StartsWith("/") == false)
                path = "/" + path;

            if (path.EndsWith("/") == false)
                path = path + "/";
            
            var folder = await _folders.GetAsync(path);

            if (folder == null)
                return null;

            return new Folder
            {
                Id = folder.Id,
                Path = folder.Path,
                Name = GetLast(folder.Path),
            };
        }

        public async Task<IFolder> GetHomeAsync()
        {
            return await FindFolderAsync("/");
        }

        public async Task<IFile> LookupFileAsync(string id)
        {
            var file = await _files.LookupAsync(id);
            return Mapping.Mapper.Map<File>(file);
        }

        public async Task<IFolder> LookupFolderAsync(string id)
        {
            var folder = await _folders.LookupAsync(id);
            return new Folder
            {
                Id = folder.Id,
                Path = folder.Path,
                Name = GetLast(folder.Path),
            };
        }

        public Stream OpenFile(IFile file)
        {   
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            return SysFile.Open(((File)file).BinaryPath, FileMode.Open, FileAccess.ReadWrite);
        }

        public async Task RemoveAsync(IFolder folder)
        {
            if (folder == null)
                throw new ArgumentNullException(nameof(folder));

            var files = await GetFilesAsync(folder.Path);

            foreach (var file in files)
            {
                await _files.RemoveAsync(file.Id);
            }

            var folders = await GetFoldersAsync(folder.Path);

            foreach (var child in folders)
                await RemoveAsync(child);

            await _folders.RemoveAsync(folder.Id); 
        }

        public async Task RemoveAsync(IFile file)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            await _files.RemoveAsync(file.Id);

            SysFile.Delete(((File)file).BinaryPath);
        }

        public async Task MoveAsync(IFolder folder, IFolder dst, string name)
        {
            if (folder == null)
                throw new ArgumentNullException(nameof(folder));

            if (dst == null)
                throw new ArgumentNullException(nameof(dst));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            
            if (name.Contains('/'))
                throw new ArgumentException("Cannot use path delimiter('/')", nameof(name));

            await _folders.UpdateAsync(folder.Id, dst.Path + name + "/");
        }

        public async Task MoveAsync(IFile file, IFolder dst, string name)
        {
            if (file == null)
                throw new ArgumentNullException(nameof(file));

            if (dst == null)
                throw new ArgumentNullException(nameof(dst));

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            
            if (name.Contains('/'))
                throw new ArgumentException("Cannot use path delimiter('/')", nameof(name));

            var userId = _resolver.GetUserId();

            if (userId == null)
                throw new UnauthorizedAccessException();

            await _files.UpdatePathAsync(file.Id, dst.Id, name, userId, DateTime.UtcNow);
        }

        public async Task<IEnumerable<IFolder>> GetFoldersAsync(string path, string skipToken = null, int limit = 100)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var folders = await _folders.GetChildrenAsync(path, skipToken, limit);
            return folders.Select(folder => new Folder
            {
                Id = folder.Id,
                Path = folder.Path,
                Name = GetLast(folder.Path)
            });
        }

        public async Task<IEnumerable<IFolder>> GetFoldersAsync(IFolder parent, string skipToken = null, int limit = 100)
        {
            return await GetFoldersAsync(parent.Path, skipToken, limit);
        }

        public async Task<IEnumerable<IFile>> GetFilesAsync(string path, string skipToken = null, int limit = 100)
        {
            var folder = await FindFolderAsync(path);
            
            return await GetFilesAsync(folder, skipToken, limit);
        }

        public async Task<IEnumerable<IFile>> GetFilesAsync(IFolder parent, string skipToken = null, int limit = 100)
        {
            var files = await _files.GetChildrenAsync(parent.Id, skipToken, limit);
            
            return files.Select(Mapping.Mapper.Map<File>).ToArray();
        }
    }
}
