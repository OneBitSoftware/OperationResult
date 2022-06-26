using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OneBitSoftware.Utilities.Errors;
using Xunit;

namespace OneBitSoftware.Utilities.OperationResultTests
{
    public class OperationResultSerializationTests
    {
        private JsonSerializerOptions GetSerializationOptions()
        {
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new CustomErrorPolymorphicConverter());
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
            await JsonSerializer.SerializeAsync<OperationResult>(outputStream, operationResult, GetSerializationOptions());
            outputStream.Position = 0;
            string text = new StreamReader(outputStream).ReadToEnd();

            // Assert
            Assert.Contains(testText, text);
        }

        [Fact]
        public async Task CanSerializeAndDeserializeWithSystemTextJson()
        {
            // Arrange
            var testText = "Test details";
            var serializeStream = new MemoryStream();
            var deserializeStream = new MemoryStream();
            var operationResult = new OperationResult();
            operationResult.AppendError(new OperationError(message: "Test") { Code = 123, Details = testText });

            // Act
            var serializeString = JsonSerializer.Serialize<OperationResult>(operationResult, GetSerializationOptions());

            var resultObject = JsonSerializer.Deserialize<OperationResult>(serializeString, GetSerializationOptions());

            // Assert
            Assert.NotNull(resultObject);
            Assert.Equal(operationResult.Errors.Count(), resultObject?.Errors.Count());
        }

        [Fact]
        public async Task CanSerializeWithNewtonSoftJson()
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
