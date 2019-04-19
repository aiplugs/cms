namespace Aiplugs.CMS.Query
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