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
}
