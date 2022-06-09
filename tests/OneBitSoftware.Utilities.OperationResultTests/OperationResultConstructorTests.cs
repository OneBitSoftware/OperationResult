using Xunit;

namespace OneBitSoftware.Utilities.OperationResultTests
{
    public class OperationResultConstructorTests
    {
        [Fact]
        public void NewOperationResult_ShouldNotBeNull()
        {
            var sut = new OperationResult();

            Assert.NotNull(sut);
        }

        [Fact]
        public void NewOperationResultWithNullLogger_ShouldSucceed()
        {
            var sut = new OperationResult(null);

            Assert.NotNull(sut);
        }
    }
}