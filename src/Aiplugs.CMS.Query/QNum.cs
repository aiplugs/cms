namespace Aiplugs.CMS.Query
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