using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Aiplugs.CMS.Web.Models
{
  public class JObjectBinder : IModelBinder
  {
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
      // bindingContext.HttpContext.Request.Form
      throw new System.NotImplementedException();
    }
  }
}