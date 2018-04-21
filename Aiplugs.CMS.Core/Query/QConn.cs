namespace Aiplugs.CMS.Core.Query
{
  public class QConn : IQuery
  {
    public string Value { get; }
    public QConn(string value)
    {
      Value = value;
    }
  }
}