using Aiplugs.CMS.Core;
using Aiplugs.CMS.Data;
using Aiplugs.CMS.Data.Entities;
using Aiplugs.CMS.Web.Filters;
using Aiplugs.CMS.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace Aiplugs.CMS.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private CultureInfo[] supportedCultures = new[]
        {
            new CultureInfo("en"),
            new CultureInfo("ja"),
        };

        public void ConfigureServices(IServiceCollection services)
        {
            //var connection = new SqliteConnection("DataSource=:memory:");
            //connection.Open();
            //var options = new DbContextOptionsBuilder<AiplugsDbContext>()
            //        .UseSqlite(connection)
            //        .Options;
            //var context = new AiplugsDbContext(options);
            //context.Database.EnsureCreated();

            services.AddDistributedMemoryCache();

            services.AddDbContext<AiplugsDbContext>();

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<AiplugsDbContext>()
                .AddDefaultTokenProviders();

            services.AddHttpClient();

            services.AddAiplugsCMS(opts => opts.UseSqlite().ForceMigration());

            services.AddScoped<SharedDataLoad>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<IStoragePagingService, StoragePagingService>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddViewLocalization(
                    LanguageViewLocationExpanderFormat.Suffix,
                    opts => { opts.ResourcesPath = "Resources"; })
                .AddDataAnnotationsLocalization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime, AiplugsDbContext dbContext)
        {
            dbContext.Database.Migrate();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Default/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Default}/{action=Index}/{id?}");
            });

            app.UseAiplugsCMS();
        }
    }
}
