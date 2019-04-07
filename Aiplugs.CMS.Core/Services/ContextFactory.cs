using System;
using System.Threading;
using Aiplugs.CMS.Core.Models;
using Aiplugs.CMS.Data.Entities;
using Aiplugs.CMS.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aiplugs.CMS.Core.Services
{
    public class ContextFactory : IContextFactory
    {
        private readonly IServiceProvider _provider;
        public ContextFactory(IServiceProvider provider)
        {
            _provider = provider;
        }
        public IContext Create(Job job, ILogger logger, CancellationToken token, Action<int> onProgress)
        {
            var resolver = new StaticUserResolver(job.CreatedBy);
            var data = _provider.GetRequiredService<IDataRepository>();
            var settings = _provider.GetRequiredService<ISettingsRepository>();
            var validator = _provider.GetRequiredService<IValidationService>();
            var config = _provider.GetRequiredService<IAppConfiguration>();
            var files = _provider.GetRequiredService<IFileRepository>();
            var folders = _provider.GetRequiredService<IFolderRepository>();
            return new Context
            {
                Logger = logger,
                CancellationToken = token,
                Progress = new Progress<int>(onProgress),
                DataService = new DataService(data, resolver, validator),
                StorageService = new StorageService(config, files, folders, resolver),
                SettingsService = new SettingsService(config, settings, resolver, validator),
                Parameters = job.GetParameters<ContextParameters>(),
            };
        }
    }
}