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

        [Fact]
        public void NewOperationResultT_ShouldNotBeNull()
        {
            var sut = new OperationResult<object>();

            Assert.NotNull(sut);
        }

        [Fact]
        public void NewOperationResultTWithNullLogger_ShouldSucceed()
        {
            var sut = new OperationResult<object>(null);

            Assert.NotNull(sut);
        }
    }
}