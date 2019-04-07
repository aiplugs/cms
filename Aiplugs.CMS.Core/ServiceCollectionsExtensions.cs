using System;
using Aiplugs.CMS.Core.Models;
using Aiplugs.CMS.Core.Services;
using Aiplugs.CMS.Data.QueryBuilders;
using Aiplugs.CMS.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Aiplugs.CMS.Core
{
    public static class ServiceCollecitonsExtensions
    {
        public static IServiceCollection AddAiplugsCMS(this IServiceCollection services, Func<OptionsBuilder, OptionsBuilder> options)
        {
            var opts = options(new OptionsBuilder());

            //services.AddTransient<IMigration>(provider => opts.MigrationFactory(provider.GetRequiredService<IDbConnection>()));
            services.AddTransient<IQueryBuilder, SqliteQueryBuilder>();
            services.AddTransient<IDataRepository, DataRepository>();
            services.AddTransient<IFolderRepository, FolderRepository>();
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<ISettingsRepository, SettingsRepository>();
            services.AddTransient<ITemplateRepository, TemplateRepository>();
            services.AddTransient<IJobRepository, JobRepository>();
            services.AddTransient<IAppConfiguration, AppConfiguration>();
            services.AddTransient<IContextFactory, ContextFactory>();
            services.AddTransient<ILockService, LockService>();
            services.AddTransient<IUserResolver>(_ => new StaticUserResolver(Guid.Empty.ToString()));
            services.AddSingleton<IJobRegistory, JobRegistory>();
            services.AddTransient<IProcedureService, ProcedureSerivce>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<IStorageService, StorageService>();
            services.AddTransient<IDataService, DataService>();
            services.AddTransient<IJobService, JobService>();
            services.AddTransient<ITemplateService, TemplateService>();
            services.AddHostedService<JobHostedService>();

            //services.AddAiplugsFunctions(_ => opts.FunctionOptionsBuilder);

            return services;
        }
    }
}