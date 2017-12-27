namespace Aiplugs.CMS {
  public interface IStorageService
  {
    IFolder GetHome();    
    IFolder GetFolder(string path);
    IFile GetFile(string path);

    void Add(IFolder parent, string name);
    void Add(IFolder folder, string name, string contentType, byte[] binary);

    void Rename(IFolder folder, string name);
    void Rename(IFile file, string name);

    void Remove(IFolder folder);
    void Remove(IFile file);
  }
}