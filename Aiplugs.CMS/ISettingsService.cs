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
    IEnumerable<Settings> GetHistory();
    
    IEnumerable<Collection> GetCollections();
    Collection FindCollection(long id);
    Collection GetCollection(string name);
    bool Validate(Collection collection);
    void Add(Collection collection);
    void Update(Collection collection);
    IEnumerable<Collection> GetHistory(long id);
  }
}