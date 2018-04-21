using System;
using System.Linq;
using Aiplugs.CMS.Core.Query;
using Pidgin;
using Xunit;


namespace Aiplugs.CMS.Tests
{
    public class QueryBuilderTest
    {
        [Fact]
        public void SQLiteTest()
        {
            var builder = new Core.Data.Sqlite.QueryBuilder();
            Assert.Equal(" json_extract(Data, '$.value') = 100", builder.Build(QParser.Parse("$.value = 100").Expression));
            Assert.Equal(" json_extract(Data, '$.Text') like '%'", builder.Build(QParser.Parse("$.Text like '%'").Expression));
            Assert.Equal(" json_extract(Data, '$.value') = 100 and  json_extract(Data, '$.value') = 100", 
                                builder.Build(QParser.Parse("$.value = 100 and $.value = 100").Expression));
            Assert.Equal(" json_extract(Data, '$.value') = 100 and ( json_extract(Data, '$.value') = 100 or  json_extract(Data, '$.value') = 100)", 
                                builder.Build(QParser.Parse("$.value = 100 and ($.value = 100 or $.value = 100)").Expression));
        }
    }
}   