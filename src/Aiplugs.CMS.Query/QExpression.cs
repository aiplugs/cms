namespace Aiplugs.CMS.Query
{
  public class QComplex : IQuery
  {
    public QConn Conn { get; }
    public IQuery Left { get; }
    public IQuery Right { get; }
    public QComplex(QConn conn, IQuery left, IQuery right)
    {
      Conn = conn;
      Left = left;
      Right = right;
    }
  }
}