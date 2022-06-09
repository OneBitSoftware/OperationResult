using Xunit;

namespace OneBitSoftware.Utilities.OperationResultTests
{
    public class OperationResultTests
    {
        [Fact]
        public void NewOperationResult_ShouldNotBeNull()
        {
            var operationResult = new OperationResult();

            Assert.NotNull(operationResult);
        }

        [Fact]
        public void NewOperationResultWithNullLogger_ShouldSucced()
        {
            var operationResult = new OperationResult(null);

            Assert.NotNull(operationResult);
        }
    }
}