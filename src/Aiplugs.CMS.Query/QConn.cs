namespace Aiplugs.CMS.Query
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