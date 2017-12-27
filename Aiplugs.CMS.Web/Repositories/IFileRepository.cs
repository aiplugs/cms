using System.Collections.Generic;

namespace Aiplugs.CMS.Web.Repositories
{
  public interface IFileRepository
  {
      IFile Find(long id);
      void Add(IFolder parent, string name, string contentType, byte[] binary);
      void Replace(IFile file, string name, string contentType, byte[] binary);
      void Remove(IFile file);
  }
}