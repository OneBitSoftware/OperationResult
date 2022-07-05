using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneBitSoftware.Utilities.Errors;
using Xunit;

namespace OneBitSoftware.Utilities.OperationResultTests
{
    public class CustomErrorTests
    {
        [Fact]
        public void CustomError_CanInstantiate()
        {
            var customError = new CustomError();

            Assert.NotNull(customError);
        }

        [Fact]
        public void CustomError_IsIOperationError()
        {
            Assert.True(typeof(IOperationError).IsAssignableFrom(typeof(CustomError)));
        }

        [Fact]
        public void CustomError_IsOperationError()
        {
            var customError = new CustomError();

            Assert.True(typeof(OperationError).IsInstanceOfType(customError));
        }
    }
}
