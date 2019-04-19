using Aiplugs.CMS.Query;

namespace Aiplugs.CMS.Data.QueryBuilders
{
    public interface IQueryBuilder
    {
        string Build(IQuery query);
    }
}