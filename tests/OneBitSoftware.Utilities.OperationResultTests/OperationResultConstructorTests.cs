namespace OneBitSoftware.Utilities.OperationResultTests;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Xunit;

public class OperationResultConstructorTests
{
    [Fact]
    public void NewOperationResult_ShouldNotBeNull()
    {
        var sut = new OperationResult();

        Assert.NotNull(sut);
        Assert.True(sut.Success);
        Assert.False(sut.Fail);
    }

    [Fact]
    public void NewOperationResultWithNullLogger_ShouldSucceed()
    {
        var sut = new OperationResult(null);

        Assert.NotNull(sut);
        Assert.True(sut.Success);
        Assert.False(sut.Fail);
    }

    [Fact]
    public void NewOperationResultWithLogger_ShouldSucceed()
    {

        var logger = new DebugLoggerProvider();
        var sut = new OperationResult(logger.CreateLogger("Tests"));

        Assert.NotNull(sut);
        Assert.True(sut.Success);
        Assert.False(sut.Fail);
    }

    [Fact]
    public void NewOperationResultT_ShouldNotBeNull()
    {
        var sut = new OperationResult<object>();

        Assert.NotNull(sut);
        Assert.True(sut.Success);
        Assert.False(sut.Fail);
    }

    [Fact]
    public void NewOperationResultTWithNullLogger_ShouldSucceed()
    {
        var sut = new OperationResult<object>(logger: null);

        Assert.NotNull(sut);
        Assert.True(sut.Success);
        Assert.False(sut.Fail);
    }

    [Fact]
    public void NewOperationResultFromException_ShouldSucceed()
    {
        var result = OperationResult.FromException(new Exception());
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.True(result.Fail);
    }

    [Fact]
    public void NewOperationResultTFromException_ShouldSucceed()
    {
        var result = OperationResult<object>.FromException(new Exception());
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.True(result.Fail);
    }

    [Fact]
    public void NewOperationResultTFromError_ShouldSucceed()
    {
        var errorCode = 666;
        var message = "Error";
        var detail = "Detail";
        var result = OperationResult<object>.FromError(message, errorCode, LogLevel.Debug, detail, null);
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.True(result.Fail);
        Assert.Contains(result.Errors, e => e.Code.Equals(errorCode));
        Assert.Contains(result.Errors, e => e.Message.Equals(message));
        Assert.Contains(result.Errors, e => e.Details != null && e.Details.Equals(detail));
        Assert.Equal(1, result.Errors.Count);
    }

    [Fact]
    public void NewOperationResultFromError_ShouldSucceed()
    {
        var errorCode = 666;
        var message = "Error";
        var detail = "Detail";
        var result = OperationResult.FromError(message, errorCode, LogLevel.Debug, detail, null);
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.True(result.Fail);
        Assert.Contains(result.Errors, e => e.Code.Equals(errorCode));
        Assert.Contains(result.Errors, e => e.Message.Equals(message));
        Assert.Contains(result.Errors, e => e.Details != null && e.Details.Equals(detail));
        Assert.Equal(1, result.Errors.Count);
    }
}
