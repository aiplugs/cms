using System.Collections.Generic;

namespace Aiplugs.CMS.Web.Repositories
{
  public interface ISettingsRepository
  {
      IEnumerable<Collection> GetCollections();
      Collection FindCollection(long id);
      Collection GetCollection(string name);
      void Add(Collection collection);
      void Replace(Collection collection);
      void Remove(Collection collection);
  }
}