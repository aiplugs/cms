using Aiplugs.CMS.Core.Query;

namespace Aiplugs.CMS.Core.Data
{
    public interface IQueryBuilder
    {
        string Build(IQuery query);
    }
}