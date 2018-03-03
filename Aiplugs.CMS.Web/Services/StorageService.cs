using System.Linq;
using Aiplugs.CMS.Web.Data;
using Aiplugs.CMS.Web.Models;
using Aiplugs.CMS.Web.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Aiplugs.CMS.Web.Services
{
  public class StorageService : IStorageService
  {
    const int HOME = 1;
    private readonly IFolderRepository folders;
    private readonly IFileRepository files;

    public StorageService(IFolderRepository folderRepository, IFileRepository fileRepository)
    {
      folders = folderRepository;
      files = fileRepository;
    }
    public void Add(IFolder parent, string name)
    {
      folders.Add(parent, name);
    }

    public void Add(IFolder folder, string name, string contentType, byte[] binary)
    {
      files.Add(folder, name, contentType, binary);
    }

    public IFile GetFile(string path)
    {
      var _path = path?.Split('/');
      if (_path == null || _path.Length == 0)
        return null;

      var folder = folders.Find(_path.SkipLast(1));
      var fileName = _path.Last();

      return folder.Files.Where(f => f.Name == fileName).FirstOrDefault();
    }

    public IFile LoadFile(IFile file)
    {
      return files.LoadBinary(file);
    }

    public IFolder GetFolder(string path)
    {
      if (string.IsNullOrEmpty(path))
        return folders.GetHome();
      
      return folders.Find(path.Split("/"));
    }

    public IFolder GetHome()
    {
      return folders.GetHome();
    }

    public void Remove(IFolder folder)
    {
      folders.Remove(folder);
    }

    public void Remove(IFile file)
    {
      files.Remove(file);
    }

    public void Rename(IFolder folder, string name)
    {
      folders.Rename(folder, name);
    }

    public void Rename(IFile file, string name)
    {
      files.Replace(file, name, file.ContentType, file.Binary);
    }
  }
}