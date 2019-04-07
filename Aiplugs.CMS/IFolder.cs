using System.Collections.Generic;

namespace Aiplugs.CMS
{
  public interface IFolder
  {
    string Id { get; set; }
    string Name { get; }
    string Path { get; }
  }
}