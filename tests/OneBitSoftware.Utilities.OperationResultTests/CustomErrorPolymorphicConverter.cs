namespace OneBitSoftware.Utilities.OperationResultTests
{
    using OneBitSoftware.Utilities.Errors;

    /// <summary>
    /// A <see cref="System.Text.Json.Serialization.JsonConverter"/> inheriting class for polymorphic serialization.deserialization of custom <see cref="IOperationError"/> implementations.
    /// </summary>
    internal class CustomErrorPolymorphicConverter : PolymorphicOperationErrorConverter<IOperationError>
    {
        public CustomErrorPolymorphicConverter()
        {
            // Define your discriminator and custom type mapping
            this.AddMapping("custom_error", typeof(CustomError));
        }
    }
}
