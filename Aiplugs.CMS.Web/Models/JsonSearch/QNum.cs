namespace Aiplugs.CMS.Web.Models.JsonSearch
{
  public class QNum : IQuery 
  { 
    public decimal Value { get; } 
    public QNum(decimal value)
    {
      Value = value;
    }
  }
}