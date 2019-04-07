using Aiplugs.CMS.Query;
using Pidgin;
using System;
using System.Linq;
using Xunit;


namespace Aiplugs.CMS.Tests
{
    public class JsonSearchQueryParserTest
    {
        [Fact]
        public void ParseNum()
        {
            Assert.Equal(100, ((QNum)QParser.Num.Parse("100").Value).Value);
            Assert.Equal(-100, ((QNum)QParser.Num.Parse("-100").Value).Value);
            Assert.Equal(100, ((QNum)QParser.Num.Parse("100.0").Value).Value);
            Assert.Equal(-100, ((QNum)QParser.Num.Parse("-100.0").Value).Value);
        }

        [Fact]
        public void ParseString()
        {
            Assert.Equal("hoge fuga", QParser.String2.Parse("\"hoge fuga\"").Value);
            Assert.Equal("hoge fuga", QParser.String3.Parse("'hoge fuga'").Value);

            Assert.Equal("hogefuga", ((QString)QParser.String.Parse("hogefuga").Value).Value);
            Assert.Equal("hoge fuga", ((QString)QParser.String.Parse("'hoge fuga'").Value).Value);
            Assert.Equal("hoge fuga", ((QString)QParser.String.Parse("\"hoge fuga\"").Value).Value);
        }

        [Fact]
        public void ParseArray()
        {
            Assert.True(new decimal[] { 0, 1, 2 }.SequenceEqual(((QArray)QParser.Array.Parse("[0, 1, 2]").Value).Values.Select(v => ((QNum)v).Value)));
            Assert.True(new[] { "hoge", "fuga" }.SequenceEqual(((QArray)QParser.Array.Parse("[hoge, fuga]").Value).Values.Select(v => ((QString)v).Value)));
        }

        [Fact]
        public void ParseOp()
        {
            Assert.True(QParser.NumOp.Parse("=").Success);
            Assert.True(QParser.NumOp.Parse(">").Success);
            Assert.True(QParser.NumOp.Parse("<").Success);
            var result = QParser.NumOp.Parse(">=");
            Assert.True(QParser.NumOp.Parse(">=").Success);
            Assert.True(QParser.NumOp.Parse("<=").Success);
            Assert.True(QParser.NumOp.Parse("has").Success);
            Assert.True(QParser.StringOp.Parse("has").Success);
            Assert.True(QParser.StringOp.Parse("like").Success);
            Assert.True(QParser.ArrayOp.Parse("in").Success);
        }

        [Fact]
        public void ParseCond()
        {
            Assert.True(QParser.Cond1.Parse("= 100").Success);
            Assert.True(QParser.Cond1.Parse("> 100").Success);
            Assert.True(QParser.Cond1.Parse("< 100").Success);
            Assert.True(QParser.Cond1.Parse(">= 100").Success);
            Assert.True(QParser.Cond1.Parse("<= 100").Success);
            Assert.True(QParser.Cond123.Parse("= 100").Success);
            Assert.True(QParser.Cond2.Parse("= hoge").Success);
            Assert.True(QParser.Cond2.Parse("has hoge").Success);
            Assert.True(QParser.Cond2.Parse("like hoge").Success);
            Assert.True(QParser.Cond123.Parse("= hoge").Success);
            Assert.True(QParser.Cond123.Parse("has hoge").Success);
            Assert.True(QParser.Cond123.Parse("like hoge").Success);
            Assert.False(QParser.Cond.Parse("val = 100").Success);
            Assert.True(QParser.Cond.Parse("$.val = 100").Success);
            Assert.True(QParser.Cond.Parse("$.val > 100").Success);
            Assert.True(QParser.Cond.Parse("$.val < 100").Success);
            Assert.True(QParser.Cond.Parse("$.val >= 100").Success);
            Assert.True(QParser.Cond.Parse("$.val <= 100").Success);
            Assert.False(QParser.Cond.Parse("$.val in 100").Success);
            Assert.True(QParser.Cond.Parse("$.val in [100, 200]").Success);
            Assert.False(QParser.Cond.Parse("$.val = [100, 200]").Success);
            Assert.True(QParser.Cond.Parse("$.val has hoge").Success);
            Assert.True(QParser.Cond.Parse("$.val like '%hoge%'").Success);
            Assert.True(QParser.Cond.Parse("$.hoge.fuga = 100").Success);
            Assert.True(QParser.Cond.Parse("$.hoge.fuga = 'hoge fuga'").Success);
            Assert.True(QParser.Cond.Parse("$['hoge'].fuga = 100").Success);
            Assert.True(QParser.Cond.Parse("$[\"hoge\"].fuga = 100").Success);
            Assert.True(QParser.Cond.Parse("$['hoge fuga'].fuga = 100").Success);
            Assert.True(QParser.Cond.Parse("$[\"hoge fuga\"].fuga = 100").Success);
        }
        [Fact]
        public void ParseParenthesis()
        {
            Assert.True(QParser.Parenthesis.Parse("($.val = 100)").Success);
            Assert.True(QParser.Parenthesis.Parse("($.val = 100 or $.val = 100)").Success);
            Assert.True(QParser.Parenthesis.Parse("($.val = 100 or $.val = 100 or $.val = 100)").Success);
        }

        [Fact]
        public void ParseExpression()
        {
            Assert.True(QParser.Atom.Parse("$.val = 100").Success);
            Assert.True(QParser.Expression.Parse("$.val = 100").Success);
            Assert.True(QParser.Expression.Parse("$.val = 100 and $.val = 100").Success);
            Assert.True(QParser.Expression.Parse("$.val = 100 and $.val = 100 and $.val = 100").Success);
            Assert.NotNull(QParser.Expression.Parse("$.val = 100 and $.val = 100").Value as QComplex);
            Assert.True(QParser.Expression.Parse("$.val = 100 and ( $.val = 100 or $.val = 100 )").Success);
            Assert.True(QParser.Expression.Parse("( $.val = 100 or $.val = 100 ) and ( $.val = 100 or $.val = 100 )").Success);
        }
    }
}
