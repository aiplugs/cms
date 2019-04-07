using System;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS
{
    public interface IRecord
    {
        string Id { get; }
        JToken Data { get; }
        DateTimeOffset CreatedAt { get; }
        string CreatedBy { get; }
    }
}