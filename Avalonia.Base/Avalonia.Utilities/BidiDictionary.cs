using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media.TextFormatting;

namespace Avalonia.Utilities;

internal sealed class BidiDictionary<T1, T2> where T1 : notnull where T2 : notnull
{
	private Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();

	private Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

	public void ClearThenResetIfTooLarge()
	{
		FormattingBufferHelper.ClearThenResetIfTooLarge(ref _forward);
		FormattingBufferHelper.ClearThenResetIfTooLarge(ref _reverse);
	}

	public void Add(T1 key, T2 value)
	{
		_forward.Add(key, value);
		_reverse.Add(value, key);
	}

	public bool TryGetValue(T1 key, [MaybeNullWhen(false)] out T2 value)
	{
		return _forward.TryGetValue(key, out value);
	}

	public bool TryGetKey(T2 value, [MaybeNullWhen(false)] out T1 key)
	{
		return _reverse.TryGetValue(value, out key);
	}

	public bool ContainsKey(T1 key)
	{
		return _forward.ContainsKey(key);
	}

	public bool ContainsValue(T2 value)
	{
		return _reverse.ContainsKey(value);
	}
}
