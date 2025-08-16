using System.Runtime.CompilerServices;
using System.Text;

namespace Avalonia.Logging;

public readonly record struct ParametrizedLogger(ILogSink sink, LogEventLevel level, string area)
{
	public bool IsValid => sink != null;

	private readonly ILogSink _sink = sink;

	private readonly LogEventLevel _level = level;

	private readonly string _area = area;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Log(object? source, string messageTemplate)
	{
		sink.Log(level, area, source, messageTemplate);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Log<T0>(object? source, string messageTemplate, T0 propertyValue0)
	{
		sink.Log(level, area, source, messageTemplate, propertyValue0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Log<T0, T1>(object? source, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
	{
		sink.Log(level, area, source, messageTemplate, propertyValue0, propertyValue1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Log<T0, T1, T2>(object? source, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
	{
		sink.Log(level, area, source, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Log<T0, T1, T2, T3>(object? source, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2, T3 propertyValue3)
	{
		sink.Log(level, area, source, messageTemplate, propertyValue0, propertyValue1, propertyValue2, propertyValue3);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Log<T0, T1, T2, T3, T4>(object? source, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2, T3 propertyValue3, T4 propertyValue4)
	{
		sink.Log(level, area, source, messageTemplate, propertyValue0, propertyValue1, propertyValue2, propertyValue3, propertyValue4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Log<T0, T1, T2, T3, T4, T5>(object? source, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2, T3 propertyValue3, T4 propertyValue4, T5 propertyValue5)
	{
		sink.Log(level, area, source, messageTemplate, propertyValue0, propertyValue1, propertyValue2, propertyValue3, propertyValue4, propertyValue5);
	}

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append("IsValid = ");
		builder.Append(IsValid.ToString());
		return true;
	}
}
