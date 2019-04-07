using System.ComponentModel.DataAnnotations;

namespace Aiplugs.CMS.Web.ViewModels.TemplateViewModels
{
    public class NewTemplateViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Template { get; set; }
    }
}
