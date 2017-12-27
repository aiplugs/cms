using System;
using System.Linq;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Aiplugs.CMS.Web.Models.JsonSearch
{
  public class QParser
  {
    private static readonly Parser<char, char> LBracket = Char('[');
    private static readonly Parser<char, char> RBracket = Char(']');
	private static readonly Parser<char, char> LParenthesis = Char('(');
    private static readonly Parser<char, char> RParenthesis = Char(')');
    private static readonly Parser<char, char> DQ = Char('"');
    private static readonly Parser<char, char> SQ = Char('\'');
    private static readonly Parser<char, string> Eq = String("=");
    private static readonly Parser<char, string> Lt = String("<");
    private static readonly Parser<char, string> Gt = String(">");
    private static readonly Parser<char, string> Le = String("<=");
    private static readonly Parser<char, string> Ge = String(">=");
    private static readonly Parser<char, string> In = String("in");
    private static readonly Parser<char, string> Has = String("has");
    private static readonly Parser<char, string> Like = String("like");
    private static readonly Parser<char, string> And = String("and");
    private static readonly Parser<char, string> Or = String("or");
    private static readonly Parser<char, char> Comma = Char(',');
    private static readonly Parser<char, char> Dot = Char('.');
    private static readonly Parser<char, char> Sign = Char('-').Or(Char('+'));
    private static readonly Parser<char, long> Degits = Digit.AtLeastOnceString().Select(s => long.Parse(s));
    private static readonly Parser<char, decimal> Num1 = from sign in Sign.Optional()
                              from d in Degits
                              select decimal.Parse($"{(sign.HasValue?sign.Value.ToString():"")}{d}");
    private static readonly Parser<char, decimal> Num2 = from sign in Sign.Optional()
                                                         from d in Digit.AtLeastOnceString()
                                                         from dot in Dot.Optional()
                                                         from f in Digit.AtLeastOnceString()
                                                         select decimal.Parse($"{(sign.HasValue?sign.Value.ToString():"")}{d}{(dot.HasValue?".":"")}{f}");
    private static readonly Parser<char, IQuery> Num = Num1.Or(Num2).Select<IQuery>(n => new QNum(n));
	private static readonly char[] StrIgnoreChars = new[]{',',' ','[',']'};
    private static readonly Parser<char, string> String1 = Token(c => !StrIgnoreChars.Contains(c)).ManyString().Between(SkipWhitespaces);
    private static readonly Parser<char, string> String2 = Token(c => c != '"').ManyString().Between(DQ);
    private static readonly Parser<char, string> String3 = Token(c => c != '\'').ManyString().Between(SQ);
    private static readonly Parser<char, IQuery> String = String1.Or(String2).Or(String3).Select<IQuery>(s => new QString(s));
    private static readonly Parser<char, QOp> Op = Eq.Or(Lt).Or(Gt).Or(Le).Or(Ge).Or(In).Or(Has).Or(Like).Select(o => new QOp(o));
    private static readonly Parser<char, QConn> Conn = And.Or(Or).Select(o => new QConn(o));
    private static readonly Parser<char, QOp> OpWhitespace = Op.Between(SkipWhitespaces);
    private static readonly Parser<char, QConn> ConnWhitespace = Conn.Between(SkipWhitespaces);
	private static readonly Parser<char, IQuery> Value = Num.Or(String);
	private static readonly Parser<char, IQuery> Array = Value.Between(SkipWhitespaces)
																		.Separated(Comma)
																		.Between(LBracket, RBracket)
																		.Select<IQuery>(els => new QArray(els));
	private static readonly Parser<char, IQuery> Cond = from m in String1
																  from o in OpWhitespace
																  from v in Value.Or(Array)
																  select (IQuery)new QCond(o, m, v);
	private static readonly Parser<char, IQuery> Parenthesis = Cond.Or(Rec(() =>Expression)).Between(LParenthesis, RParenthesis);
	private static readonly Parser<char, IQuery> Atom = Cond.Or(Parenthesis);
	private static readonly Parser<char, IQuery> Expression = from l in Atom
																		from c in ConnWhitespace
																		from r in Atom
																		select (IQuery)new QExpression(c, l, r);
    

    

    public static IQuery Parse(string query)
    {
      var result = Expression.Parse(query);
      if (result.Success == false) 
        throw new ApplicationException($"Cannot parse query: {result.Error.ToString()}");

      return result.Value;
    }
  }
}