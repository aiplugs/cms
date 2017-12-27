namespace Aiplugs.CMS.Web.Models.JsonSearch
{
  public class QExpression : IQuery
  {
    public QConn Conn { get; }
    public IQuery Left { get; }
    public IQuery Right { get; }
    public QExpression(QConn conn, IQuery left, IQuery right)
    {
      Conn = conn;
      Left = left;
      Right = right;
    }
  }
}