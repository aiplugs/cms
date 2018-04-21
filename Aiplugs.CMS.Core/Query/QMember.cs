namespace Aiplugs.CMS.Core.Query
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