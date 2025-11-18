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

    [Fact]
    public void AppendErrors_NonGenericSource_To_GenericTarget_Retains_Generic_Type_And_MergesErrors()
    {
        // Arrange
        var source = new OperationResult();
        source.AppendError("E1", 101, LogLevel.Warning, "D1");

        var target = new OperationResult<string>();
        target.AppendError("E0", 100, LogLevel.Information, "D0");

        // Act
        var returned = target.AppendErrors(source);

        // Assert
        Assert.Same(target, returned);
        Assert.True(target.Fail);
        Assert.Equal(2, target.Errors.Count);
        Assert.NotNull(target.Errors.Single(e => e is { Code: 100, Message: "E0" }));
        Assert.NotNull(target.Errors.Single(e => e is { Code: 101, Message: "E1" }));
    }

    [Fact]
    public void AppendErrors_GenericSource_To_GenericTarget_Retains_Generic_Type_And_MergesErrors()
    {
        // Arrange
        var source = new OperationResult<double>();
        source.AppendError("E1", 101, LogLevel.Warning, "D1");

        var target = new OperationResult<string>();
        target.AppendError("E0", 100, LogLevel.Information, "D0");

        // Act
        var returned = target.AppendErrors(source);

        // Assert
        Assert.Same(target, returned);
        Assert.True(target.Fail);
        Assert.Equal(2, target.Errors.Count);
        Assert.NotNull(target.Errors.Single(e => e is { Code: 100, Message: "E0" }));
        Assert.NotNull(target.Errors.Single(e => e is { Code: 101, Message: "E1" }));
    }

    [Fact]
    public void AppendErrors_WithNullSource_On_NonGenericTarget_Returns_Same_Instance_And_NoChange()
    {
        // Arrange
        var target = new OperationResult();
        target.AppendError("E0", 100, LogLevel.Information, "D0");
        var beforeCount = target.Errors.Count;

        // Act
        var returned = target.AppendErrors(null);

        // Assert
        Assert.Same(target, returned);
        Assert.Equal(beforeCount, target.Errors.Count);
    }

    [Fact]
    public void AppendErrors_WithNullSource_On_GenericTarget_Returns_Same_Instance_And_NoChange()
    {
        // Arrange
        var target = new OperationResult<object>();
        target.AppendError("E0", 100, LogLevel.Information, "D0");
        var beforeCount = target.Errors.Count;

        // Act
        var returned = target.AppendErrors((OperationResult)null);

        // Assert
        Assert.Same(target, returned);
        Assert.Equal(beforeCount, target.Errors.Count);
    }

    [Fact]
    public void AppendErrors_PreservesResultObject_WhenMergingErrors()
    {
        // Arrange
        var originalResult = new { Id = 42, Name = "Test" };
        var target = new OperationResult<object>(originalResult);

        var source = new OperationResult();
        source.AppendError("Source error", 500);

        // Act
        var returned = target.AppendErrors(source);

        // Assert
        Assert.Same(target, returned);
        Assert.Same(originalResult, target.ResultObject);
        Assert.Single(target.Errors);
        Assert.True(target.Fail);
    }

    [Fact]
    public void AppendErrors_MaintainsFluentChaining_WithGenericType()
    {
        // Arrange
        var source1 = new OperationResult();
        source1.AppendError("E1", 1);

        var source2 = new OperationResult<int>();
        source2.AppendError("E2", 2);

        var target = new OperationResult<string>();

        // Act - Chain multiple AppendErrors calls
        var returned = target
            .AppendErrors(source1)
            .AppendErrors(source2)
            .AppendError("E3", 3);

        // Assert
        Assert.Same(target, returned);
        Assert.IsType<OperationResult<string>>(returned);
        Assert.Equal(3, target.Errors.Count);
    }

    [Fact]
    public void AppendErrors_PreservesInitialException_FromSource()
    {
        // Arrange
        var sourceException = new InvalidOperationException("Source exception");
        var source = new OperationResult();
        source.AppendException(sourceException, 999);

        var target = new OperationResult<double>();

        // Act
        target.AppendErrors(source);

        // Assert
        Assert.Null(target.InitialException); // Target doesn't inherit source's InitialException
        Assert.Single(target.Errors);
        Assert.Contains(sourceException.ToString(), target.Errors[0].Message);
    }

    [Fact]
    public void AppendErrors_HandlesSourceWithMultipleErrorTypes()
    {
        // Arrange
        var source = new OperationResult<List<string>>();
        source.AppendError("Standard error", 100);
        source.AppendException(new ArgumentNullException("param"), 200);
        source.AppendError("Another error", 300, LogLevel.Critical, "Critical details");

        var target = new OperationResult<int>();

        // Act
        target.AppendErrors(source);

        // Assert
        Assert.Equal(3, target.Errors.Count);
        Assert.NotNull(target.Errors.Single(e => e.Code == 100));
        Assert.NotNull(target.Errors.Single(e => e.Code == 200));
        Assert.NotNull(target.Errors.Single(e => e is { Code: 300, Details: "Critical details" }));
    }

    [Fact]
    public void AppendErrors_MergesSuccessMessages_AreNotTransferred()
    {
        // Arrange
        var source = new OperationResult();
        source.AddSuccessMessage("Success from source");
        source.AppendError("But has error", 1);

        var target = new OperationResult<string>();
        target.AddSuccessMessage("Success from target");

        // Act
        target.AppendErrors(source);

        // Assert - SuccessMessages are not merged in AppendErrors
        Assert.Single(target.SuccessMessages);
        Assert.Contains("Success from target", target.SuccessMessages);
    }

    [Fact]
    public void AppendErrors_EmptySource_DoesNotAffectTarget()
    {
        // Arrange
        var source = new OperationResult(); // No errors
        var target = new OperationResult<string>();
        target.AppendError("Target error", 1);

        // Act
        var returned = target.AppendErrors(source);

        // Assert
        Assert.Same(target, returned);
        Assert.Single(target.Errors);
        Assert.True(target.Fail);
    }

    [Fact]
    public void AppendErrors_EmptyTarget_AcceptsSourceErrors()
    {
        // Arrange
        var source = new OperationResult();
        source.AppendError("Source error", 100);

        var target = new OperationResult<string>();

        // Act
        target.AppendErrors(source);

        // Assert
        Assert.True(target.Fail);
        Assert.Single(target.Errors);
        Assert.Equal("Source error", target.Errors[0].Message);
    }

    [Fact]
    public void AppendErrors_WithDifferentGenericTypes_MaintainsTargetType()
    {
        // Arrange
        var sourceInt = new OperationResult<int> { ResultObject = 42 };
        sourceInt.AppendError("Int error", 1);

        var targetString = new OperationResult<string> { ResultObject = "test" };

        // Act
        var returned = targetString.AppendErrors(sourceInt);

        // Assert
        Assert.IsType<OperationResult<string>>(returned);
        Assert.Equal("test", targetString.ResultObject);
        Assert.Single(targetString.Errors);
    }

    [Fact]
    public void AppendErrors_ChainedCalls_AccumulatesAllErrors()
    {
        // Arrange
        var s1 = new OperationResult();
        s1.AppendError("E1", 1);

        var s2 = new OperationResult<int>();
        s2.AppendError("E2", 2);

        var s3 = new OperationResult();
        s3.AppendError("E3", 3);

        var target = new OperationResult<string>();

        // Act
        target.AppendErrors(s1).AppendErrors(s2).AppendErrors(s3);

        // Assert
        Assert.Equal(3, target.Errors.Count);
        Assert.Contains(target.Errors, e => e.Code == 1);
        Assert.Contains(target.Errors, e => e.Code == 2);
        Assert.Contains(target.Errors, e => e.Code == 3);
    }

    [Fact]
    public void AppendErrors_WithComplexGenericType_PreservesTypeIntegrity()
    {
        // Arrange
        var complexObject = new Dictionary<string, List<int>>
        {
            ["key1"] = [1, 2, 3]
        };

        var target = new OperationResult<Dictionary<string, List<int>>>(complexObject);
        var source = new OperationResult();
        source.AppendError("Complex type error", 999);

        // Act
        var returned = target.AppendErrors(source);

        // Assert
        Assert.Same(complexObject, target.ResultObject);
        Assert.IsType<OperationResult<Dictionary<string, List<int>>>>(returned);
        Assert.Single(target.Errors);
    }

    [Fact]
    public void AppendErrors_ReturnsCorrectType_AfterMultipleOperations()
    {
        // Arrange
        var target = new OperationResult<int> { ResultObject = 100 };
        var source1 = new OperationResult();
        source1.AppendError("E1", 1);

        // Act
        var step1 = target.AppendErrors(source1);
        var step2 = step1.AppendError("E2", 2);
        var step3 = step2.AppendException(new Exception("E3"), 3);

        // Assert
        Assert.IsType<OperationResult<int>>(step1);
        Assert.IsType<OperationResult<int>>(step2);
        Assert.IsType<OperationResult<int>>(step3);
        Assert.Same(target, step3);
        Assert.Equal(3, target.Errors.Count);
    }
}
