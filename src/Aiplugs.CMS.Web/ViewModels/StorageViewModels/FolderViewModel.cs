using System;

namespace Aiplugs.CMS.Web.ViewModels.StorageViewModels
{
  public class FolderViewModel
  {
    public string Name { get; set; }
    public DateTimeOffset? LastModifiedAt {get;set;}  
  }
}