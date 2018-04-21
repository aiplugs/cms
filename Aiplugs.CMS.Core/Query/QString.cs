namespace Aiplugs.CMS.Core.Query
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