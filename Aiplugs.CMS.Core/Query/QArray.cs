using System.Collections.Immutable;
using System.Linq;

namespace Aiplugs.CMS.Core.Query
{
  public class QArray : IQuery
  { 
    public ImmutableArray<IQuery> Values { get; }
    public QArray(ImmutableArray<IQuery> values)
    {
      Values = values;
    }
  }
}