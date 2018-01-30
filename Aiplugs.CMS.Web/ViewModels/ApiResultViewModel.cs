using System.Collections.Generic;

namespace Aiplugs.CMS.Web.ViewModels
{
  public class ApiResultViewModel<TEntity>
  {
    public IEnumerable<TEntity> Result { get; set; }
    public string Prev { get; set; }
    public string Next { get; set; }
  }
}