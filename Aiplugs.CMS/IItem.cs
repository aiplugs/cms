using System;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS
{
    public interface IItem
    {
        long Id { get; set; }
        JObject Data { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
        string UpdatedBy { get; set; }
        bool IsValid { get; set; }
    }
}