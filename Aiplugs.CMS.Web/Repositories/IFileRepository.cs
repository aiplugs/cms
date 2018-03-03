using System.Collections.Generic;

namespace Aiplugs.CMS.Web.Repositories
{
  public interface IFileRepository
  {
      byte[] LoadBinary(IFile file);
      void Add(IFolder parent, string name, string contentType, byte[] binary);
      void Replace(IFile file, string name, string contentType, byte[] binary);
      void Remove(IFile file);
  }
}