using System;
using System.Linq;
using Aiplugs.CMS.Core.Query;
using Pidgin;
using Xunit;


namespace Aiplugs.CMS.Tests
{
    public class JsonSearchQueryParserTest
    {
        [Fact]
        public void ParseNum()
        {
            Assert.Equal(100, ((QNum)((Result<char, IQuery>)QParser.Num.Parse("100")).Value).Value);
            Assert.Equal(-100, ((QNum)((Result<char, IQuery>)QParser.Num.Parse("-100")).Value).Value);
            Assert.Equal(100, ((QNum)((Result<char, IQuery>)QParser.Num.Parse("100.0")).Value).Value);
            Assert.Equal(-100, ((QNum)((Result<char, IQuery>)QParser.Num.Parse("-100.0")).Value).Value);
        }

        [Fact]
        public void ParseString()
        {
            Assert.Equal("hoge fuga", ((Result<char, string>)QParser.String2.Parse("\"hoge fuga\"")).Value);
            Assert.Equal("hoge fuga", ((Result<char, string>)QParser.String3.Parse("'hoge fuga'")).Value);

            Assert.Equal("hogefuga", ((QString)((Result<char, IQuery>)QParser.String.Parse("hogefuga")).Value).Value);
            Assert.Equal("hoge fuga", ((QString)((Result<char, IQuery>)QParser.String.Parse("'hoge fuga'")).Value).Value);
            Assert.Equal("hoge fuga", ((QString)((Result<char, IQuery>)QParser.String.Parse("\"hoge fuga\"")).Value).Value);
        }

        [Fact]
        public void ParseArray()
        {
            Assert.True(new decimal[]{0,1,2}.SequenceEqual(((QArray)((Result<char, IQuery>)QParser.Array.Parse("[0, 1, 2]")).Value).Values.Select(v => ((QNum)v).Value)));
            Assert.True(new []{"hoge","fuga"}.SequenceEqual(((QArray)((Result<char, IQuery>)QParser.Array.Parse("[hoge, fuga]")).Value).Values.Select(v => ((QString)v).Value)));
        }

        [Fact]
        public void ParseCond()
        {
            Assert.False(((Result<char, IQuery>)QParser.Cond.Parse("val = 100")).Success);
            Assert.True(((Result<char, IQuery>)QParser.Cond.Parse("$.val = 100")).Success);
            Assert.True(((Result<char, IQuery>)QParser.Cond.Parse("$.val > 100")).Success);
            Assert.True(((Result<char, IQuery>)QParser.Cond.Parse("$.val < 100")).Success);
            Assert.True(((Result<char, IQuery>)QParser.Cond.Parse("$.val >= 100")).Success);
            Assert.True(((Result<char, IQuery>)QParser.Cond.Parse("$.val <= 100")).Success);
            Assert.False(((Result<char, IQuery>)QParser.Cond.Parse("$.val in 100")).Success);
            Assert.True(((Result<char, IQuery>)QParser.Cond.Parse("$.val in [100, 200]")).Success);
            Assert.False(((Result<char, IQuery>)QParser.Cond.Parse("$.val = [100, 200]")).Success);
            Assert.True(((Result<char, IQuery>)QParser.Cond.Parse("$.val has hoge")).Success);
            Assert.True(((Result<char, IQuery>)QParser.Cond.Parse("$.val like '%hoge%'")).Success);
            Assert.True(((Result<char, IQuery>)QParser.Cond.Parse("$.hoge.fuga = 100")).Success);
            Assert.True(((Result<char, IQuery>)QParser.Cond.Parse("$.hoge.fuga = 'hoge fuga'")).Success);
            Assert.True(((Result<char, IQuery>)QParser.Cond.Parse("$['hoge'].fuga = 100")).Success);
            Assert.True(((Result<char, IQuery>)QParser.Cond.Parse("$[\"hoge\"].fuga = 100")).Success);
            Assert.True(((Result<char, IQuery>)QParser.Cond.Parse("$['hoge fuga'].fuga = 100")).Success);
            Assert.True(((Result<char, IQuery>)QParser.Cond.Parse("$[\"hoge fuga\"].fuga = 100")).Success);
        }
        [Fact]
        public void ParseParenthesis()
        {
            Assert.True(((Result<char, IQuery>)QParser.Parenthesis.Parse("($.val = 100)")).Success);            
            Assert.True(((Result<char, IQuery>)QParser.Parenthesis.Parse("($.val = 100 or $.val = 100)")).Success);            
            Assert.True(((Result<char, IQuery>)QParser.Parenthesis.Parse("($.val = 100 or $.val = 100 or $.val = 100)")).Success);            
        }

        [Fact]
        public void ParseExpression()
        {
            Assert.True(((Result<char, IQuery>)QParser.Atom.Parse("$.val = 100")).Success);            
            Assert.True(((Result<char, IQuery>)QParser.Expression.Parse("$.val = 100")).Success);            
            Assert.True(((Result<char, IQuery>)QParser.Expression.Parse("$.val = 100 and $.val = 100")).Success);                 
            Assert.True(((Result<char, IQuery>)QParser.Expression.Parse("$.val = 100 and $.val = 100 and $.val = 100")).Success);        
            Assert.NotNull(((Result<char, IQuery>)QParser.Expression.Parse("$.val = 100 and $.val = 100")).Value as QComplex);           
            Assert.True(((Result<char, IQuery>)QParser.Expression.Parse("$.val = 100 and ( $.val = 100 or $.val = 100 )")).Success);        
            Assert.True(((Result<char, IQuery>)QParser.Expression.Parse("( $.val = 100 or $.val = 100 ) and ( $.val = 100 or $.val = 100 )")).Success);        
        }
    }
}
