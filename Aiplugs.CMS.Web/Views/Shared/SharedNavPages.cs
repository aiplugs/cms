using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Aiplugs.CMS.Web.Views.Shared
{
  public static class SharedNavPages
  {
    public static string ActivePageKey => "ActivePage";

    public static string Index => "Index";
    public static string Files => "Files";
    public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);
    public static string FilesNavClass(ViewContext viewContext) => PageNavClass(viewContext, Files);

    public static class Settings
    {
      public static string AppSettings => "Settings_AppSettings";
      public static string Users => "Settings_Users";
      public static string Collections => "Settings_Collections";
      public static string AppSettingsNavClass(ViewContext viewContext) => PageNavClass(viewContext, AppSettings);
      public static string UsersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Users);
      public static string CollectionsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Collections);
    }

    public static class Collections
    {
      public static string ResolveName(string collectionName) => $"Collections_{collectionName}";
      public static string NavClass(ViewContext viewContext, string collectionName) => PageNavClass(viewContext, ResolveName(collectionName));
    }

    public static string PageNavClass(ViewContext viewContext, string page)
    {
      var activePage = viewContext.ViewData["ActivePage"] as string;
      return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
    }

    public static void AddActivePage(this ViewDataDictionary viewData, string activePage) => viewData[ActivePageKey] = activePage;
  }
}
