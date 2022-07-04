namespace OneBitSoftware.Utilities.OperationResultTests.Serialization
{
    using System;
    using System.Text.Json;
    using OneBitSoftware.Utilities.Errors;

    // TODO: unify with the serializer
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

    }
}
