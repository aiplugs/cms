using System.Collections.Generic;

namespace Aiplugs.CMS.Web.Repositories
{
  public interface IFolderRepository
  {
    IFolder GetHome();
    IFolder Find(long id);
    IFolder Find(IEnumerable<string> path);
    void Add(IFolder parent, string name);
    void Rename(IFolder folder, string name);
    void Remove(IFolder folder);
  }
}