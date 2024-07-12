using UnityEngine;

public class CustomLogHandler : ILogHandler
{
	private readonly ILogHandler originalLogHandler;

	public CustomLogHandler(ILogHandler originalLogHandler)
	{
		this.originalLogHandler = originalLogHandler;
	}

	public void LogFormat(LogType logType, Object context, string format, params object[] args)
	{
		// Suppress NullReferenceException messages
		if (logType == LogType.Error && format.Contains("NullReferenceException"))
		{
			return;
		}

		// Forward all other log messages to the original log handler
		originalLogHandler.LogFormat(logType, context, format, args);
	}

	public void LogException(System.Exception exception, Object context)
	{
		// Suppress NullReferenceException messages
		if (exception is System.NullReferenceException)
		{
			return;
		}

		// Forward all other exceptions to the original log handler
		originalLogHandler.LogException(exception, context);
	}
}
