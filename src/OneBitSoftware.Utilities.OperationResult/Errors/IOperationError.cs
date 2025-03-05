namespace OneBitSoftware.Utilities.Errors
{
    using Microsoft.Extensions.Logging;

    public interface IOperationError
    {
        int? Code { get; set; }

        string? Message { get; set; }

        string? Details { get; set; }

        /// <summary>
        /// Defines if the error is logged or not. Used when merging <cref="OperationResult"/> instances and one of them does not have an ILogger.
        /// </summary>
        bool Logged { get; internal set; }

        /// <summary>
        /// Defines the log level for the error.
        /// </summary>
        LogLevel? LogLevel { get; set; }
    }
}
