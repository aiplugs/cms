using System;

namespace Aiplugs.CMS {
  public interface IFile
  {
    string Name { get; set; }
    string ContentType { get; set; }
    byte[] Binary { get; set; }
    int Size { get; set; }
    string LastModifiedBy { get; set; }
    DateTimeOffset LastModifiedAt { get; set; }
  }
}