using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Aiplugs.CMS.Core.Models
{
    public class AppConfiguration : IAppConfiguration
    {
        public string UploadRootPath { get; }  

        public AppConfiguration(IConfiguration config)
        {
            UploadRootPath = config["UploadRootPath"] ?? CreateDefaultUploadRootIfNotExist();
        }

        string CreateDefaultUploadRootIfNotExist()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            path = Path.Combine(path, ".aiplugs");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            path = Path.Combine(path, "cms");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            return path;
        }
    }
}