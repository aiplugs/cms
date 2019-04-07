using Aiplugs.CMS.Web.Binders;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Linq;

namespace Aiplugs.CMS.Web.Extensions
{
    public static class FormExtensions
    {
        public static JToken ToJToken(this IFormCollection form, string schema)
            => ToJToken(form, JSchema.Parse(schema));

        public static JToken ToJToken(this IFormCollection form, JSchema schema)
        {
            return JTokenBinder.Extract(form.AsEnumerable(), schema);
        }
    }
}
