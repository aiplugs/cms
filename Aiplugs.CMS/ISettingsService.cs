using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS 
{
  public interface ISettingsService
  {
    Settings GetSettings();
    bool Validate(Settings settings);
    void Update(Settings settings);
    IQueryable<Settings> GetHistory();
    
    IEnumerable<Collection> GetCollections();
    Collection GetCollection(string collection);
    bool Validate(Collection collection);
    long Add(Collection collection);
    void Update(Collection collection);
    IQueryable<Collection> GetHistory(long id);
  }
}