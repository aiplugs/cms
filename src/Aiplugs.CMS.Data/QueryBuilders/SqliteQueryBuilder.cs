using System;
using System.Linq;
using System.Text;
using Aiplugs.CMS.Query;

namespace Aiplugs.CMS.Data.QueryBuilders
{
    public class SqliteQueryBuilder : IQueryBuilder
    {
        private readonly string _colName;
        public SqliteQueryBuilder()
            : this("Data")
        {}
        public SqliteQueryBuilder(string colName)
        {
            _colName = colName;
        }
        public string Build(IQuery query)
        {
            var sb = new StringBuilder();
            
            Build(sb, query);

            return sb.ToString();
        }
        internal string ConvertValue(IQuery value)
        {
            if (value is QNum n)
                return n.Value.ToString();
            
            if (value is QString s)
                return $"'{s.Value}'";
            
            if (value is QArray arr)
                return $"[{string.Join(",", arr.Values.Select(v => ConvertValue(v)))}]";

            throw new NotSupportedException($"Not supported value type: {value.GetType().Name}");
        }
        internal string ConvertOp(QOp op)
        {
            switch(op.Value)
            {
                default:
                    return op.Value;
            }
        }
        internal string ConvertConn(QConn conn)
        {
            switch(conn.Value)
            {
                default:
                    return conn.Value;
            }
        }
        internal void Build(StringBuilder sb, IQuery query)
        {
            if (query is QCond cond)
            {
                if (cond.Op.Value == "has")
                {
                    sb.Append($" i.Id IN (SELECT Items.Id FROM Items, json_each(Items.{_colName}, '{cond.Member}') WHERE json_each.value = {ConvertValue(cond.Value)})");
                }
                else
                {
                    sb.Append($" json_extract({_colName}, '{cond.Member}') {ConvertOp(cond.Op)} {ConvertValue(cond.Value)}");
                }
            }
            else if (query is QComplex complex)
            {
                Build(sb, complex.Left);
                sb.Append($" {ConvertConn(complex.Conn)} ");
                Build(sb, complex.Right);
            }

            else if (query is QParenthesis ps)
            {
                sb.Append("(");
                Build(sb, ps.Expression);
                sb.Append(")");
            }
        }   
    }
}