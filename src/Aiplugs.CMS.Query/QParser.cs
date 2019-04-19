using Pidgin;
using System.Collections.Immutable;
using System.Linq;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Aiplugs.CMS.Query
{
    public class QParser
    {
        internal static readonly Parser<char, char> LBracket = Char('[');
        internal static readonly Parser<char, char> RBracket = Char(']');
        internal static readonly Parser<char, char> LParenthesis = Char('(');
        internal static readonly Parser<char, char> RParenthesis = Char(')');
        internal static readonly Parser<char, char> DQ = Char('"');
        internal static readonly Parser<char, char> SQ = Char('\'');
        internal static readonly Parser<char, string> Eq = String("=");
        internal static readonly Parser<char, string> Lt = String("<");
        internal static readonly Parser<char, string> Gt = String(">");
        internal static readonly Parser<char, string> Le = String("<=");
        internal static readonly Parser<char, string> Ge = String(">=");
        internal static readonly Parser<char, string> In = String("in");
        internal static readonly Parser<char, string> Has = String("has");
        internal static readonly Parser<char, string> Like = String("like");
        internal static readonly Parser<char, string> And = String("and");
        internal static readonly Parser<char, string> Or = String("or");
        internal static readonly Parser<char, char> Comma = Char(',');
        internal static readonly Parser<char, char> Dot = Char('.');
        internal static readonly Parser<char, char> Sign = Char('-').Or(Char('+'));
        internal static readonly Parser<char, long> Degits = Digit.AtLeastOnceString().Select(s => long.Parse(s));
        internal static readonly Parser<char, decimal> Num1 = from sign in Sign.Optional()
                                                              from d in Degits
                                                              select decimal.Parse($"{(sign.HasValue ? sign.Value.ToString() : "")}{d}");
        internal static readonly Parser<char, decimal> Num2 = from sign in Sign.Optional()
                                                              from d in Digit.AtLeastOnceString()
                                                              from dot in Dot.Optional()
                                                              from f in Digit.AtLeastOnceString()
                                                              select decimal.Parse($"{(sign.HasValue ? sign.Value.ToString() : "")}{d}{(dot.HasValue ? "." : "")}{f}");
        internal static readonly Parser<char, IQuery> Num = Num1.Or(Num2).Select<IQuery>(n => new QNum(n));
        internal static readonly char[] StrIgnoreChars = new[] { ',', ' ', '[', ']', '\'', '"' };
        internal static readonly Parser<char, string> String1 = Token(c => !StrIgnoreChars.Contains(c)).AtLeastOnceString().Between(SkipWhitespaces);
        internal static readonly Parser<char, string> String2 = Token(c => c != '"').ManyString().Between(DQ);
        internal static readonly Parser<char, string> String3 = Token(c => c != '\'').ManyString().Between(SQ);
        internal static readonly Parser<char, IQuery> String = String3.Or(String2).Or(String1).Select<IQuery>(s => new QString(s));
        internal static readonly char[] MemberIgnoreChars = new[] { '.', ';', ':', ',', ' ', '[', ']', '\'', '"' };
        internal static readonly Parser<char, string> Member1 = Dot.Then(Token(c => !MemberIgnoreChars.Contains(c)).ManyString().Select(s => $".{s}"));
        internal static readonly Parser<char, string> Member2 = String3.Or(String2).Between(LBracket, RBracket).Select(s => $"[{s}]");
        internal static readonly Parser<char, string> Member = from d in String("$")
                                                               from m in Member1.Or(Member2).ManyString()
                                                               select d + m;
        internal static readonly Parser<char, QOp> NumOp = Try(Le).Or(Try(Ge).Or(Try(Lt).Or(Try(Gt).Or(Try(Eq).Or(Has))))).Select(o => new QOp(o));
        internal static readonly Parser<char, QOp> StringOp = Eq.Or(Has).Or(Like).Select(o => new QOp(o));
        internal static readonly Parser<char, QOp> ArrayOp = In.Select(o => new QOp(o));
        internal static readonly Parser<char, QConn> Conn = And.Or(Or).Select(o => new QConn(o));
        internal static readonly Parser<char, QConn> ConnWhitespace = Conn.Between(SkipWhitespaces);
        internal static readonly Parser<char, IQuery> Value = Num.Or(String);
        internal static readonly Parser<char, IQuery> Array = Value.Between(SkipWhitespaces)
                                                                          .Separated(Comma)
                                                                          .Between(LBracket, RBracket)
                                                                          .Select<IQuery>(els => new QArray(els.ToImmutableArray()));
        internal static readonly Parser<char, QCond> Cond1 = from o in NumOp
                                                             from _ in SkipWhitespaces
                                                             from v in Num
                                                             select new QCond(o, "", v);
        internal static readonly Parser<char, QCond> Cond2 = from o in StringOp
                                                             from _ in SkipWhitespaces
                                                             from v in String
                                                             select new QCond(o, "", v);
        internal static readonly Parser<char, QCond> Cond3 = from o in ArrayOp
                                                             from _ in SkipWhitespaces
                                                             from v in Array
                                                             select new QCond(o, "", v);
        internal static readonly Parser<char, QCond> Cond123 = Try(Cond1).Or(Try(Cond2).Or(Cond3)).Between(SkipWhitespaces);
        internal static readonly Parser<char, IQuery> Cond = from m in Member
                                                             from c in Cond123
                                                             select (IQuery)new QCond(c.Op, m, c.Value);
        internal static readonly Parser<char, IQuery> Parenthesis = Rec(() => Expression)
                                                                        .Between(SkipWhitespaces)
                                                                        .Between(LParenthesis, RParenthesis)
                                                                        .Select(e => (IQuery)new QParenthesis(e));
        internal static readonly Parser<char, IQuery> Atom = Cond.Or(Parenthesis);
        internal static readonly Parser<char, IQuery> Complex = from c in ConnWhitespace
                                                                from r in Atom
                                                                select (IQuery)new QComplex(c, null, r);
        internal static readonly Parser<char, IQuery> Expression = from head in Atom
                                                                   from tails in Complex.Many()
                                                                   select tails.Aggregate(head, (l, r) =>
                                                                   {
                                                                       var c = (QComplex)r;
                                                                       return new QComplex(c.Conn, l, c.Right);
                                                                   });
        public static QResult Parse(string query)
        {
            var result = Expression.Parse(query);
            var ret = new QResult { Success = result.Success };

            if (ret.Success)
                ret.Expression = result.Value;

            else
                ret.ErrorMessage = result.Error.ToString();

            return ret;
        }
    }
}