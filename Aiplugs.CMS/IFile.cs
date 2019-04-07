using System;

namespace Aiplugs.CMS {
  public interface IFile
  {
    string Id { get; set; }
    string Name { get; set; }
    string ContentType { get; set; }
    long Size { get; set; }
    string LastModifiedBy { get; set; }
    DateTimeOffset LastModifiedAt { get; set; }
  }
}