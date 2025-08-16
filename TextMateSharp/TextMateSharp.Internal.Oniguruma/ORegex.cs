using System;
using System.Text;

namespace TextMateSharp.Internal.Oniguruma;

public class ORegex : IDisposable
{
	private static object _createRegexSync = new object();

	private IntPtr _regex;

	private IntPtr _region;

	private bool _disposed;

	private object _syncObject = new object();

	private string _regexString;

	public bool Valid => _regex != IntPtr.Zero;

	public unsafe ORegex(string pattern, bool ignoreCase = true, bool multiline = false)
	{
		int ignoreCaseArg = (ignoreCase ? 1 : 0);
		int multilineArg = (multiline ? 1 : 0);
		pattern = UnicodeCharEscape.AddBracesToUnicodePatterns(pattern);
		pattern = UnicodeCharEscape.ConstraintUnicodePatternLenght(pattern);
		lock (_createRegexSync)
		{
			fixed (char* patternPtr = pattern)
			{
				_regex = OnigInterop.Instance.onigwrap_create(patternPtr, Encoding.Unicode.GetByteCount(patternPtr, pattern.Length), ignoreCaseArg, multilineArg);
			}
		}
		if (!Valid)
		{
			_regexString = pattern;
		}
	}

	public unsafe OnigResult SafeSearch(string text, int offset = 0)
	{
		if (_disposed)
		{
			throw new ObjectDisposedException("ORegex");
		}
		if (!Valid)
		{
			throw new ArgumentException($"Invalid Onigmo regular expression: {_regexString}");
		}
		lock (_syncObject)
		{
			if (_region == IntPtr.Zero)
			{
				_region = OnigInterop.Instance.onigwrap_region_new();
			}
			fixed (char* textPtr = text)
			{
				OnigInterop.Instance.onigwrap_search(_regex, textPtr, Encoding.Unicode.GetByteCount(textPtr, offset), Encoding.Unicode.GetByteCount(textPtr, text.Length), _region);
			}
			int captureCount = OnigInterop.Instance.onigwrap_num_regs(_region);
			Region region = null;
			for (int capture = 0; capture < captureCount; capture++)
			{
				int pos = OnigInterop.Instance.onigwrap_pos(_region, capture);
				if (capture == 0 && pos < 0)
				{
					return null;
				}
				int len = ((pos != -1) ? OnigInterop.Instance.onigwrap_len(_region, capture) : 0);
				if (region == null)
				{
					region = new Region(in captureCount);
				}
				region.Start[capture] = pos;
				region.End[capture] = pos + len;
			}
			return new OnigResult(region, -1);
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (_disposed)
		{
			return;
		}
		lock (_syncObject)
		{
			_disposed = true;
			if (_region != IntPtr.Zero)
			{
				OnigInterop.Instance.onigwrap_region_free(_region);
			}
			if (_regex != IntPtr.Zero)
			{
				OnigInterop.Instance.onigwrap_free(_regex);
			}
		}
	}

	~ORegex()
	{
		Dispose(disposing: false);
	}
}
