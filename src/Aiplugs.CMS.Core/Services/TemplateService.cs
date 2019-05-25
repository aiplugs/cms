using Aiplugs.CMS.Core.RazorLight;
using Aiplugs.CMS.Data.Repositories;
using Newtonsoft.Json.Linq;
using RazorLight;
using RazorLight.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Core.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly ITemplateRepository _repository;
        private readonly IUserResolver _userResolver;
        private readonly RazorLightEngine _engine;

        public TemplateService(ITemplateRepository repository, IUserResolver userResolver, PropertyInjector injector)
        {
            _repository = repository;
            _userResolver = userResolver;

            _engine = new RazorLightEngineBuilder()
                .UseProject(new AiplugsRazorProject(repository))
                .UseMemoryCachingProvider()
                .Build();
            
            _engine.Options.PreRenderCallbacks.Add(template => injector.Inject(template));
        }

        public async Task<IEnumerable<Template>> GetAsync()
        {
            return await _repository.GetAsync();
        }

        public async Task<Template> FindAsync(string name)
        {
            return await _repository.FindAsync(name);
        }

        public async Task<Template> LookupAsync(string id)
        {
            return await _repository.LookupAsync(id);
        }

        public async Task AddAsync(string name, string template)
        {
            var userId = _userResolver.GetUserId();
            await _repository.AddAsync(name, template, userId, DateTimeOffset.Now);
        }

        public async Task UpdateAsync(string id, string currentId, string name, string template)
        {
            var userId = _userResolver.GetUserId();
            await _repository.Update(id, currentId, name, template, userId, DateTimeOffset.Now);
        }

        public async Task RemoveAsync(string id, string currentId)
        {
            await _repository.RemoveAsync(id, currentId);
        }

        public async Task<string> RenderAsync<TModel>(string templateName, TModel model)
        {
            return await _engine.CompileRenderAsync(templateName, model);
        }
    }
}
