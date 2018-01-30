using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Aiplugs.CMS.Web.Data;
using Aiplugs.CMS.Web.Models;
using Aiplugs.CMS.Web.Services;
using Aiplugs.CMS.Web.Repositories;
using Aiplugs.CMS.Web.Filters;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Aiplugs.CMS.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddTransient<IFolderRepository, FolderRepository>();
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<IItemRepository, ItemRepository>();
            services.AddTransient<ISettingsRepository, SettingsRepository>();
            services.AddTransient<IUserManageService, UserManageService>();
            services.AddTransient<IStorageService, StorageService>();
            services.AddTransient<IDataValidateService, DataValidateService>();
            services.AddTransient<IDataService, DataService>();
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddScoped<SharedDataLoad>();

            services.AddMvc();

            services.AddAuthentication()
                    .AddCookie();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
