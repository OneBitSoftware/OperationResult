namespace OneBitSoftware.Utilities.Errors
{
    public interface IOperationError
    {
        int? Code { get; set; }

        string Message { get; set; }

        string? Details { get; set; }
    }
}
