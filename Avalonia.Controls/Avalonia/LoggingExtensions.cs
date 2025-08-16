using Avalonia.Logging;

namespace Avalonia;

public static class LoggingExtensions
{
	public static AppBuilder LogToTrace(this AppBuilder builder, LogEventLevel level = LogEventLevel.Warning, params string[] areas)
	{
		Logger.Sink = new TraceLogSink(level, areas);
		return builder;
	}
}
