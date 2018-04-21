using System;

namespace Aiplugs.CMS.Core.Models
{
    public interface IAppConfiguration
    {
        string UploadRootPath { get; }  
        Uri SettingsSchemaUri { get; }  
        Uri CollectionSchemaUri { get; }  
    }
}