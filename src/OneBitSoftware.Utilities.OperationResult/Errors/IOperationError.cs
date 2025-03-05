namespace OneBitSoftware.Utilities.Errors
{
    using Microsoft.Extensions.Logging;

    public interface IOperationError
    {
        int? Code { get; set; }

        string? Message { get; set; }

        string? Details { get; set; }
    }
}
