using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Aiplugs.CMS.Web.Attributes
{
    public class SubmitActionAttribute : ActionMethodSelectorAttribute
    {
        public string SubmitName { get; set; }

        public SubmitActionAttribute(string submitName)
        {
            SubmitName = submitName;
        }

        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            if (routeContext.HttpContext.Request.Method.ToUpper() != "POST")
                return false;

            var form = routeContext.HttpContext.Request.Form;

            if (form.TryGetValue(SubmitName, out var value) && !string.IsNullOrEmpty(value))
                return true;

            return form.TryGetValue("ic-submit-element-name", out var ic) && ic == SubmitName;
        }
    }
}
