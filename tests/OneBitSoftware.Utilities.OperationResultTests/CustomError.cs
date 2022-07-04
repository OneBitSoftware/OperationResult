using OneBitSoftware.Utilities.Errors;

namespace OneBitSoftware.Utilities.OperationResultTests
{
    internal class CustomError : OperationError
    {
        public string CustomProperty { get; set; } = null!;
    }
}
