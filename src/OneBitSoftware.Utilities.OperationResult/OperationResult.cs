namespace OneBitSoftware.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OneBitSoftware.Utilities.Errors;

/// <summary>
/// A class for an operation result.
/// All errors will be logged if initialised with an instance of <see cref="ILogger"/>.
/// </summary>
public class OperationResult
{
    private readonly List<OperationError> _errors = new();
    private readonly List<string> _successMessages = new();

    private readonly ILogger? _logger;

    /// <summary>
    /// Gets or sets a value indicating whether the operation is successful or not.
    /// </summary>
    public bool Success => !this.Fail;

    /// <summary>
    /// Gets a value indicating whether the operation has failed.
    /// </summary>
    public bool Fail => this.Errors.Any();

    /// <summary>
    /// A collection of optional success messages that can be used to process positive operation result messages.
    /// </summary>
    public IEnumerable<string> SuccessMessages
    {
        get
        {
            return _successMessages;
        }
    }

    /// <summary>
    /// Gets an <see cref="List{T}"/> containing the error codes and messages of the <see cref="OperationResult{T}" />.
    /// </summary>
    public IReadOnlyCollection<OperationError> Errors => this._errors.AsReadOnly();

    /// <summary>
    /// Gets or sets the first exception that resulted from the operation.
    /// </summary>
    public Exception? InitialException { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult"/> class.
    /// </summary>
    /// <remarks>If the operation is a get operation, an empty result must return a truthy Success value.</remarks>
    public OperationResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult"/> class.
    /// </summary>
    /// <param name="logger">An instance of <see cref="ILogger"/> to use for automatic logging.</param>
    /// <remarks>If the operation is a get operation, an empty result (no results) must return a truthy Success value.</remarks>
    public OperationResult(ILogger? logger)
    {
        if (logger is not null)
        {
            this._logger = logger; 
        }
    }

    /// <summary>
    /// Adds a success message to the internal collection.
    /// </summary>
    /// <param name="message">The message to add.</param>
    public void AddSuccessMessage(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            this._successMessages.Add(message);
        }
    }

    /// <summary>
    /// Appends all errors from another <see cref="OperationResult"/> to the current <see cref="OperationResult"/>.
    /// </summary>
    /// <param name="otherOperationResult">The <see cref="OperationResult"/> to append errors from.</param>
    /// <returns>The current instance of the <see cref="OperationResult"/>.</returns>
    public OperationResult AppendErrors(OperationResult otherOperationResult)
    {
        if (otherOperationResult is null) return this;

        // Append the error message without logging (presuming that there is already a log message).
        foreach (var error in otherOperationResult.Errors) this.AppendErrorInternal(error);

        return this;
    }

    /// <summary>
    /// This method will append an error with a specific `user-friendly` message to this operation result instance.
    /// </summary>
    /// <param name="message">A label consuming component defining the 'user-friendly' message.</param>
    /// <param name="code">The unique code of the error.</param>
    /// <param name="logLevel">The logging severity.</param>
    /// <param name="details">A <see cref="string"/> with error details.</param>
    /// <returns>The current instance of the <see cref="OperationResult"/>.</returns>
    public OperationResult AppendError(string message, int? code = null, LogLevel? logLevel = null, string? details = null)
    {
        if (string.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));

        var error = new OperationError(message, code) { Details = details };
        this.AppendError(error, logLevel);

        return this;
    }

    /// <summary>
    /// Appends an <see cref="OperationError"/> to the internal errors collection.
    /// </summary>
    /// <param name="error">An instance of <see cref="OperationError"/> to add to the internal errors collection.</param>
    /// <param name="logLevel">The logging level.</param>
    /// <returns>The current instance of the <see cref="OperationResult"/>.</returns>
    public OperationResult AppendError(OperationError error, LogLevel? logLevel = LogLevel.Error)
    {
        this.AppendErrorInternal(error);

        if (this._logger is not null)
        {
#pragma warning disable CA2254 // Template should be a static expression
            this._logger.Log(GetLogLevel(logLevel), error.Message);
#pragma warning restore CA2254 // Template should be a static expression
        }

        return this;
    }

    /// <summary>
    /// Appends an exception to the error message collection and logs the full exception as an Error <see cref="LogEventLevel"/> level. A call to this method will set the Success property to false.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="errorCode">The error code.</param>
    /// <param name="logLevel">The <see cref="LogEventLevel"/> logging severity.</param>
    /// <returns>The current instance of the <see cref="OperationResult"/>.</returns>
    public OperationResult AppendException(Exception exception, int errorCode = 0, LogLevel? logLevel = null)
    {
        if (exception is null) throw new ArgumentNullException(nameof(exception));

        // Append the exception as a first if it is not yet set.
        this.InitialException ??= exception;

        var error = new OperationError(exception.ToString(), errorCode);
        this.AppendError(error, logLevel);

        return this;
    }

    /// <summary>
    /// Use this method to get a string with all error messages.
    /// </summary>
    /// <returns>All error messages, joined with a new line character.</returns>
    public override string ToString() => string.Join(Environment.NewLine, this.Errors);

    /// <summary>
    /// Creates an instance of <see cref="OperationResult"/> and appends the passed exception to it's error collection.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> to append.</param>
    /// <param name="logger">An optional instance of <see cref="ILogger"/>.</param>
    /// <returns>An <see cref="OperationResult"/> containing the passed exception.</returns>
    public static OperationResult FromException(Exception exception, ILogger? logger = null)
    {
        var result = new OperationResult(logger);
        return result.AppendException(exception);
    }

    /// <summary>
    /// Creates an instance of <see cref="OperationResult"/> and appends the passed error message details to it's internal error collection.
    /// </summary>
    /// <param name="message">A message to append to the internal errors collection.</param>
    /// <param name="code">An optional code to include in the error.</param>
    /// <param name="logLevel">A log event level. Defaults to Error.</param>
    /// <param name="details">An optional detail message to add to the error.</param>
    /// <param name="logger">An optional instance of <see cref="ILogger"/>.</param>
    /// <returns>An <see cref="OperationResult"/> containing the passed exception.</returns>
    public static OperationResult FromError(string message, int? code = null, LogLevel logLevel = LogLevel.Error, string? details = null, ILogger? logger = null)
    {
        var result = new OperationResult(logger);
        return result.AppendError(message, code, logLevel, details);
    }

    // TODO: this method needs completing.
    private static LogLevel GetLogLevel(LogLevel? optionalLevel) => optionalLevel ?? LogLevel.Error;

    /// <summary>
    /// Appends an <see cref="OperationError"/> to the internal errors collection.
    /// </summary>
    /// <param name="error">An instance of <see cref="OperationError"/> to add to the internal errors collection.</param>
    private void AppendErrorInternal(OperationError error) => this._errors.Add(error);
}

