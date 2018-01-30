using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Aiplugs.CMS.Web.Repositories;
using System;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json;

namespace Aiplugs.CMS.Web.Services 
{
  public class SettingsService : ISettingsService
  {
    const string COLLECTIONS = "__collections__";
    const string SETTINGS = "__settings__";
    private readonly ISettingsRepository _settings;
    private readonly IDataValidateService _validator;
    public SettingsService(ISettingsRepository settingsRepository, IDataValidateService dataValidateService)
    {
      _settings = settingsRepository;
      _validator = dataValidateService;
    }
    public void Add(Collection collection)
    {
      _settings.Add(collection);
    }

    public Collection FindCollection(long id)
    {
      return _settings.FindCollection(id);
    }

    public Collection GetCollection(string name)
    {
      return _settings.GetCollection(name);
    }

    public IEnumerable<Collection> GetCollections()
    {
      return _settings.GetCollections();
    }

    public IEnumerable<Settings> GetHistory()
    {
      throw new System.NotImplementedException();
    }

    public IEnumerable<Collection> GetHistory(long id)
    {
      throw new System.NotImplementedException();
    }

    public Settings GetSettings()
    {
      throw new System.NotImplementedException();
    }

    public void Update(Settings settings)
    {
      throw new System.NotImplementedException();
    }

    public void Update(Collection collection)
    {
      _settings.Replace(collection);
    }

    public bool Validate(Settings settings)
    {
      throw new System.NotImplementedException();
    }

    public bool Validate(Collection collection)
    {
      if (_validator.Validate("collection.json", JObject.FromObject(collection)) == false)
        return false;
      
      JObject data = null;
      try {
        data = JObject.Parse(collection.Schema);
      }
      catch(JsonReaderException ex) {
        return false;
      }
      return _validator.Validate(new Uri("http://schemas.aiplugs.com/cms/2017-10-09/root.json"), data);
    }
  }
}