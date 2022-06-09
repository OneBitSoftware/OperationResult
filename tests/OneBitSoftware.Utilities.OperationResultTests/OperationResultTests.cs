using OneBitSoftware.Utilities;
using Xunit;

namespace OneBitSoftware.Utilities.OperationResultTests
{
    public class OperationResultTests
    {
        [Fact]
        public void NewOperationResult_ShouldSucced()
        {
            var operationResult = new OperationResult();

            Assert.NotNull(operationResult);
        }
    }
}