/// <summary>
/// A class for a system operation result with a generic result object type.
/// </summary>
/// <typeparam name="TResult">The type of object that will be returned.</typeparam>
public class OperationResult<TResult> : OperationResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult"/> class.
    /// </summary>
    /// <remarks>If the operation is a get operation, an empty result must return a truthy Success value.</remarks>
    public OperationResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult"/> class.
    /// </summary>
    /// <param name="loggerService">An instance of <see cref="ILoggerService"/>.</param>
    /// <remarks>If the operation is a get operation, an empty result must return a truthy Success value.</remarks>
    public OperationResult(ILogger loggerService) : base(loggerService)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult"/> class and sets the passed result object. Internally, this will set the Success result to True.
    /// </summary>
    /// <param name="resultObject">An initial failure message for the operation result. This will fail the success status.</param>
    /// <param name="loggerService">An instance of <see cref="ILoggerService"/>.</param>
    /// <remarks>If the operation is a get operation, an empty result must return a truthy Success value.</remarks>
    public OperationResult(TResult resultObject, ILogger loggerService) : base(loggerService)
    {
        this.ResultObject = resultObject;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OperationResult"/> class and sets the passed result object. Internally, this will set the Success result to True.
    /// </summary>
    /// <param name="resultObject">An initial failure message for the operation result. This will fail the success status.</param>
    /// <remarks>If the operation is a get operation, an empty result must return a truthy Success value.</remarks>
    public OperationResult(TResult resultObject) : base()
    {
        this.ResultObject = resultObject;
    }

    /// <summary>
    /// Gets or sets the related result object of the operation.
    /// </summary>
    public TResult? ResultObject { get; set; }

    /// <summary>
    /// This method will append an error with a specific `user-friendly` message to this operation result instance.
    /// </summary>
    /// <param name="message">A label consuming component defining the 'user-friendly' message.</param>
    /// <param name="code">The unique code of the error.</param>
    /// <param name="logLevel">The logging severity.</param>
    /// <param name="details">A <see cref="string"/> with error details.</param>
    /// <returns>The current instance of the <see cref="OperationResult{TResult}"/>.</returns>
    public new OperationResult<TResult> AppendError(string message, int? code = null, LogLevel? logLevel = null, string? details = null)
    {
        base.AppendError(message, code, logLevel, details);

        return this;
    }

    /// <summary>
    /// Appends error messages from <paramref name="otherOperationResult"/> to <paramref name="originalOperationResult"/>.
    /// </summary>
    /// <param name="originalOperationResult">The <see cref="OperationResult"/> to append to.</param>
    /// <param name="otherOperationResult">The <see cref="OperationResult"/> to append from.</param>
    /// <typeparam name="TOriginal">A type that inherits from <see cref="OperationResult"/>.</typeparam>
    /// <typeparam name="TOther">A type that inherits from <see cref="OperationResult"/>.</typeparam>
    /// <returns>The original <see cref="OperationResult"/> with the appended messages from <paramref name="otherOperationResult"/>.</returns>
    public OperationResult<TResult> AppendErrorMessages<TOther>(TOther otherOperationResult)
        where TOther : OperationResult
    {
        base.AppendErrors(otherOperationResult);

        return this;
    }

    /// <summary>
    /// Appends an exception to the error message collection and logs the full exception as an Error <see cref="LogEventLevel"/> level. A call to this method will set the Success property to false.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="errorCode">The error code.</param>
    /// <param name="logLevel">The <see cref="LogEventLevel"/> logging severity.</param>
    /// <returns>The current instance of the <see cref="OperationResult{TResult}"/>.</returns>
    public new OperationResult<TResult> AppendException(Exception exception, int errorCode = 0, LogLevel? logLevel = null)
    {
        base.AppendException(exception, errorCode, logLevel);

        return this;
    }
}
