using Aiplugs.CMS.Web.Binders;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Schema;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Aiplugs.CMS.Tests
{
    public class JTokenBindTest
    {
        [Fact]
        public void Extract1()
        {
            var schema = JSchema.Parse(schemaJson);

            var keyValues = new[]
            {
                new KeyValuePair<string, StringValues>("name", new []{ "Name" }),
                new KeyValuePair<string, StringValues>("array1[0].key", new []{ "100" }),
                new KeyValuePair<string, StringValues>("array1[0].values[0]", new []{ "Val1" }),
                new KeyValuePair<string, StringValues>("array1[0].values[1]", new []{ "Val2" }),
                new KeyValuePair<string, StringValues>("array1[0].values[2]", new []{ "Val3" }),
                new KeyValuePair<string, StringValues>("array1[1].key", new []{ "101" }),
                new KeyValuePair<string, StringValues>("array1[1].values[0]", new []{ "Val1" }),
                new KeyValuePair<string, StringValues>("array1[1].values[1]", new []{ "Val2" }),
                new KeyValuePair<string, StringValues>("array1[1].values[2]", new []{ "Val3" }),
            };

            var token = JTokenBinder.Extract(keyValues, schema);

            Assert.Equal("Name", (string)token["name"]);
            Assert.Equal(2, token["array1"].Cast<IEnumerable<object>>().Count());
            Assert.Equal(100, (long)token.SelectToken("$.array1[0].key"));
            Assert.Equal("Val1", (string)token.SelectToken("$.array1[0].values[0]"));
            Assert.Equal("Val2", (string)token.SelectToken("$.array1[0].values[1]"));
            Assert.Equal("Val3", (string)token.SelectToken("$.array1[0].values[2]"));
            Assert.Equal(101, (long)token.SelectToken("$.array1[1].key"));
            Assert.Equal("Val1", (string)token.SelectToken("$.array1[1].values[0]"));
            Assert.Equal("Val2", (string)token.SelectToken("$.array1[1].values[1]"));
            Assert.Equal("Val3", (string)token.SelectToken("$.array1[1].values[2]"));
        }

        [Fact]
        public void Extract2()
        {
            var schema = JSchema.Parse(schemaJson);

            var keyValues = new[]
            {
                new KeyValuePair<string, StringValues>("name", new []{ "Name" }),
                new KeyValuePair<string, StringValues>("array1[0].key", new []{ "100" }),
                new KeyValuePair<string, StringValues>("array1[0].values[]", new []{ "Val1", "Val2", "Val3" }),
                new KeyValuePair<string, StringValues>("array1[1].key", new []{ "101" }),
                new KeyValuePair<string, StringValues>("array1[1].values[]", new []{ "Val1", "Val2", "Val3" }),
            };

            var token = JTokenBinder.Extract(keyValues, schema);

            Assert.Equal("Name", (string)token["name"]);
            Assert.Equal(2, token["array1"].Cast<IEnumerable<object>>().Count());
            Assert.Equal(100, (long)token.SelectToken("$.array1[0].key"));
            Assert.Equal("Val1", (string)token.SelectToken("$.array1[0].values[0]"));
            Assert.Equal("Val2", (string)token.SelectToken("$.array1[0].values[1]"));
            Assert.Equal("Val3", (string)token.SelectToken("$.array1[0].values[2]"));
            Assert.Equal(101, (long)token.SelectToken("$.array1[1].key"));
            Assert.Equal("Val1", (string)token.SelectToken("$.array1[1].values[0]"));
            Assert.Equal("Val2", (string)token.SelectToken("$.array1[1].values[1]"));
            Assert.Equal("Val3", (string)token.SelectToken("$.array1[1].values[2]"));
        }

        [Fact]
        public void Extract3()
        {
            var schema = JSchema.Parse(schemaJson);

            var keyValues = new[]
            {
                new KeyValuePair<string, StringValues>("name", new []{ "Name" }),
                new KeyValuePair<string, StringValues>("array1[0].key", new []{ "100" }),
                new KeyValuePair<string, StringValues>("array1[0].values[0]", new []{ "Val1" }),
                new KeyValuePair<string, StringValues>("array1[0].values[1]", new []{ "Val2" }),
                new KeyValuePair<string, StringValues>("array1[0].values[2]", new []{ "Val3" }),
                new KeyValuePair<string, StringValues>("array1[1].key", new []{ "101" }),
                new KeyValuePair<string, StringValues>("array1[1].values[0]", new []{ "Val1" }),
                new KeyValuePair<string, StringValues>("array1[1].values[1]", new []{ "Val2" }),
                new KeyValuePair<string, StringValues>("array1[1].values[2]", new []{ "Val3" }),
                new KeyValuePair<string, StringValues>("foo", new []{ "foo" }),
                new KeyValuePair<string, StringValues>("bar", new []{ "bar" }),
                new KeyValuePair<string, StringValues>("__", new []{ "__" }),
            };

            var token = JTokenBinder.Extract(keyValues, schema);

            Assert.Equal("Name", (string)token["name"]);
            Assert.Equal(2, token["array1"].Cast<IEnumerable<object>>().Count());
            Assert.Equal(100, (long)token.SelectToken("$.array1[0].key"));
            Assert.Equal("Val1", (string)token.SelectToken("$.array1[0].values[0]"));
            Assert.Equal("Val2", (string)token.SelectToken("$.array1[0].values[1]"));
            Assert.Equal("Val3", (string)token.SelectToken("$.array1[0].values[2]"));
            Assert.Equal(101, (long)token.SelectToken("$.array1[1].key"));
            Assert.Equal("Val1", (string)token.SelectToken("$.array1[1].values[0]"));
            Assert.Equal("Val2", (string)token.SelectToken("$.array1[1].values[1]"));
            Assert.Equal("Val3", (string)token.SelectToken("$.array1[1].values[2]"));
            Assert.Equal("foo", (string)token.SelectToken("$.foo"));
            Assert.Equal("bar", (string)token.SelectToken("$.bar"));
            Assert.Null(token.SelectToken("$.__"));
        }

        #region helper
        string schemaJson = @"{
  '$schema': 'http://json-schema.org/draft-07/schema#',
  'type': 'object',
  'properties': {
    'name': {
      'type': 'string'
    },
    'array1': {
      'type': 'array',
      'items': {
        'type': 'object',
        'properties': {
          'key': {
            'type': 'integer'
          },
          'values': {
            'type': 'array',
            'items': {
              'type': 'string'
            }
          }
        }
      }
    }
  },
  'patternProperties': {
    '[a-zA-Z0-9]+': { type: 'string' }
  }
}";
        #endregion 
    }
}
