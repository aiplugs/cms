using Aiplugs.CMS.Core.Services;
using Xunit;

namespace Aiplugs.CMS.Tests
{
    public class ProcedureTest
    {
        [Fact]
        public void GetBuiltinProceduresTest()
        {
            var service = new ProcedureSerivce(null, null, null);
            var types = service.GetBuiltinProcedures();

            Assert.NotNull(types);
            Assert.Single(types);
            Assert.Equal("ValidateProcedure", types[0].Name);
        }
    }
}