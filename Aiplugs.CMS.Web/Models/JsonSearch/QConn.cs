namespace Aiplugs.CMS.Web.Models.JsonSearch
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