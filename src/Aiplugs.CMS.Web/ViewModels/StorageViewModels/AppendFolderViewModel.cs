using System.ComponentModel.DataAnnotations;

namespace Aiplugs.CMS.Web.ViewModels.StorageViewModels
{
  public class AppendFolderViewModel
  {
    [Required]    
    public string Name { get; set; }  
  }
}