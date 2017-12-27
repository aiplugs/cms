using System;
using System.Net.Http;
using Aiplugs.CMS.Web.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Aiplugs.CMS.Web.Services
{
  public class DataValidateService : IDataValidateService
  {
    private readonly ISettingsService settings;
    public DataValidateService(ISettingsService service) {
      settings = service;
    }
    public bool Validate(string collectionName, JObject data)
    {
      var collection = settings.GetCollection(collectionName);
      if (collection == null)
        throw new ArgumentException($"Collection({collectionName}) is not found");
      
      return Validate(collection.Schema, data);
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