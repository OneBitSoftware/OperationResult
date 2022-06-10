namespace OneBitSoftware.Utilities.Errors;

using System.Text;

public class OperationError
{
    public OperationError(string message, int? code = null)
    {
        this.Message = message;
        this.Code = code;
    }

    public int? Code { get; set; }

    public string Message { get; set; }

    public string? Details { get; set; }

    public override string ToString()
    {
        var result = new StringBuilder();

        if (this.Code != null) result.AppendLine($"Code: {this.Code}");

        if (!string.IsNullOrWhiteSpace(this.Message)) result.AppendLine($"Message: {this.Message}");

        if (!string.IsNullOrWhiteSpace(this.Details)) result.AppendLine($"Trace: {this.Details}");

        return result.ToString();
    }
}
