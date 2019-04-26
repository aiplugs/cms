using System.ComponentModel.DataAnnotations;

namespace Aiplugs.CMS.Web.ViewModels
{
    public class AdminViewModel
    {
        [Required]
        [Display(Name="Email")]
        public string Email { get; set; }

        [Required]
        [MaxLength(16)]
        [Display(Name="Display Name")]
        public string DisplayName { get; set; }

        [Required]
        [MinLength(16)]
        [Display(Name="Password")]
        public string Password { get; set; }

        [Required]
        [MinLength(16)]
        [Display(Name="Confirm Password")]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}