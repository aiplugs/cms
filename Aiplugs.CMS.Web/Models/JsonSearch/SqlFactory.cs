namespace Aiplugs.CMS.Web.Models.JsonSearch
{
  public class SqlFactory
  {
    public static IFormmater Formatter { get; set; } = new SqliteFormatter();

    public static string Generate(IQuery query)
    {
      return Formatter.Format(query);
    }
    public static string Generate(string jsonQuery)
    {
      return Generate(QParser.Parse(jsonQuery));
    }
  }
}