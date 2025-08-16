using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Avalonia.Utilities;

public record struct StringTokenizer : IDisposable
{
	public string? CurrentToken
	{
		get
		{
			if (_tokenIndex >= 0)
			{
				return _s.Substring(_tokenIndex, _tokenLength);
			}
			return null;
		}
	}

	private const char DefaultSeparatorChar = ',';

	private readonly string _s;

	private readonly int _length;

	private readonly char _separator;

	private readonly string? _exceptionMessage;

	private readonly IFormatProvider _formatProvider;

	private int _index;

	private int _tokenIndex;

	private int _tokenLength;

	public StringTokenizer(string s, IFormatProvider formatProvider, string? exceptionMessage = null)
		: this(s, GetSeparatorFromFormatProvider(formatProvider), exceptionMessage)
	{
		_formatProvider = formatProvider;
	}

	public StringTokenizer(string s, char separator = ',', string? exceptionMessage = null)
	{
		_s = s ?? throw new ArgumentNullException("s");
		_length = s?.Length ?? 0;
		_separator = separator;
		_exceptionMessage = exceptionMessage;
		_formatProvider = CultureInfo.InvariantCulture;
		_index = 0;
		_tokenIndex = -1;
		_tokenLength = 0;
		while (_index < _length && char.IsWhiteSpace(_s, _index))
		{
			_index++;
		}
	}

	public void Dispose()
	{
		if (_index != _length)
		{
			throw GetFormatException();
		}
	}

	public bool TryReadInt32(out int result, char? separator = null)
	{
		if (TryReadString(out string result2, separator) && int.TryParse(result2, NumberStyles.Integer, _formatProvider, out result))
		{
			return true;
		}
		result = 0;
		return false;
	}

	public int ReadInt32(char? separator = null)
	{
		if (!TryReadInt32(out var result, separator))
		{
			throw GetFormatException();
		}
		return result;
	}

	public bool TryReadDouble(out double result, char? separator = null)
	{
		if (TryReadString(out string result2, separator) && double.TryParse(result2, NumberStyles.Float, _formatProvider, out result))
		{
			return true;
		}
		result = 0.0;
		return false;
	}

	public double ReadDouble(char? separator = null)
	{
		if (!TryReadDouble(out var result, separator))
		{
			throw GetFormatException();
		}
		return result;
	}

	public bool TryReadString([MaybeNullWhen(false)] out string result, char? separator = null)
	{
		bool result2 = TryReadToken(separator ?? _separator);
		result = CurrentToken;
		return result2;
	}

	public string ReadString(char? separator = null)
	{
		if (!TryReadString(out string result, separator))
		{
			throw GetFormatException();
		}
		return result;
	}

	private bool TryReadToken(char separator)
	{
		_tokenIndex = -1;
		if (_index >= _length)
		{
			return false;
		}
		char c = _s[_index];
		int index = _index;
		int num = 0;
		while (_index < _length)
		{
			c = _s[_index];
			if (char.IsWhiteSpace(c) || c == separator)
			{
				break;
			}
			_index++;
			num++;
		}
		SkipToNextToken(separator);
		_tokenIndex = index;
		_tokenLength = num;
		if (_tokenLength < 1)
		{
			throw GetFormatException();
		}
		return true;
	}

	private void SkipToNextToken(char separator)
	{
		if (_index >= _length)
		{
			return;
		}
		char c = _s[_index];
		if (c != separator && !char.IsWhiteSpace(c))
		{
			throw GetFormatException();
		}
		int num = 0;
		while (_index < _length)
		{
			c = _s[_index];
			if (c == separator)
			{
				num++;
				_index++;
				if (num > 1)
				{
					throw GetFormatException();
				}
			}
			else
			{
				if (!char.IsWhiteSpace(c))
				{
					break;
				}
				_index++;
			}
		}
		if (num > 0 && _index >= _length)
		{
			throw GetFormatException();
		}
	}

	private FormatException GetFormatException()
	{
		if (_exceptionMessage == null)
		{
			return new FormatException();
		}
		return new FormatException(_exceptionMessage);
	}

	private static char GetSeparatorFromFormatProvider(IFormatProvider provider)
	{
		char c = ',';
		NumberFormatInfo instance = NumberFormatInfo.GetInstance(provider);
		if (instance.NumberDecimalSeparator.Length > 0 && c == instance.NumberDecimalSeparator[0])
		{
			c = ';';
		}
		return c;
	}

	[CompilerGenerated]
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append("CurrentToken = ");
		builder.Append((object?)CurrentToken);
		return true;
	}
}
