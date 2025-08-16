using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Avalonia.Utilities;

namespace Avalonia.Media;

public struct BoxShadow
{
	private struct ArrayReader
	{
		private int _index;

		private string[] _arr;

		public ArrayReader(string[] arr)
		{
			_arr = arr;
			_index = 0;
		}

		public bool TryReadString([MaybeNullWhen(false)] out string s)
		{
			s = null;
			if (_index >= _arr.Length)
			{
				return false;
			}
			s = _arr[_index];
			_index++;
			return true;
		}

		public string ReadString()
		{
			if (!TryReadString(out string s))
			{
				throw new FormatException();
			}
			return s;
		}
	}

	private static readonly char[] s_Separator = new char[2] { ' ', '\t' };

	public double OffsetX { get; set; }

	public double OffsetY { get; set; }

	public double Blur { get; set; }

	public double Spread { get; set; }

	public Color Color { get; set; }

	public bool IsInset { get; set; }

	public bool Equals(in BoxShadow other)
	{
		if (OffsetX.Equals(other.OffsetX) && OffsetY.Equals(other.OffsetY) && Blur.Equals(other.Blur) && Spread.Equals(other.Spread))
		{
			return Color.Equals(other.Color);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is BoxShadow other)
		{
			return Equals(in other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (((((((OffsetX.GetHashCode() * 397) ^ OffsetY.GetHashCode()) * 397) ^ Blur.GetHashCode()) * 397) ^ Spread.GetHashCode()) * 397) ^ Color.GetHashCode();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = StringBuilderCache.Acquire();
		ToString(stringBuilder);
		return StringBuilderCache.GetStringAndRelease(stringBuilder);
	}

	internal void ToString(StringBuilder sb)
	{
		if (this == default(BoxShadow))
		{
			sb.Append("none");
			return;
		}
		if (IsInset)
		{
			sb.Append("inset ");
		}
		if (OffsetX != 0.0)
		{
			sb.AppendFormat("{0} ", OffsetX.ToString(CultureInfo.InvariantCulture));
		}
		if (OffsetY != 0.0)
		{
			sb.AppendFormat("{0} ", OffsetY.ToString(CultureInfo.InvariantCulture));
		}
		if (Blur != 0.0)
		{
			sb.AppendFormat("{0} ", Blur.ToString(CultureInfo.InvariantCulture));
		}
		if (Spread != 0.0)
		{
			sb.AppendFormat("{0} ", Spread.ToString(CultureInfo.InvariantCulture));
		}
		sb.AppendFormat("{0}", Color.ToString());
	}

	public static BoxShadow Parse(string s)
	{
		if (s == null)
		{
			throw new ArgumentNullException();
		}
		if (s.Length == 0)
		{
			throw new FormatException();
		}
		string[] array = s.Split(s_Separator, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 1 && array[0] == "none")
		{
			return default(BoxShadow);
		}
		if (array.Length < 3 || array.Length > 6)
		{
			throw new FormatException();
		}
		bool isInset = false;
		ArrayReader arrayReader = new ArrayReader(array);
		string text = arrayReader.ReadString();
		if (text == "inset")
		{
			isInset = true;
			text = arrayReader.ReadString();
		}
		double offsetX = double.Parse(text, CultureInfo.InvariantCulture);
		double offsetY = double.Parse(arrayReader.ReadString(), CultureInfo.InvariantCulture);
		double blur = 0.0;
		double spread = 0.0;
		arrayReader.TryReadString(out string s2);
		arrayReader.TryReadString(out string s3);
		arrayReader.TryReadString(out string s4);
		if (s3 != null)
		{
			blur = double.Parse(s2, CultureInfo.InvariantCulture);
		}
		if (s4 != null)
		{
			spread = double.Parse(s3, CultureInfo.InvariantCulture);
		}
		Color color = Color.Parse(s4 ?? s3 ?? s2);
		BoxShadow result = default(BoxShadow);
		result.IsInset = isInset;
		result.OffsetX = offsetX;
		result.OffsetY = offsetY;
		result.Blur = blur;
		result.Spread = spread;
		result.Color = color;
		return result;
	}

	public Rect TransformBounds(in Rect rect)
	{
		if (!IsInset)
		{
			return rect.Translate(new Vector(OffsetX, OffsetY)).Inflate(Spread + Blur);
		}
		return rect;
	}

	public static bool operator ==(BoxShadow left, BoxShadow right)
	{
		return left.Equals(in right);
	}

	public static bool operator !=(BoxShadow left, BoxShadow right)
	{
		return !(left == right);
	}
}
