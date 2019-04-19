namespace Aiplugs.CMS.Query
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