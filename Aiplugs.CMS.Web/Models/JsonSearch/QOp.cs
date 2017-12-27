namespace Aiplugs.CMS.Web.Models.JsonSearch
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