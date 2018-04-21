using System;
using System.Data;
using Aiplugs.CMS.Core.Data;
using Aiplugs.CMS.Core.Models;
using Aiplugs.CMS.Core.Services;
using Aiplugs.Functions.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Aiplugs.CMS.Core
{
    public static class ServiceCollecitonsExtensions
    {
        public static IServiceCollection AddAiplugsCMS(this IServiceCollection services, Func<OptionsBuilder, OptionsBuilder> options)
        {
            var opts = options(new OptionsBuilder());

            services.AddTransient<IMigration>(provider => opts.MigrationFactory(provider.GetRequiredService<IDbConnection>()));
            services.AddTransient<IDataRepository>(provider => opts.DataRepositoryFactory(provider.GetRequiredService<IDbConnection>()));
            services.AddTransient<IFolderRepository>(provider => opts.FolderRepositoryFactory(provider.GetRequiredService<IDbConnection>()));
            services.AddTransient<IFileRepository>(provider => opts.FileRepositoryFactory(provider.GetRequiredService<IDbConnection>()));
            services.AddTransient<ISettingsRepository, SettingsRepository>();
            services.AddTransient<IAppConfiguration, AppConfiguration>();
            services.AddTransient<IContextFactory<IContextParameters>, ContextFactory>();
            services.AddTransient<ILockService, LockService>();
            services.AddTransient<IUserResolver, CurrentUserResolver>();
            services.AddTransient<IProcedureResolver, ProcedureSerivce>();
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<IStorageService, StorageService>();
            services.AddTransient<IDataService, DataService>();

            services.AddAiplugsFunctions(_ => opts.FunctionOptionsBuilder);

            return services;
        }
    }
}