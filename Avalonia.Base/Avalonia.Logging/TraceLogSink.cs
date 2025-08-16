#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Avalonia.Utilities;

namespace Avalonia.Logging;

internal class TraceLogSink : ILogSink
{
	private readonly LogEventLevel _level;

	private readonly IList<string>? _areas;

	public TraceLogSink(LogEventLevel minimumLevel, IList<string>? areas = null)
	{
		_level = minimumLevel;
		_areas = ((areas != null && areas.Count > 0) ? areas : null);
	}

	public bool IsEnabled(LogEventLevel level, string area)
	{
		if (level >= _level)
		{
			return _areas?.Contains(area) ?? true;
		}
		return false;
	}

	public void Log(LogEventLevel level, string area, object? source, string messageTemplate)
	{
		if (IsEnabled(level, area))
		{
			Trace.WriteLine(Format<object, object, object>(area, messageTemplate, source, null));
		}
	}

	public void Log(LogEventLevel level, string area, object? source, string messageTemplate, params object?[] propertyValues)
	{
		if (IsEnabled(level, area))
		{
			Trace.WriteLine(Format(area, messageTemplate, source, propertyValues));
		}
	}

	private static string Format<T0, T1, T2>(string area, string template, object? source, object?[]? values)
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire(template.Length);
		CharacterReader characterReader = new CharacterReader(template.AsSpan());
		int num = 0;
		stringBuilder.Append('[');
		stringBuilder.Append(area);
		stringBuilder.Append("] ");
		while (!characterReader.End)
		{
			char c = characterReader.Take();
			if (c != '{')
			{
				stringBuilder.Append(c);
			}
			else if (characterReader.Peek != '{')
			{
				stringBuilder.Append('\'');
				stringBuilder.Append((values != null) ? values[num++] : null);
				stringBuilder.Append('\'');
				characterReader.TakeUntil('}');
				characterReader.Take();
			}
			else
			{
				stringBuilder.Append('{');
				characterReader.Take();
			}
		}
		if (source != null)
		{
			stringBuilder.Append(" (");
			stringBuilder.Append(source.GetType().Name);
			stringBuilder.Append(" #");
			stringBuilder.Append(source.GetHashCode());
			stringBuilder.Append(')');
		}
		return StringBuilderCache.GetStringAndRelease(stringBuilder);
	}

	private static string Format(string area, string template, object? source, object?[] v)
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire(template.Length);
		CharacterReader characterReader = new CharacterReader(template.AsSpan());
		int num = 0;
		stringBuilder.Append('[');
		stringBuilder.Append(area);
		stringBuilder.Append(']');
		while (!characterReader.End)
		{
			char c = characterReader.Take();
			if (c != '{')
			{
				stringBuilder.Append(c);
			}
			else if (characterReader.Peek != '{')
			{
				stringBuilder.Append('\'');
				stringBuilder.Append((num < v.Length) ? v[num++] : null);
				stringBuilder.Append('\'');
				characterReader.TakeUntil('}');
				characterReader.Take();
			}
			else
			{
				stringBuilder.Append('{');
				characterReader.Take();
			}
		}
		if (source != null)
		{
			stringBuilder.Append('(');
			stringBuilder.Append(source.GetType().Name);
			stringBuilder.Append(" #");
			stringBuilder.Append(source.GetHashCode());
			stringBuilder.Append(')');
		}
		return StringBuilderCache.GetStringAndRelease(stringBuilder);
	}
}
