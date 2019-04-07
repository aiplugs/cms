using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KeyValues = System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>>;

namespace Aiplugs.CMS.Web.Binders
{
    public class JTokenBinder : IModelBinder
    {
        public JSchema Schema;
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var form = bindingContext.HttpContext.Request.Form;

            var model = Extract(form.AsEnumerable(), Schema);

            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
        public static string ConcatPrefix(string prefix, string name)
               => string.IsNullOrEmpty(prefix) ? name : prefix + "." + name;

        public static KeyValues FilterKeyValues(KeyValues keyValues, string prefix)
            => keyValues.Where(pair => pair.Key.StartsWith(prefix));

        public static JToken Extract(KeyValues keyValues, JSchema schema, string prefix = "")
        {
            if (schema.Type == JSchemaType.Object)
            {
                var jobj = new JObject();
                var props = new List<KeyValuePair<string, StringValues>>();
                foreach (var prop in schema.Properties)
                {
                    var propPrefix = ConcatPrefix(prefix, prop.Key);
                    var filtered = FilterKeyValues(keyValues, propPrefix);
                    jobj[prop.Key] = Extract(filtered, prop.Value, propPrefix);
                    props.AddRange(filtered);
                }

                var remains = keyValues.Where(pair => !props.Any(prop => pair.Key == prop.Key)).ToArray();

                if (remains.Length > 0 && schema.PatternProperties != null && schema.PatternProperties.Keys.Count() > 0)
                {
                    var groups = remains.GroupBy(remain =>
                    {
                        var b = remain.Key.IndexOf('[', prefix.Length);
                        var d = remain.Key.IndexOf('.', prefix.Length);
                        var length = b != -1 ? Math.Min(b, d) + 1 : d != -1 ? d + 1 : remain.Key.Length;
                        return remain.Key.Substring(prefix.Length, length - prefix.Length);
                    }, remain => remain);

                    foreach (var group in groups)
                    {
                        var itemSchema = schema.PatternProperties.Where(prop => Regex.IsMatch(group.Key, prop.Key)).ToArray();
                        if (itemSchema.Length == 0)
                            continue;

                        jobj[group.Key] = Extract(group, itemSchema.First().Value, ConcatPrefix(prefix, group.Key));
                    }
                }

                return jobj;
            }
            else if (schema.Type == JSchemaType.Array)
            {
                if (schema.Items.Count > 1)
                    throw new NotSupportedException("Not specified items schema or indeterminate schema");

                var item = schema.Items?.FirstOrDefault() ?? new JSchema { Type = JSchemaType.String };

                var jarray = new JArray();

                // Primitive types
                if (item.Type != JSchemaType.Array && item.Type != JSchemaType.Object)
                {
                    foreach (var pair in keyValues)
                    {
                        foreach (var value in pair.Value)
                        {
                            jarray.Add(Extract(new[] { new KeyValuePair<string, StringValues>(pair.Key, new[] { value }) }, item, prefix));
                        }
                    }

                    return jarray;
                }

                // Nested types
                var groups = keyValues.GroupBy(
                    pair => pair.Key.Substring(0, pair.Key.IndexOf(']', prefix.Length) + 1),
                    pair => pair
                ).ToArray();


                foreach (var group in groups)
                {
                    jarray.Add(Extract(group, item, group.Key));
                }

                return jarray;
            }
            else
            {
                var count = keyValues.Count();
                if (count > 1)
                    throw new ArgumentOutOfRangeException(nameof(keyValues));

                if (count == 0)
                    return null;

                var pair = keyValues.First();

                if (pair.Value.Count() > 1)
                    throw new ArgumentOutOfRangeException(nameof(keyValues));

                var value = pair.Value.FirstOrDefault();

                if (value == null)
                    return null;

                switch (schema.Type)
                {
                    case JSchemaType.Boolean:
                        if (bool.TryParse(value, out var b))
                            return b;
                        return null;

                    case JSchemaType.Integer:
                        if (long.TryParse(value, out var l))
                            return l;
                        return null;

                    case JSchemaType.Number:
                        if (double.TryParse(value, out var d))
                            return d;
                        return null;

                    default: return value;
                }
            }
        }
    }
}
