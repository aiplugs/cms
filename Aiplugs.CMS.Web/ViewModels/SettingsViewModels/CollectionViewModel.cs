using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aiplugs.CMS.Web.ViewModels.SettingsViewModels
{
    public class CollectionViewModel
    {
        [Required]
        [RegularExpression(@"[a-zA-Z][a-zA-Z0-9_]*", ErrorMessage = "Alphabet and number, underscore.")]
        [Display(Name = "Collection Name", Description = "Collection name for data")]
        public string Name { get; set; }

        [Required]
        public string Schema { get; set; }

        [Required]
        public string TitlePath { get; set; }

        public string PreviewTemplate { get; set; }

        [Display(Name = "Display Name", Description = "Collection name for display")]
        public string DisplayName { get; set; }

        public int DisplayOrder { get; set; }

        public IEnumerable<ProcedureInfo> Procedures { get; set; } = new ProcedureInfo[0];

        public string GetDisplayName()
        {
            return DisplayName ?? Name;
        }
    }
}
