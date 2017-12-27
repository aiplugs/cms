namespace Aiplugs.CMS.Web.Models.JsonSearch
{
  public class QMember : IQuery
  {
    public string Path { get; }
    public QMember(string path)
    {
      Path = path;
    }
  }
}