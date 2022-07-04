namespace OneBitSoftware.Utilities.OperationResultTests.Serialization
{
    using System;
    using System.Text.Json;
    using OneBitSoftware.Utilities.Errors;

    /// <summary>
    /// A <see cref="System.Text.Json.Serialization.JsonConverter"/> inheriting class for polymorphic serialization.deserialization of custom <see cref="IOperationError"/> implementations.
    /// </summary>
    internal class CustomErrorPolymorphicDeserializer : OperationResultJsonConverter
    {
        public CustomErrorPolymorphicDeserializer() : base()
        {
            // Define your discriminator and custom type mapping
            AddMapping("custom_error", typeof(CustomError));
        }

        //public override void WriteAdditionalElements(Utf8JsonWriter writer, string typeValue, IOperationError error)
        //{
        //    if (error != null && error is CustomError)
        //    {
        //        var customError = error as CustomError;
        //        writer.WriteString(
        //            propertyName: nameof(CustomError.CustomProperty),
        //            value: String.IsNullOrEmpty(customError?.CustomProperty) ? string.Empty : customError.CustomProperty.ToString());
        //    }
        //}

        //public override void AppendErrorType(JsonElement element, Type mappedType, OperationResult operationResult)
        //{
        //    if (mappedType.Equals("custom_error"))
        //    {
        //        operationResult.AppendError(this.ToObject<CustomError>(element));
        //    }
        //}
    }
}
