namespace Aiplugs.CMS.Core.Query
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