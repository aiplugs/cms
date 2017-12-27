using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiplugs.CMS.Web.Models
{
  public class Folder : IFolder
  {

    public const int HOME = 1;
    [NotMapped]
    public bool IsHome { get { return Id == HOME; } }
    
    [Key]
    public long Id { get; set; }

    public long? ParentId { get; set; }

    [Required]
    public string Name { get; set; }
    
    [ForeignKey(nameof(ParentId))]
    public virtual Folder Parent { get; set; }

    public virtual ICollection<IFolder> Children { get; set; }
    public virtual ICollection<IFile> Files { get; set; }
  }
}