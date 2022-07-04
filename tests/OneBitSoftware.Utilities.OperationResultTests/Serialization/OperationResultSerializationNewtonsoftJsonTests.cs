using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OneBitSoftware.Utilities.Errors;
using Xunit;

namespace OneBitSoftware.Utilities.OperationResultTests.Serialization
{
    public class TypeNameSerializationBinder : ISerializationBinder
    {
        public string TypeFormat { get; private set; }

        public TypeNameSerializationBinder(string typeFormat)
        {
            TypeFormat = typeFormat;
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }

        public Type BindToType(string assemblyName, string typeName)
        {
            var resolvedTypeName = string.Format(TypeFormat, typeName);
            return Type.GetType(resolvedTypeName, true);
        }
    }

    public class OperationResultSerializationNewtonsoftJsonTests
    {
        public OperationResultSerializationNewtonsoftJsonTests()
        {
        }

        private JsonSerializerSettings GetJsonSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Auto,
                SerializationBinder = new TypeNameSerializationBinder("OneBitSoftware.Utilities.Errors.OperationError, OneBitSoftware.Utilities.OperationResult")
            };
        }

        [Fact]
        public async Task CanSerializeWithNewtonsoftJson()
        {
            // Arrange
            var testText = "Test details";
            var operationResult = new OperationResult();
            operationResult.AppendError(new OperationError(message: "Test") { Code = 123, Details = testText });

            // Act
            string json = JsonConvert.SerializeObject(operationResult, GetJsonSerializerSettings());

            // Assert
            Assert.Contains(testText, json);
        }

        [Fact]
        public async Task CanSerializeAndDeserializeCustomErrorWithSystemTextJson()
        {
            // TODO: clean up variables and repeated code
            // Arrange
            var testText = "Test details";
            var customErrorText = "Custom error test details";
            var serializeStream = new MemoryStream();
            var deserializeStream = new MemoryStream();
            var operationResult = new OperationResult();
            operationResult.AppendError(new OperationError(message: "Test") { Code = 123, Details = testText });
            operationResult.AppendError(new CustomError() { Message = "Custom Test", Code = 666, Details = testText, CustomProperty = customErrorText });

            var c2 = typeof(IOperationError).IsAssignableFrom(typeof(OperationError));

            // Act
            string json = JsonConvert.SerializeObject(operationResult, GetJsonSerializerSettings());

            var resultObject = JsonConvert.DeserializeObject<OperationResult>(json, GetJsonSerializerSettings());

            //// Assert
            Assert.NotNull(resultObject);
            Assert.Equal(operationResult.Fail, resultObject?.Fail);
            Assert.Equal(operationResult.InitialException, resultObject?.InitialException);
            Assert.Equal(operationResult.Success, resultObject?.Success);
            Assert.Equal(operationResult.SuccessMessages, resultObject?.SuccessMessages);
            Assert.Contains(operationResult.Errors, r => r.GetType() == typeof(CustomError));
            Assert.Contains(operationResult.Errors, r => r as CustomError != null && (r as CustomError)!.CustomProperty.Equals(customErrorText));
            Assert.Contains(operationResult.Errors, r => r as CustomError != null && (r as CustomError)!.Code.Equals(666));
            Assert.Contains(testText, json);
            Assert.Contains(customErrorText, json);
            Assert.Contains("666", json);
        }
    }
}
