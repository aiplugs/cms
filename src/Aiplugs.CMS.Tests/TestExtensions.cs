using Aiplugs.CMS.Data.Entities;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Tests
{
    public static class TestExtensions
    {
        public static string Data(this Item item, string prop) => (string)JToken.Parse(item.Data)[prop];
        public static string Data(this ItemRecord record, string prop) => (string)JToken.Parse(record.Data)[prop];
    }
}
