using System;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS
{
    public interface IItem
    {
        string Id { get; }
        JToken Data { get; }
        DateTimeOffset CreatedAt { get; }
        DateTimeOffset UpdatedAt { get; }
        string CreatedBy { get; }
        string UpdatedBy { get; }
        bool IsValid { get; }
        string CurrentId { get; set; }
    }
}