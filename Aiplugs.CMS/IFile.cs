using System;

namespace Aiplugs.CMS {
  public interface IFile
  {
    long Id { get; set; }
    string Name { get; set; }
    string ContentType { get; set; }
    long Size { get; set; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedAt { get; set; }
  }
}