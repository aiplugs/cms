using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Web.Models
{
  public class Record
  {    
    [Key]
    public long Id { get; set; }
    
    public long ItemId { get; set; }
    
    [Column(TypeName="json")]
    public string JSON { get; set; }

    [Required]
    public string CreatedBy { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTimeOffset CreatedAt { get; set; }

    [ForeignKey(nameof(ItemId))]
    public virtual Item Item { get; set; }

    [NotMapped]
    public JObject Data
    {
      get
      {
        return JObject.Parse(JSON);
      }
      set
      {
        JSON = value.ToString(Formatting.None);
      }
    }
  }
}