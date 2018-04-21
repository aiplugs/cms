using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Core.Services
{
    public interface IValidationService
    {
        Task<bool> ValidateAsync(Uri schemaUri, JObject data);
        Task<bool> ValidateAsync(string collectionName, JObject data);
    }
}