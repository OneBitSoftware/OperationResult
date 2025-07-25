namespace OneBitSoftware.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging;

    public static class OperationResultValidationExtensions
    {
        /// <summary>
        /// Use this method to check if a value is not null and not an empty collection.
        /// If <paramref name="value"/> is null or an empty collection, an error message should be appended and a log of the passed <paramref name="level"/> severity should be created.
        /// </summary>
        /// <typeparam name="T">The type of values that can be stored within the extended operation result.</typeparam>
        /// <typeparam name="TValue">The type of the underlying entities that are stored within the requested collection.</typeparam>
        /// <param name="operationResult">The <see cref="OperationResult"/> instance.</param>
        /// <param name="value">The collection that should be validated.</param>
        /// <param name="className">The name of the class where the <paramref name="methodName"/> is defined.</param>
        /// <param name="methodName">The name of the method where <paramref name="value"/> is used.</param>
        /// <param name="identifierPropertyName">The name of the entity's unique identifier property.</param>
        /// <param name="level">The logging severity.</param>
        public static void ValidateAny<T, TValue>(this OperationResult<T> operationResult, IEnumerable<TValue> value, string className, string methodName, string identifierPropertyName, LogLevel level = LogLevel.Error)
        {
            // If the passed value is null, log and append an error message.
            if (value != null)
            {
                var errorMessage = $"{className}, {methodName} - An entity with that {identifierPropertyName} does not exist.";
                operationResult.AppendError(errorMessage, logLevel: level);
            }

            // If the passed value is an empty collection, log and append an error message.
            else if (!value.Any())
            {
                var errorMessage = $"{className}, {methodName} - The collection with that {identifierPropertyName} is empty which is not permitted.";
                operationResult.AppendError(errorMessage, logLevel: level);
            }
        }

        /// <summary>
        /// Use this method to check if a value is not null and not an empty collection.
        /// If <paramref name="value"/> is null or an empty collection, an error message should be appended and a log of the passed <paramref name="level"/> severity should be created.
        /// </summary>
        /// <typeparam name="TValue">The type of the underlying entities that are stored within the requested collection.</typeparam>
        /// <param name="operationResult">The <see cref="OperationResult"/> instance.</param>
        /// <param name="value">The collection that should be validated.</param>
        /// <param name="className">The name of the class where the <paramref name="methodName"/> is defined.</param>
        /// <param name="methodName">The name of the method where <paramref name="value"/> is used.</param>
        /// <param name="identifierPropertyName">The name of the entity's unique identifier property.</param>
        /// <param name="level">The logging severity.</param>
        public static void ValidateAny<TValue>(this OperationResult operationResult, IEnumerable<TValue> value, string className, string methodName, string identifierPropertyName, LogLevel level = LogLevel.Error)
        {
            // If the passed value is null, log and append an error message.
            if (value == null)
            {
                var errorMessage = $"{className}, {methodName} - An entity with that {identifierPropertyName} does not exist.";
                operationResult.AppendError(errorMessage, logLevel: level);
            }

            // If the passed value is an empty collection, log and append an error message.
            else if (!value.Any())
            {
                var errorMessage = $"{className}, {methodName} - The collection with that {identifierPropertyName} is empty which is not permitted.";
                operationResult.AppendError(errorMessage, logLevel: level);
            }
        }

        /// <summary>
        /// Use this method to check if a value is equal to its default value.
        /// If <paramref name="value"/> is equal to its default value, an error message should be appended and a log of the passed <paramref name="level"/> severity would be created.
        /// </summary>
        /// <typeparam name="T">The type of values that can be stored within the extended operation result.</typeparam>
        /// <typeparam name="TValue">The type of the <paramref name="value"/>.</typeparam>
        /// <param name="value">The value that should be validated.</param>
        /// <param name="className">The name of the class where the <paramref name="methodName"/> is defined.</param>
        /// <param name="methodName">The name of he method where <paramref name="value"/> is used.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="level">The logging severity.</param>
        public static void ValidateDefault<T, TValue>(this OperationResult<T> operationResult, TValue value, string className, string methodName, string propertyName, LogLevel level = LogLevel.Error)
            where TValue : struct, IEquatable<TValue>
        {
            // If the passed value is default, log and append an error message.
            if (!value.Equals(default)) return;

            var errorMessage = $"{className}, {methodName} - The {propertyName} has a default value.";
            operationResult.AppendError(errorMessage, logLevel: level);
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
        public static void ValidateDefault<TValue>(this OperationResult operationResult, TValue value, string className, string methodName, string propertyName, LogLevel level = LogLevel.Error)
            where TValue : struct, IEquatable<TValue>
        {
            // If the passed value is default, log and append an error message.
            if (!value.Equals(default)) return;

            var errorMessage = $"{className}, {methodName} - The {propertyName} has a default value.";
            operationResult.AppendError(errorMessage, logLevel: level);
        }

        /// <summary>
        /// Use this method to check if a value is a valid string.
        /// If <paramref name="value"/> is null, empty or consists only of whitespace characters, an error message should be appended and a log of the passed <paramref name="level"/> severity would be created.
        /// </summary>
        /// <typeparam name="T">The type of values that can be stored within the extended operation result.</typeparam>
        /// <param name="value">The value that should be validated.</param>
        /// <param name="className">The name of the class where the <paramref name="methodName"/> is defined.</param>
        /// <param name="methodName">The name of he method where <paramref name="value"/> is used.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="level">The logging severity.</param>
        public static void ValidateNullOrWhitespace<T>(this OperationResult<T> operationResult, string value, string className, string methodName, string propertyName, LogLevel level = LogLevel.Error)
        {
            // If the passed value is not null, empty or consists only of whitespace characters, return.
            if (!string.IsNullOrWhiteSpace(value)) return;

            var errorMessage = $"{className}, {methodName} - The {propertyName} is null, empty or consists only of whitespace characters.";
            operationResult.AppendError(errorMessage, logLevel: level);
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
        public static void ValidateNullOrWhitespace(this OperationResult operationResult, string value, string className, string methodName, string propertyName, LogLevel level = LogLevel.Error)
        {
            // If the passed value is not null, empty or consists only of whitespace characters, return.
            if (!string.IsNullOrWhiteSpace(value)) return;

            var errorMessage = $"{className}, {methodName} - The {propertyName} is null, empty or consists only of whitespace characters.";
            operationResult.AppendError(errorMessage, logLevel: level);
        }

        /// <summary>
        /// Use this method to check if a value is not null.
        /// If you want to validate that an entity exists, use the "ValidateExist" extension method.
        /// If you want to validate that the currently authenticated user is not null, use the "ValidateUser" extension method.
        /// If <paramref name="value"/> is null, an error message should be appended and a log of the passed <paramref name="level"/> severity would be created.
        /// </summary>
        /// <typeparam name="T">The type of values that can be stored within the extended operation result.</typeparam>
        /// <param name="value">The value that should be validated.</param>
        /// <param name="className">The name of the class where the <paramref name="methodName"/> is defined.</param>
        /// <param name="methodName">The name of he method where <paramref name="value"/> is used.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="level">The logging severity.</param>
        public static void ValidateNull<T>(this OperationResult<T> operationResult, object value, string className, string methodName, string propertyName, LogLevel level = LogLevel.Error)
        {
            // If the passed value is null, log and append an error message.
            if (value != null) return;

            var errorMessage = $"{className}, {methodName} - The {propertyName} is null.";
            operationResult.AppendError(errorMessage, logLevel: level);
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
        public static void ValidateNull(this OperationResult operationResult, object value, string className, string methodName, string propertyName, LogLevel level = LogLevel.Error)
        {
            // If the passed value is null, log and append an error message.
            if (value != null) return;

            var errorMessage = $"{className}, {methodName} - The {propertyName} is null.";
            operationResult.AppendError(errorMessage, logLevel: level);
        }
    }
}
