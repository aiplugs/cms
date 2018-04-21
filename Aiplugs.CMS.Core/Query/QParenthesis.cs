namespace Aiplugs.CMS.Core.Query
{
    public class QParenthesis : IQuery
    {
        public IQuery Expression { get; }
        public QParenthesis(IQuery expression)
        {
            Expression = expression;
        }
    }
}