namespace OneBitSoftware.Utilities.OperationResultTests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;

public class OperationResultAppendErrorsTests
{
    [Fact]
    public void AppendErrors_ShouldListAllErrors()
    {
        // Arrange
        var errorCode1 = 666;
        var message1 = "Error";
        var detail1 = "Detail";
        var operationResultTarget = new OperationResult();
        operationResultTarget.AppendError(message1, errorCode1, LogLevel.Debug, detail1);
        operationResultTarget.AppendException(new Exception(message1));

        var errorCode2 = 669;
        var message2 = "Error2";
        var detail2 = "Detail2";
        var operationResultBase = new OperationResult();
        operationResultBase.AppendError(message2, errorCode2, LogLevel.Debug, detail2);

        // Act
        operationResultBase.AppendErrors(operationResultTarget);

        // Assert
        Assert.False(operationResultBase.Success);
        Assert.False(operationResultTarget.Success);
        Assert.True(operationResultBase.Fail);
        Assert.True(operationResultTarget.Fail);

        Assert.Equal(3, operationResultBase.Errors.Count);
        Assert.Equal(2, operationResultTarget.Errors.Count);

        Assert.NotNull(operationResultBase.Errors.Single(r => r.Code.Equals(errorCode2)));
        Assert.NotNull(operationResultBase.Errors.Single(r => r.Message.Equals(message2)));
        Assert.NotNull(operationResultBase.Errors.Single(r => r.Details is not null && r.Details.Equals(detail2)));
    }

    [Fact]
    public void AppendErrorsT_ShouldListAllErrors()
    {
        // Arrange
        var errorCode1 = 666;
        var message1 = "Error";
        var detail1 = "Detail";
        var operationResultTarget = new OperationResult<object>();
        operationResultTarget.AppendError(message1, errorCode1, LogLevel.Debug, detail1);
        operationResultTarget.AppendException(new Exception(message1));

        var errorCode2 = 669;
        var message2 = "Error2";
        var detail2 = "Detail2";
        var operationResultBase = new OperationResult<object>();
        operationResultBase.AppendError(message2, errorCode2, LogLevel.Debug, detail2);

        // Act - AppendErrorMessages is to be removed
        operationResultBase.AppendErrors(operationResultTarget);

        // Assert
        Assert.False(operationResultBase.Success);
        Assert.False(operationResultTarget.Success);
        Assert.True(operationResultBase.Fail);
        Assert.True(operationResultTarget.Fail);

        Assert.Equal(3, operationResultBase.Errors.Count);
        Assert.Equal(2, operationResultTarget.Errors.Count);

        Assert.NotNull(operationResultBase.Errors.Single(r => r.Code.Equals(errorCode2)));
        Assert.NotNull(operationResultBase.Errors.Single(r => r.Message.Equals(message2)));
        Assert.NotNull(operationResultBase.Errors.Single(r => r.Details is not null && r.Details.Equals(detail2)));
    }

    [Fact]
    public void AppendErrorsStringInt_ShouldListAllErrors()
    {
        // Arrange
        var errorCode1 = 666;
        var message1 = "Error";
        var detail1 = "Detail";
        var operationResultTarget = new OperationResult<int>();
        operationResultTarget.AppendError(message1, errorCode1, LogLevel.Debug, detail1);
        operationResultTarget.AppendException(new Exception(message1));

        var errorCode2 = 669;
        var message2 = "Error2";
        var detail2 = "Detail2";
        var operationResultBase = new OperationResult<string>();
        operationResultBase.AppendError(message2, errorCode2, LogLevel.Debug, detail2);

        // Act
        operationResultBase.AppendErrors(operationResultTarget);

        // Assert
        Assert.False(operationResultBase.Success);
        Assert.False(operationResultTarget.Success);
        Assert.True(operationResultBase.Fail);
        Assert.True(operationResultTarget.Fail);

        Assert.Equal(3, operationResultBase.Errors.Count);
        Assert.Equal(2, operationResultTarget.Errors.Count);

        Assert.NotNull(operationResultBase.Errors.Single(r => r.Code.Equals(errorCode2)));
        Assert.NotNull(operationResultBase.Errors.Single(r => r.Message.Equals(message2)));
        Assert.NotNull(operationResultBase.Errors.Single(r => r.Details is not null && r.Details.Equals(detail2)));
    }

    [Fact]
    public void AppendErrors_ShouldLogWhenCreatedWithALogger()
    {
        // Arrange
        var testLogger = new TestLogger();
        var operationResultNoLogger = new OperationResult();
        var operationResultWithLogger = new OperationResult(testLogger);

        // Act
        operationResultNoLogger.AppendError("test");
        operationResultWithLogger.AppendErrors(operationResultNoLogger);

        // Assert
        Assert.Equal(1, testLogger.LogMessages.Count);
    }

    [Fact]
    public void AppendErrors_ShouldLogOnceWhenCreatedWithALogger()
    {
        // Arrange
        var testLogger = new TestLogger();
        var operationResultWithLogger = new OperationResult(testLogger);
        var operationResultWithLogger2 = new OperationResult(testLogger);

        // Act
        operationResultWithLogger2.AppendError("test");
        operationResultWithLogger.AppendErrors(operationResultWithLogger2);

        // Assert
        Assert.Equal(1, testLogger.LogMessages.Count);
    }

    [Fact]
    public void AppendErrors_ShouldLogOnceWhenNestingWithALogger()
    {
        // Arrange
        var testLogger = new TestLogger();
        var operationResultWithLogger = new OperationResult(testLogger);
        var operationResultWithLogger2 = new OperationResult(testLogger);
        var operationResultWithLogger3 = new OperationResult(testLogger);

        // Act
        operationResultWithLogger3.AppendError("test1");
        operationResultWithLogger2.AppendError("test2");
        operationResultWithLogger.AppendErrors(operationResultWithLogger2);

        // Assert
        Assert.Equal(2, testLogger.LogMessages.Count);
    }

    [Fact]
    public void AppendErrors_ShouldLogWhenCreatedWithNoLogger()
    {
        // Arrange
        var testLogger = new TestLogger();
        var operationResultNoLogger = new OperationResult();
        var operationResultWithLogger = new OperationResult(testLogger);

        // Act
        operationResultWithLogger.AppendError("test");
        operationResultNoLogger.AppendErrors(operationResultNoLogger);

        // Assert
        Assert.Equal(1, testLogger.LogMessages.Count);
    }
}
