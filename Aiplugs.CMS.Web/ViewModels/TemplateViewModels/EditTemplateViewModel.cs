using System.ComponentModel.DataAnnotations;

namespace Aiplugs.CMS.Web.ViewModels.TemplateViewModels
{
    public class EditTemplateViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string CurrentId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Template { get; set; }
    }
}
