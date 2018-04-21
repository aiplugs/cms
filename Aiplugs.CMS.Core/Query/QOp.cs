namespace Aiplugs.CMS.Core.Query
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