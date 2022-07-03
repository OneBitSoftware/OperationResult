using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneBitSoftware.Utilities.Errors;

namespace OneBitSoftware.Utilities
{
    public class OperationResultJsonConverter : JsonConverter<OperationResult>
    {
        public OperationResultJsonConverter()
        {
        }

        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert is null) return false;
            return typeof(OperationResult) == typeToConvert;
        }

        public override OperationResult? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                var serializationOptions = this.ConstructSafeFallbackOptions(options);
                return JsonSerializer.Deserialize(ref reader, typeToConvert, serializationOptions) as OperationResult;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Invalid JSON conversion in OperationResultJsonConverter.", ex);
            }
        }

        public override void Write(Utf8JsonWriter writer, OperationResult value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }
            var fallbackSerializationOptions = this.ConstructSafeFallbackOptions(options);
            JsonSerializer.Serialize(writer, value, value.GetType(), fallbackSerializationOptions);
        }

        private JsonSerializerOptions ConstructSafeFallbackOptions(JsonSerializerOptions options)
        {
            var fallbackSerializationOptions = new JsonSerializerOptions(options);
            fallbackSerializationOptions.Converters.Remove(this);
            return fallbackSerializationOptions;
        }

        //private class ReadOnlyPartialConverter : JsonConverter<OperationResult>
        //{
        //    private readonly OperationResultJsonConverter _operationResultConverter;

        //    internal ReadOnlyPartialConverter(OperationResultJsonConverter operationResultConverter)
        //    {
        //        this._operationResultConverter = operationResultConverter ?? throw new ArgumentNullException(nameof(operationResultConverter));
        //    }

        //    public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(OperationResult);

        //    public override OperationResult Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        //    {
        //        // We copy the value here so we can easily reuse the reader for the subsequent deserialization.
        //        var readerRestore = reader;

        //        // Get the `type` value by parsing the JSON string into a JsonDocument.
        //        var jsonDocument = JsonDocument.ParseValue(ref reader);
        //        jsonDocument.RootElement.TryGetProperty(this._operationResultConverter.TypePropertyName, out var typeElement);

        //        var returnType = this._operationResultConverter.GetType(typeElement);
        //        if (returnType is null) throw new InvalidOperationException("The received JSON cannot be deserialized to any known type.");

        //        try
        //        {
        //            // Deserialize the JSON to the specified type.
        //            return (OperationResult)JsonSerializer.Deserialize(ref readerRestore, returnType, options);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new InvalidOperationException("Invalid JSON in request.", ex);
        //        }
        //    }

        //    public override void Write(Utf8JsonWriter writer, OperationResult value, JsonSerializerOptions options) => throw new InvalidOperationException("The `Read only partial converter` cannot be used for serialization.");
        //}
    }
}
