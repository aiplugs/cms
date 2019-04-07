namespace Aiplugs.CMS.Query
{
  public class QOp : IQuery
  {
    public string Value { get; }
    public QOp(string value)
    {
      Value = value;
    }
  }
}