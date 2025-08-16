namespace Avalonia.Logging;

public static class Logger
{
	public static ILogSink? Sink { get; set; }

	public static bool IsEnabled(LogEventLevel level, string area)
	{
		return Sink?.IsEnabled(level, area) ?? false;
	}

	public static ParametrizedLogger? TryGet(LogEventLevel level, string area)
	{
		if (!IsEnabled(level, area))
		{
			return null;
		}
		return new ParametrizedLogger(Sink, level, area);
	}

	public static bool TryGet(LogEventLevel level, string area, out ParametrizedLogger outLogger)
	{
		ParametrizedLogger? parametrizedLogger = TryGet(level, area);
		outLogger = parametrizedLogger.GetValueOrDefault();
		return parametrizedLogger.HasValue;
	}
}
