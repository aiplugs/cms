using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aiplugs.CMS.Web.ViewModels.StorageViewModels
{
    public class MoveViewModel
    {
        [Required]
        public string Destination { get; set; }

        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        public IEnumerable<string> Files { get; set; }

        public IEnumerable<string> Folders { get; set; }
    }
}