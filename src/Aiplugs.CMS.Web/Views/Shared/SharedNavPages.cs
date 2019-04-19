using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;

namespace Aiplugs.CMS.Web.Views.Shared
{
    public static class SharedNavPages
    {
        public static string ActivePageKey => "ActivePage";

        public static string Index => "Index";
        public static string Files => "Files";
        public static string Templates => "Templates";
        public static string Jobs => "Jobs";
        public static bool IndexNavActive(ViewContext viewContext) => PageNavActive(viewContext, Index);
        public static bool FilesNavActive(ViewContext viewContext) => PageNavActive(viewContext, Files); 
        public static bool TemplatesNavActive(ViewContext viewContext) => PageNavActive(viewContext, Templates);
        public static bool JobsNavActive(ViewContext viewContext) => PageNavActive(viewContext, Jobs);

        public static class Settings
        {
            public static string AppSettings => "Settings_AppSettings";
            public static string Users => "Settings_Users";
            public static string Collections => "Settings_Collections";
            public static bool AppSettingsNavActive(ViewContext viewContext) => PageNavActive(viewContext, AppSettings);
            public static bool UsersNavActive(ViewContext viewContext) => PageNavActive(viewContext, Users);
            public static bool CollectionsNavActive(ViewContext viewContext) => PageNavActive(viewContext, Collections);
        }

        public static class Collections
        {
            public static string ResolveName(string collectionName) => $"Collections_{collectionName}";
            public static bool NavActive(ViewContext viewContext, string collectionName) => PageNavActive(viewContext, ResolveName(collectionName));
        }
        
        public static bool PageNavActive(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string;
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase);
        }

        public static void AddActivePage(this ViewDataDictionary viewData, string activePage) => viewData[ActivePageKey] = activePage;
    }
}
