using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Aiplugs.CMS.Core.Models
{
    public class AppConfiguration : IAppConfiguration
    {
        public string UploadRootPath { get; }  
        public Uri SettingsSchemaUri { get; }  
        public Uri CollectionSchemaUri { get; }  

        public AppConfiguration(IConfiguration config)
        {
            var schemas = config.GetSection("Schemas");
            SettingsSchemaUri = new Uri(schemas["Settings"]);
            CollectionSchemaUri = new Uri(schemas["Collection"]);

            UploadRootPath = config["UploadRootPath"] ?? Path.GetTempPath();
        }
    }
}