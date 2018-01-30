using System.ComponentModel.DataAnnotations;

namespace Aiplugs.CMS.Web.ViewModels.StorageViewModels
{
  public class RenameViewModel
  {
    [Required]    
    public string Name { get; set; }  
  }
}