using System;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS
{
    public interface IRecord
    {
        long Id { get; }
        JObject Data { get; }
        DateTime CreatedAt { get; }
        string CreatedBy { get; set; }
    }
}