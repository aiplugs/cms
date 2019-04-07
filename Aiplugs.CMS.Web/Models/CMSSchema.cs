using Aiplugs.Elements;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Collections.Generic;
using System.Linq;

namespace Aiplugs.CMS.Web.Models
{
    public class CMSSchema
    {
        private JObject _jobject;
        private JSchema _jschema;

        public CMSSchema(string json)
        {
            _jobject = JObject.Parse(json);
            _jschema = JSchema.Parse(json);
        }

        public CMSSchema(JObject jobject)
        {
            _jobject = jobject;
            _jschema = JSchema.Parse(jobject.ToString());
        }

        public JSchema AsJSchema() => _jschema;

        public string Title => _jschema.Title;
        public string Description => _jschema.Description;
        public string MonacoLanguage => (string)_jobject["input"]?["monaco"]?["language"] ?? "text";
        public JObject Monaco => (JObject)_jobject["input"]?["monaco"] ?? new JObject();
        public JSchemaType? Type => _jschema.Type;
        public CMSSchema Items => new CMSSchema((JObject)_jobject["items"] ?? new JObject());
        public IDictionary<string, CMSSchema> Properties
            => (_jobject?["properties"] as JObject)
                .Properties()
                .ToDictionary(o => o.Name, o => new CMSSchema(o.Value as JObject));
        public IDictionary<string, CMSSchema> PatternProperties
            => (_jobject?["patternProperties"] as JObject)
                .Properties()
                .ToDictionary(o => o.Name, o => new CMSSchema(o.Value as JObject));

        public IEnumerable<SelectListItem> GetSelectList(string current)
            => GetSelectList(new[] { current });

        public IEnumerable<SelectListItem> GetSelectList(JToken token)
            => GetSelectList(token?.AsEnumerable()?.Select(t => t.ToString()));

        public IEnumerable<SelectListItem> GetSelectList(IEnumerable<string> currents)
        {
            if (_jschema.Enum == null)
                return null;

            var labels = (JObject)_jobject?["enumLabels"];

            return _jschema.Enum.Select(e =>
            {
                var value = e.ToString();
                var text = labels.ContainsKey(value) ? labels[value] : value;

                return new SelectListItem { Text = text?.ToString(), Value = value, Selected = currents.Contains(value) };
            });
        }

        public CMSSchemaInput Input => _jobject["input"]?.ToObject<CMSSchemaInput>();
        public bool Unique => ((bool?)_jobject["unique"]) ?? false;
        public Ajax AutoComplete
        {
            get
            {
                var obj = _jobject["autocomplete"];

                if (obj == null)
                    return null;

                return new Ajax
                {
                    Url = obj["url"].ToString(),
                    Label = obj["labelKey"].ToString(),
                    Value = obj["valueKey"].ToString(),
                    Headers = obj["customHeaders"].ToObject<Dictionary<string, string>>()
                };
            }
        }
    }

    public class CMSSchemaInput
    {
        public string Type { get; set; }
        public double? Step { get; set; }
        public JObject Code { get; set; }
        public JObject Wysiwyg { get; set; }

        public Elements.Ajax Ajax { get; set; }

        public bool IsText => Type == "type";
        public bool IsTextArea => Type == "typearea";
        public bool IsWysiwyg => Type == "wysiwyg";
        public bool IsCode => Type == "code";
        public bool IsDatetimeLocal => Type == "datetime-local";
        public bool IsDate => Type == "date";
        public bool IsEmail => Type == "email";
        public bool IsCheckbox => Type == "checkbox";
        public bool IsRadio => Type == "radio";
        public bool IsSelect => Type == "select";
        public bool IsTag => Type == "tag";
    }
}
