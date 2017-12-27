namespace Aiplugs.CMS.Web.Models.JsonSearch
{
  public class QString : IQuery 
  { 
    public string Value { get; } 
    public QString(string value)
    {
      Value = value;
    }
  }
}