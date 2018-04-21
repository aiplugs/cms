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
        public static string CollectionDataLink(this IUrlHelper urlHelper, string collectionName, long id)
        {
            return urlHelper.Action("Data", "Collections", new { name = collectionName, id });
        }
    }
}
