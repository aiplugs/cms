using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiplugs.CMS.Web.Models
{
  public class File : IFile
  {
    [Key]
    public long Id { get; set; }
    
    public long FolderId { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string ContentType { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset LastModifiedAt { get; set; }
    
    [Required]
    public string LastModifiedBy { get; set; }

    public byte[] Binary { get; set; }
    
    public int Size { get; set; }

    [ForeignKey(nameof(FolderId))]
    public virtual Folder Folder { get; set; }
  }
}