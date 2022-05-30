namespace OneBitSoftware.Utilities.OperationResult
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A class for a system operation result.
    /// </summary>
    public class OperationResult
    {
        private readonly List<Error> _errors = new();
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
        /// Gets an <see cref="List{T}"/> containing the error codes and messages of the <see cref="OperationResult{T}" />.
        /// </summary>
        public IReadOnlyCollection<Error> Errors => this._errors.AsReadOnly();

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
        /// <param name="loggerService">An instance of <see cref="ILogger"/> to use for automatic logging.</param>
        /// <remarks>If the operation is a get operation, an empty result (no results) must return a truthy Success value.</remarks>
        public OperationResult(ILogger loggerService)
        {
            this._logger = loggerService;
        }

        public void AppendErrors(OperationResult otherOperationResult)
        {
            if (otherOperationResult is null) return;

            // Append the error message without logging (presuming that there is already a log message).
            foreach (var error in otherOperationResult.Errors) this.AppendError(error);
        }

        /// <summary>
        /// This method will append an error with a specific `user-friendly` message to this operation result instance.
        /// </summary>
        /// <param name="message">A label consuming component defining the 'user-friendly' message.</param>
        /// <param name="errorCode">The unique code of the error.</param>
        /// <param name="logLevel">The logging severity.</param>
        /// <returns>The current instance of the <see cref="OperationResult"/>.</returns>
        public OperationResult AppendError(string message, int errorCode = 0, LogLevel? logLevel = null)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));

            var error = new Error { Message = message,Code = errorCode };
            this.AppendError(error, logLevel);

            return this;
        }

        /// <summary>
        /// This method will append an error with a default `user-friendly` message to this operation result instance.
        /// </summary>
        /// <param name="debugMessage">A debug message that should be logged and provide additional information in debug mode.</param>
        /// <param name="errorCode">The unique code of the error.</param>
        /// <param name="logLevel">The logging severity.</param>
        /// <returns>The current instance of the <see cref="OperationResult"/>.</returns>
        public OperationResult AppendErrorMessage(string message, int errorCode = 0, LogLevel? logLevel = null) => this.AppendError(message, errorCode, logLevel);

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

            var error = new Error { Message = exception.ToString(), Code = errorCode };
            this.AppendError(error, logLevel);

            return this;
        }

        /// <summary>
        /// Use this method to check if a value is a valid string.
        /// If <paramref name="value"/> is null, empty or consists only of whitespace characters, an error message should be appended and a log of the passed <paramref name="level"/> severity would be created.
        /// </summary>
        /// <param name="value">The value that should be validated.</param>
        /// <param name="className">The name of the class where the <paramref name="methodName"/> is defined.</param>
        /// <param name="methodName">The name of he method where <paramref name="value"/> is used.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="level">The logging severity.</param>
        public void ValidateNullOrWhitespace(string value, string className, string methodName, string propertyName, LogLevel level = LogLevel.Error)
        {
            // If the passed value is null, empty or consists only of whitespace characters, log and append an error message.
            if (string.IsNullOrWhiteSpace(value) == false) return;

            var errorMessage = $"{className}, {methodName} - The {propertyName} is null, empty or consists only of whitespace characters.";
            this.AppendErrorMessage(errorMessage, logLevel: level);
        }

        /// <summary>
        /// Use this method to check if a value is not null.
        /// If you want to validate that an entity exists, use the "ValidateExist" extension method.
        /// If you want to validate that the currently authenticated user is not null, use the "ValidateUser" extension method.
        /// If <paramref name="value"/> is null, an error message should be appended and a log of the passed <paramref name="level"/> severity would be created.
        /// </summary>
        /// <param name="value">The value that should be validated.</param>
        /// <param name="className">The name of the class where the <paramref name="methodName"/> is defined.</param>
        /// <param name="methodName">The name of he method where <paramref name="value"/> is used.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="level">The logging severity.</param>
        public void ValidateNull(object value, string className, string methodName, string propertyName, LogLevel level = LogLevel.Error)
        {
            // If the passed value is null, log and append an error message.
            if (value != null) return;

            var errorMessage = $"{className}, {methodName} - The {propertyName} is null.";
            this.AppendErrorMessage(errorMessage, logLevel: level);
        }

        /// <summary>
        /// Use this method to check if a value is equal to its default value.
        /// If <paramref name="value"/> is equal to its default value, an error message should be appended and a log of the passed <paramref name="level"/> severity would be created.
        /// </summary>
        /// <typeparam name="TValue">The type of the <paramref name="value"/>.</typeparam>
        /// <param name="value">The value that should be validated.</param>
        /// <param name="className">The name of the class where the <paramref name="methodName"/> is defined.</param>
        /// <param name="methodName">The name of he method where <paramref name="value"/> is used.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="level">The logging severity.</param>
        public void ValidateDefault<TValue>(TValue value, string className, string methodName, string propertyName, LogLevel level = LogLevel.Error)
            where TValue : struct, IEquatable<TValue>
        {
            // If the passed value is null, log and append an error message.
            if (value.Equals(default) == false) return;

            var errorMessage = $"{className}, {methodName} - The {propertyName} has a default value.";
            this.AppendErrorMessage(errorMessage, logLevel: level);
        }

        /// <summary>
        /// Use this method to check if a value is not null and not an empty collection.
        /// If <paramref name="value"/> is null or an empty collection, an error message should be appended and a log of the passed <paramref name="level"/> severity would be created.
        /// </summary>
        /// <typeparam name="T">The type of the underlying entities that are stored within the requested collection.</typeparam>
        /// <param name="value">The collection that should be validated.</param>
        /// <param name="className">The name of the class where the <paramref name="methodName"/> is defined.</param>
        /// <param name="methodName">The name of he method where <paramref name="value"/> is used.</param>
        /// <param name="identifierPropertyName">The name of the entity's unique identifier property.</param>
        /// <param name="level">The logging severity.</param>
        public void ValidateAny<T>(IEnumerable<T> value, string className, string methodName, string identifierPropertyName, LogLevel level = LogLevel.Error)
        {
            // If the passed value is null, log and append an error message.
            if (value == null)
            {
                var errorMessage = $"{className}, {methodName} - An object with that {identifierPropertyName} is null.";
                this.AppendErrorMessage(errorMessage, logLevel: level);
            }

            // If the passed value is an empty collection, log and append an error message.
            else if (!value.Any())
            {
                var errorMessage = $"{className}, {methodName} - The collection with that {identifierPropertyName} is empty.";
                this.AppendErrorMessage(errorMessage, logLevel: level);
            }
        }

        /// <summary>
        /// Use this method to get a string with all error messages.
        /// </summary>
        /// <returns>All error messages, joined with a new line character.</returns>
        public override string ToString() => string.Join(Environment.NewLine, this.Errors);

        private static LogLevel GetLogLevel(LogLevel? optionalLevel) => optionalLevel ?? LogLevel.Error;

        private void AppendError(Error error, LogLevel? logLevel)
        {
            this.AppendError(error);

            if (this._logger is not null)
            {
                this._logger.Log(GetLogLevel(logLevel), error.Message);
            }
        }

        private void AppendError(Error error) => this._errors.Add(error);
    }

    /// <summary>
    /// A class for a system operation result with a generic result object type.
    /// </summary>
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
        public OperationResult(ILogger loggerService, TResult resultObject)
            : base(loggerService)
        {
            this.RelatedObject = resultObject;
        }

        /// <summary>
        /// Gets or sets the related object of the operation.
        /// </summary>
        public TResult? RelatedObject { get; set; }
    }
}