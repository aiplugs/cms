using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiplugs.CMS.Data.Entities
{
    public class Folder
    {
        [Key]
        public string Id { get; set; }

        public string Path { get; set; }
    }
}
