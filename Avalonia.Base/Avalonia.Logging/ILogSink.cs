namespace Avalonia.Logging;

public interface ILogSink
{
	bool IsEnabled(LogEventLevel level, string area);

	void Log(LogEventLevel level, string area, object? source, string messageTemplate);

	void Log(LogEventLevel level, string area, object? source, string messageTemplate, params object?[] propertyValues);
}
