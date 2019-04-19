using Aiplugs.CMS.Core.Services;
using Aiplugs.CMS.Data.Repositories;
using Aiplugs.CMS.Query;
using Newtonsoft.Json;
using RazorLight.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Core.RazorLight
{
    public class AiplugsRazorProject : RazorLightProject
    {
        public readonly ITemplateRepository _repository;
        public AiplugsRazorProject(ITemplateRepository repository)
        {
            _repository = repository;
        }

        public override Task<IEnumerable<RazorLightProjectItem>> GetImportsAsync(string templateKey)
        {
            return Task.FromResult(Enumerable.Empty<RazorLightProjectItem>());
        }

        public override async Task<RazorLightProjectItem> GetItemAsync(string templateKey)
        {
            var template = await _repository.FindAsync(templateKey);
            return new AiplugsRazorProjectItem(templateKey, template.Text);
        }
    }
}
