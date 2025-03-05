namespace OneBitSoftware.Utilities.Errors
{
    using System.Text;
    using Microsoft.Extensions.Logging;

    public class OperationError : IOperationError
    {
        public OperationError(string? message = null, int? code = null, string? details = null, LogLevel? logLevel = null)
        {
            this.Message = message;
            this.Code = code;
            this.Details = details;
            this.LogLevel = logLevel;
        }

        public int? Code { get; set; }

        public string? Message { get; set; }

        public string? Details { get; set; }

        /// <inheritdoc />
        public LogLevel? LogLevel { get; set; }

        /// <inheritdoc />
        bool IOperationError.Logged { get; set; }

        public override string ToString()
        {
            var result = new StringBuilder();

            if (this.Code != null) result.AppendLine($"Code: {this.Code}");

            if (this.LogLevel is not null) result.AppendLine($"Severity: {this.LogLevel}"); // TODO: maybe convert to string?

            if (!string.IsNullOrWhiteSpace(this.Message)) result.AppendLine($"Message: {this.Message}");

            if (!string.IsNullOrWhiteSpace(this.Details)) result.AppendLine($"Trace: {this.Details}");

            return result.ToString();
        }
    }

}
