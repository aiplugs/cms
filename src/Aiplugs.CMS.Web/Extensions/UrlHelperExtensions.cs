using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aiplugs.CMS.Web.Controllers;

namespace Microsoft.AspNetCore.Mvc
{
    public static class UrlHelperExtensions
    {
        // public static string EmailConfirmationLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        // {
        //     return urlHelper.Action(
        //         action: nameof(AccountController.ConfirmEmail),
        //         controller: "Account",
        //         values: new { userId, code },
        //         protocol: scheme);
        // }

        // public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string code, string scheme)
        // {
        //     return urlHelper.Action(
        //         action: nameof(AccountController.ResetPassword),
        //         controller: "Account",
        //         values: new { userId, code },
        //         protocol: scheme);
        // }

        public static string CollectionListLink(this IUrlHelper urlHelper, string collectionName)
        {
            return urlHelper.Action("List", "Collections", new { name = collectionName});
        }
        public static string CollectionDataLink(this IUrlHelper urlHelper, string collectionName, string id)
        {
            return urlHelper.Action("Data", "Collections", new { name = collectionName, id });
        }
        public static IEnumerable<(string name, string link)> Breadcrumbs(this IUrlHelper urlHelper,  string path, string action = "Index", string controller = "Files")
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var splited = path.Split("/").Where(s => !string.IsNullOrEmpty(s)).ToArray();

            return new[]
            {
                ("Home", urlHelper.Action(action, controller, new { path = "" }))
            }.Concat(
                splited.Select((s, i) => (s, urlHelper.Action(action, controller, new {
                    path = string.Join("/", splited.Take(i + 1))
                }))
            ));
        }
        private static string ItemLink(this IUrlHelper urlHelper, string path, string name, string action, string controller)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (name == null)
                throw new ArgumentNullException(nameof(path));

            var splited = path.Split("/").Where(s => !string.IsNullOrEmpty(s)).ToArray();

            return urlHelper.Action(action, controller, new { path = string.Join("/", splited.Concat(new[] { name })) }).Replace("%2F", "/");
        }
        public static string FolderLink(this IUrlHelper urlHelper, string path, string name, string action = "Index", string controller = "Files")
            => ItemLink(urlHelper, path, name, action, controller);
        public static string FilePreviewLink(this IUrlHelper urlHelper, string path, string name, string action = "Preview", string controller = "Files")
            => ItemLink(urlHelper, path, name, action, controller);
        public static string FolderPreviewLink(this IUrlHelper urlHelper, string path, string name, string action = "Fodlers", string controller = "Preview")
            => ItemLink(urlHelper, path, name, action, controller);
        public static string DatumPreviewLink(this IUrlHelper urlHelper, string path, string name, string action = "Data", string controller = "Preview")
            => ItemLink(urlHelper, path, name, action, controller);
    }
}
