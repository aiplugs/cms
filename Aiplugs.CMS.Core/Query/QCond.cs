namespace Aiplugs.CMS.Core.Query
{
  public class QCond : IQuery
  {
    public QOp Op { get; }
    public string Member { get; }
    public IQuery Value { get; }
    public QCond(QOp op, string m, IQuery v)
    {
      Op = op;
      Member = m;
      Value = v;
    }
  }
}