using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using OneBitSoftware.Utilities.Errors;

namespace OneBitSoftware.Utilities
{
    // TODO: comments: used to read an OperationResult, because we don't know what operation errors exist in the JSON
    public class OperationResultJsonConverter : JsonConverter<OperationResult>
    {
        private readonly Dictionary<string, Type> _valueMappings = new Dictionary<string, Type>();

        public OperationResultJsonConverter()
        {
            this.AddMapping("operation_error", typeof(OperationError));
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
                serializationOptions.Converters.Add(new ReadOnlyPartialConverter(this));
                return JsonSerializer.Deserialize(ref reader, typeToConvert, serializationOptions) as OperationResult;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Invalid JSON conversion in OperationResultJsonConverter.", ex);
            }
        }

        protected bool AddMapping(string typeValue, Type type)
        {
            if (string.IsNullOrWhiteSpace(typeValue) || type is null) return false;
            if (this._valueMappings.ContainsKey(typeValue)) return false;

            this._valueMappings[typeValue] = type;
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

        private class ReadOnlyPartialConverter : JsonConverter<OperationResult>
        {
            private readonly OperationResultJsonConverter _operationResultConverter;

            internal ReadOnlyPartialConverter(OperationResultJsonConverter operationResultConverter)
            {
                this._operationResultConverter = operationResultConverter ?? throw new ArgumentNullException(nameof(operationResultConverter));
            }

            public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(OperationResult);

            public override OperationResult? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                // We copy the value here so we can easily reuse the reader for the subsequent deserialization.
                var readerRestore = reader;

                // Get the `type` value by parsing the JSON string into a JsonDocument.
                var jsonDocument = JsonDocument.ParseValue(ref reader);

                try
                {
                    var fallbackDeserializationOptions = this.ConstructSafeFallbackOptions(options);

                    // Deserialize the JSON to the specified type.
                    var deserializeResult = JsonSerializer.Deserialize(ref readerRestore, typeToConvert, fallbackDeserializationOptions);
                    var operationResult = deserializeResult as OperationResult;

                    if (jsonDocument.RootElement.TryGetProperty("Errors", out var errors))
                    {
                        foreach (var item in errors.EnumerateArray())
                        {
                            if (item.TryGetProperty("type", out var typeProperty))
                            {
                                var returnType = this._operationResultConverter.GetType(typeProperty);

                                AppendErrorType(item, returnType, operationResult, options);
                            }
                            else
                            {
                                operationResult.AppendError(this.ToObject<OperationError>(item));
                            }
                        }
                    }


                    return operationResult;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Invalid JSON in request.", ex);
                }

            }

            public T ToObject<T>(JsonElement element)
            {
                var json = element.GetRawText();
                return JsonSerializer.Deserialize<T>(json);
            }

            public virtual void AppendErrorType(JsonElement element, Type mappedType, OperationResult operationResult, JsonSerializerOptions options)
            {
                if (mappedType.Equals("operation_error"))
                {
                    operationResult.AppendError(ToObject<OperationError>(element));
                }
                else
                {
                    var json = element.GetRawText();
                    var deserializeResult = JsonSerializer.Deserialize(json, mappedType, options);
                    operationResult.AppendError(error: deserializeResult as IOperationError);
                }
            }

            private JsonSerializerOptions ConstructSafeFallbackOptions(JsonSerializerOptions options)
            {
                var fallbackSerializationOptions = new JsonSerializerOptions(options);
                fallbackSerializationOptions.Converters.Remove(this);
                return fallbackSerializationOptions;
            }

            public override void Write(Utf8JsonWriter writer, OperationResult value, JsonSerializerOptions options) => throw new InvalidOperationException("The `Read only partial converter` cannot be used for serialization.");
        }
    }
}
