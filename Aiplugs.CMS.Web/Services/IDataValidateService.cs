using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Aiplugs.CMS.Web.Services
{
  public interface IDataValidateService
  {
    bool Validate(Uri schema, JObject data);
    bool Validate(string fileName, JObject data);
    bool ValidateCollection(string collectionName, JObject data);
      
  }
}