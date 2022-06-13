# OperationResult
A reusable class for capturing multiple errors and success tracking when performing all kinds of code operations. OperationResult lets you return a valid object instead of throwing and capturing exceptions.
The OperationResult class will also automatically log entries if an instance of ILogger has been passed during construction.

Example:
```cs
public OperationResult CustomLogicMethod()
{
	var operationResult = new OperationResult();
	
	// Do custom logic work

	operationResult.AppendError("The custom logic did not work");
	operationResult.AppendException(new Exception("The custom logic method threw and exception."));

	return operationResult;
}

var customLogicResult = CustomLogicMethod();
// customLogicResult.Success is false
// customLogicResult.Errors contains two errors, one with the set message and the other with the set exception.
```

## Returning data and objects
You can instantiate an OperationResult object by specifying a return type as a generic:

```cs
public OperationResult<MemoryStream> GetStream()
{
	var operationResult = new OperationResult<MemoryStream>();
	operationResult.ResultObject = [stream reference];
	return operationResult;
}

var getStreamResult = GetStream();
// getStreamResult.ResultObject contains a reference of the resulting object of MemoryStream type;
```

## Passing an instance of ILogger.
OperationResult will log to a passed ILogger if it is passed during construction.
Every method provides a log-level override.
```cs
public OperationResult CustomLogicMethod(ILogger logger)
{
	var operationResult = new OperationResult(logger);
	
	// Do custom logic work

	operationResult.AppendError("The custom logic did not work"); // this call will be logged to the logger with severity Error.
	operationResult.AppendException(new Exception("The custom logic method threw and exception.")); // this call will be logged to the logger with severity Error.

	return operationResult;
}
```