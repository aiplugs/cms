using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Aiplugs.CMS.Web.Models
{
  public class Item : IItem
  {
    [Key]
    public long Id { get; set; }
    
    [Required]
    public string CollectionName { get; set; }

    public long? CurrentId { get; set; }
    
    [ForeignKey(nameof(CurrentId))]
    public Record Current { get; set; }
  
    public virtual ICollection<Record> History { get; set; } = new List<Record>();

    [NotMapped]
    public JObject Data { get { return Current.Data; } }
  }
}