using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Newtonsoft.Json.Linq;
using System;

namespace Aiplugs.CMS.Web.Binders
{
    public class JTokenBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.Metadata.ModelType == typeof(JToken))
                return new BinderTypeModelBinder(typeof(JTokenBinder));

            return null;
        }
    }
}
