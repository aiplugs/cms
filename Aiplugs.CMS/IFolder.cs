using System.Collections.Generic;

namespace Aiplugs.CMS
{
  public interface IFolder
  {
    string Name { get; }
    ICollection<IFolder> Children { get; }
    ICollection<IFile> Files { get; }
  }
}