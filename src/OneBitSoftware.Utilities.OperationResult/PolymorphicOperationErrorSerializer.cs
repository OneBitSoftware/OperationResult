using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneBitSoftware.Utilities.Errors;

namespace OneBitSoftware.Utilities
{
    // TODO: comments - used to serialize a custom OperationError
    public class PolymorphicOperationErrorSerializer<T> : JsonConverter<T>
        where T : IOperationError
    {
        private readonly Dictionary<string, Type> _valueMappings = new Dictionary<string, Type>();
        private readonly Dictionary<Type, string> _typeMappings = new Dictionary<Type, string>();
        protected virtual string TypePropertyName => "type";

        public PolymorphicOperationErrorSerializer()
        {
            // Define the base OperationError custom type mapping discriminator
            this.AddMapping("operation_error", typeof(OperationError));
        }

        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert is null) return false;
            return typeof(IOperationError).IsAssignableFrom(typeToConvert);
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                // Should not be used, write-only.
                // Deserialize the JSON to the specified type.
                var serializationOptions = this.ConstructSafeFallbackOptions(options);
                serializationOptions.Converters.Add(new ReadOnlyPartialConverter(this));
                return (T)JsonSerializer.Deserialize(ref reader, typeToConvert, serializationOptions);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Invalid JSON in request.", ex);
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            if (this._typeMappings.TryGetValue(value.GetType(), out var typeValue) == false) throw new InvalidOperationException($"Model of type {value.GetType()} cannot be successfully serialized.");

            var tempBufferWriter = new ArrayBufferWriter<byte>();
            var tempWriter = new Utf8JsonWriter(tempBufferWriter);

            var fallbackDeserializationOptions = this.ConstructSafeFallbackOptions(options);
            JsonSerializer.Serialize(tempWriter, value, value.GetType(), fallbackDeserializationOptions);

            tempWriter.Flush();
            var jsonDocument = JsonDocument.Parse(tempBufferWriter.WrittenMemory);

            writer.WriteStartObject();
            writer.WriteString(this.TypePropertyName, typeValue);

            foreach (var property in jsonDocument.RootElement.EnumerateObject()) property.WriteTo(writer);
            writer.WriteEndObject();
        }


        protected bool AddMapping(string typeValue, Type type)
        {
            if (string.IsNullOrWhiteSpace(typeValue) || type is null) return false;
            if (this._valueMappings.ContainsKey(typeValue) || this._typeMappings.ContainsKey(type)) return false;

            this._valueMappings[typeValue] = type;
            this._typeMappings[type] = typeValue;
            return true;
        }

        private Type GetType(JsonElement typeElement)
        {
            if (typeElement.ValueKind != JsonValueKind.String) return null;

            var stringValue = typeElement.GetString();
            if (string.IsNullOrWhiteSpace(stringValue)) return null;

            this._valueMappings.TryGetValue(stringValue, out var type);
            return type;
        }

        private JsonSerializerOptions ConstructSafeFallbackOptions(JsonSerializerOptions options)
        {
            var fallbackSerializationOptions = new JsonSerializerOptions(options);
            fallbackSerializationOptions.Converters.Remove(this);
            return fallbackSerializationOptions;
        }

        private class ReadOnlyPartialConverter : JsonConverter<T>
        {
            private readonly PolymorphicOperationErrorSerializer<T> _polymorphicConverter;

            internal ReadOnlyPartialConverter(PolymorphicOperationErrorSerializer<T> polymorphicConverter)
            {
                this._polymorphicConverter = polymorphicConverter ?? throw new ArgumentNullException(nameof(polymorphicConverter));
            }

            public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(T);

            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                // We copy the value here so we can easily reuse the reader for the subsequent deserialization.
                var readerRestore = reader;

                // Get the `type` value by parsing the JSON string into a JsonDocument.
                var jsonDocument = JsonDocument.ParseValue(ref reader);
                jsonDocument.RootElement.TryGetProperty(this._polymorphicConverter.TypePropertyName, out var typeElement);

                var returnType = this._polymorphicConverter.GetType(typeElement);
                if (returnType is null) throw new InvalidOperationException("The received JSON cannot be deserialized to any known type.");

                try
                {
                    // Deserialize the JSON to the specified type.
                    return (T)JsonSerializer.Deserialize(ref readerRestore, returnType, options);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Invalid JSON in request.", ex);
                }
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) => throw new InvalidOperationException("The `Read only partial converter` cannot be used for serialization.");
        }
    }
}
