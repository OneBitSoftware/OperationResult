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
        public void TestCustomError()
        {
            var c = new CustomError();
            var c1 = typeof(CustomError).IsAssignableFrom(typeof(IOperationError));
            var c2 = typeof(IOperationError).IsAssignableFrom(typeof(CustomError));
        }
    }
}
