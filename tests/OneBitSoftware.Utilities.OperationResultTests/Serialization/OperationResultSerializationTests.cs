using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using OneBitSoftware.Utilities.Errors;
using Xunit;

namespace OneBitSoftware.Utilities.OperationResultTests.Serialization
{
    public class OperationResultSerializationTests
    {
        private JsonSerializerOptions GetSerializationOptions()
        {
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new CustomErrorPolymorphicDeserializer());
            serializeOptions.Converters.Add(new CustomErrorPolymorphicSerializer());
            // serializeOptions.Converters.Add(new PolymorphicOperationErrorListConverter<IList<IOperationError>>());
            return serializeOptions;
        }

        [Fact]
        public async Task CanSerializeWithSystemTextJson()
        {
            // Arrange
            var testText = "Test details";
            var outputStream = new MemoryStream();
            var operationResult = new OperationResult();
            operationResult.AppendError(new OperationError(message: "Test") { Code = 123, Details = testText });

            // Act
            await JsonSerializer.SerializeAsync(outputStream, operationResult, GetSerializationOptions());
            outputStream.Position = 0;
            string text = new StreamReader(outputStream).ReadToEnd();

            // Assert
            Assert.Contains(testText, text);
        }

        [Fact]
        public async Task SerializeWithSystemTextJsonIncludesTypeDiscriminator()
        {
            // Arrange
            var testText = "\"type\"";
            var operationErrorText = "operation_error";
            var outputStream = new MemoryStream();
            var operationResult = new OperationResult();
            operationResult.AppendError(new OperationError(message: "Test") { Code = 123, Details = testText });

            // Act
            await JsonSerializer.SerializeAsync(outputStream, operationResult, GetSerializationOptions());
            outputStream.Position = 0;
            string text = new StreamReader(outputStream).ReadToEnd();

            // Assert
            Assert.Contains(testText, text);
            Assert.Contains(operationErrorText, text);
            var expectText = @"{""Success"":false,""Errors"":[{""type"":""operation_error"",""Code"":123,""Message"":""Test"",""Details"":""\u0022type\u0022""}]}";
            Assert.Equal(expectText, text);
        }

        [Fact]
        public async Task SerializeWithSystemTextJsonSetsExpectedJson()
        {
            // Arrange
            var testText = "\"type\"";
            var outputStream = new MemoryStream();
            var operationResult = new OperationResult();
            operationResult.AppendError(new OperationError(message: "Test") { Code = 123, Details = testText });

            // Act
            await JsonSerializer.SerializeAsync(outputStream, operationResult, GetSerializationOptions());
            outputStream.Position = 0;
            string text = new StreamReader(outputStream).ReadToEnd();

            // Assert
            var expectText = @"{""Success"":false,""Errors"":[{""type"":""operation_error"",""Code"":123,""Message"":""Test"",""Details"":""\u0022type\u0022""}]}";
            Assert.Equal(expectText, text);
        }

        [Fact]
        public async Task NoSuccessMessagesDoesNotSerializeWithSystemTextJson()
        {
            // Arrange
            var initialExceptionText = "SuccessMessages";
            var outputStream = new MemoryStream();
            var operationResult = new OperationResult();
            operationResult.AppendError(new OperationError(message: "Test") { Code = 123, Details = "Test" });

            // Act
            await JsonSerializer.SerializeAsync(outputStream, operationResult, GetSerializationOptions());
            outputStream.Position = 0;
            string text = new StreamReader(outputStream).ReadToEnd();

            // Assert
            Assert.DoesNotContain(initialExceptionText, text);
        }

        [Fact]
        public async Task NoInitialExceptionDoesNotSerializeWithSystemTextJson()
        {
            // Arrange
            var initialExceptionText = "InitialException";
            var outputStream = new MemoryStream();
            var operationResult = new OperationResult();
            operationResult.AppendError(new OperationError(message: "Test") { Code = 123, Details = "Test" });

            // Act
            await JsonSerializer.SerializeAsync(outputStream, operationResult, GetSerializationOptions());
            outputStream.Position = 0;
            string text = new StreamReader(outputStream).ReadToEnd();

            // Assert
            Assert.DoesNotContain(initialExceptionText, text);
        }

        [Fact]
        public void CanSerializeAndDeserializeWithSystemTextJson()
        {
            // Arrange
            var testText = "Test details";
            var serializeStream = new MemoryStream();
            var deserializeStream = new MemoryStream();
            var operationResult = new OperationResult();
            operationResult.AppendError(new OperationError(message: "Test") { Code = 123, Details = testText });

            var c2 = typeof(IOperationError).IsAssignableFrom(typeof(OperationError));

            // Act
            var serializeString = JsonSerializer.Serialize(operationResult, GetSerializationOptions());

            var resultObject = JsonSerializer.Deserialize<OperationResult>(serializeString, GetSerializationOptions());

            // Assert
            Assert.NotNull(resultObject);
            Assert.Equal(operationResult.Errors.Count(), resultObject?.Errors.Count());
            Assert.Equal(operationResult.Fail, resultObject?.Fail);
            Assert.Equal(operationResult.InitialException, resultObject?.InitialException);
            Assert.Equal(operationResult.Success, resultObject?.Success);
            Assert.Equal(operationResult.SuccessMessages, resultObject?.SuccessMessages);
            Assert.Contains(operationResult.Errors, r => r.GetType() == typeof(OperationError));
        }

        [Fact]
        public async Task CanSerializeAndDeserializeCustomErrorWithSystemTextJson()
        {
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
            await JsonSerializer.SerializeAsync(serializeStream, operationResult, GetSerializationOptions());
            serializeStream.Position = 0;
            string serializeString = new StreamReader(serializeStream).ReadToEnd();
            serializeStream.Position = 0;
            var resultObject = await JsonSerializer.DeserializeAsync<OperationResult>(serializeStream, GetSerializationOptions());

            //// Assert
            Assert.NotNull(resultObject);
            Assert.Equal(operationResult.Fail, resultObject?.Fail);
            Assert.Equal(operationResult.InitialException, resultObject?.InitialException);
            Assert.Equal(operationResult.Success, resultObject?.Success);
            Assert.Equal(operationResult.SuccessMessages, resultObject?.SuccessMessages);
            Assert.Contains(operationResult.Errors, r => r.GetType() == typeof(CustomError));
            Assert.Contains(operationResult.Errors, r => r as CustomError != null && (r as CustomError)!.CustomProperty.Equals(customErrorText));
            Assert.Contains(operationResult.Errors, r => r as CustomError != null && (r as CustomError)!.Code.Equals(666));
            Assert.Contains(testText, serializeString);
            Assert.Contains(customErrorText, serializeString);
            Assert.Contains("666", serializeString);
        }

        [Fact]
        public void CanSerializeWithNewtonSoftJson()
        {
            // Arrange
            var testText = "Test details";
            var outputStream = new MemoryStream();
            var operationResult = new OperationResult();
            operationResult.AppendError(new OperationError(message: "Test") { Code = 123, Details = testText });

            // Act
            var resultString = Newtonsoft.Json.JsonConvert.SerializeObject(operationResult);

            // Assert
            Assert.Contains(testText, resultString);
        }
    }
}
