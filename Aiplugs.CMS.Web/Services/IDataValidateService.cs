using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Aiplugs.CMS.Web.Services
{
  public interface IDataValidateService
  {
    bool Validate(string collection, JObject data);
    bool Validate(JsonSchema schema, JObject data);
  }
}