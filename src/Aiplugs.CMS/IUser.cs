using System.Collections.Generic;

namespace Aiplugs.CMS
{
  public interface IUser
  {
    string Id { get; }
    string Name { get; }
    string Email { get; }
    IEnumerable<string> Roles { get; }
  }
}