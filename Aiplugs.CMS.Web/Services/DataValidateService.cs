using System;
using System.IO;
using System.Net.Http;
using Aiplugs.CMS.Web.Data;
using Aiplugs.CMS.Web.Repositories;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Aiplugs.CMS.Web.Services
{
  public class DataValidateService : IDataValidateService
  {
    private readonly ISettingsRepository _settings;
    private readonly IHostingEnvironment _env;

    public DataValidateService(ISettingsRepository settingsRepository, IHostingEnvironment hostingEnvironment) {
      _settings = settingsRepository;
      _env = hostingEnvironment;
    }
    public bool ValidateCollection(string collectionName, JObject data)
    {
      var collection = _settings.GetCollection(collectionName);
      if (collection == null)
        throw new ArgumentException($"Collection({collectionName}) is not found");
      
      return Validate(JsonSchema.Parse(collection.Schema), data);
    }

    public bool Validate(string fileName, JObject data) 
    {
      var json = File.ReadAllText(Path.Combine(_env.ContentRootPath, $"wwwroot/schema/{fileName}"));
      return Validate(JsonSchema.Parse(json), data);
    }

    public bool Validate(Uri schema, JObject data) 
    {
      using(var client = new HttpClient())
      {
        return Validate(JsonSchema.Parse(client.GetStringAsync(schema).Result), data);
      }
    }
    public bool Validate(JsonSchema schema, JObject data)
    {
      if (schema == null)
        throw new ArgumentNullException(nameof(schema));

      if (data == null)
        throw new ArgumentNullException(nameof(data));

      return data.IsValid(schema);
    }
  }
}