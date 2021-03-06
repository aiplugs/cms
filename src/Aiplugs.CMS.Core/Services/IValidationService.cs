using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Core.Services
{
    public interface IValidationService
    {
        Task<bool> ValidateAsync(Uri schemaUri, JToken data);
        Task<(bool,IEnumerable<string>)> ValidateAsync(string collectionName, JToken data);
    }
}