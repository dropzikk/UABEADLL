using System;
using System.ComponentModel;
using System.Text;
using Avalonia.Utilities;

namespace Avalonia.Media;

public struct BoxShadows
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public struct BoxShadowsEnumerator
	{
		private int _index;

		private BoxShadows _shadows;

		public BoxShadow Current => _shadows[_index];

		public BoxShadowsEnumerator(BoxShadows shadows)
		{
			_shadows = shadows;
			_index = -1;
		}

		public bool MoveNext()
		{
			_index++;
			return _index < _shadows.Count;
		}
	}

	private readonly BoxShadow _first;

	private readonly BoxShadow[]? _list;

	private static readonly char[] s_Separators = new char[1] { ',' };

	public int Count { get; }

	public BoxShadow this[int c]
	{
		get
		{
			if (c < 0 || c >= Count)
			{
				throw new IndexOutOfRangeException();
			}
			if (c == 0)
			{
				return _first;
			}
			return _list[c - 1];
		}
	}

	public bool HasInsetShadows
	{
		get
		{
			BoxShadowsEnumerator enumerator = GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoxShadow current = enumerator.Current;
				if (current != default(BoxShadow) && current.IsInset)
				{
					return true;
				}
			}
			return false;
		}
	}

	public BoxShadows(BoxShadow shadow)
	{
		_first = shadow;
		_list = null;
		Count = ((!(_first == default(BoxShadow))) ? 1 : 0);
	}

	public BoxShadows(BoxShadow first, BoxShadow[] rest)
	{
		_first = first;
		_list = rest;
		Count = 1 + ((rest != null) ? rest.Length : 0);
	}

	public override string ToString()
	{
		if (Count == 0)
		{
			return "none";
		}
		StringBuilder stringBuilder = StringBuilderCache.Acquire();
		BoxShadowsEnumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			enumerator.Current.ToString(stringBuilder);
			stringBuilder.Append(',');
			stringBuilder.Append(' ');
		}
		stringBuilder.Remove(stringBuilder.Length - 2, 2);
		return StringBuilderCache.GetStringAndRelease(stringBuilder);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public BoxShadowsEnumerator GetEnumerator()
	{
		return new BoxShadowsEnumerator(this);
	}

	public static BoxShadows Parse(string s)
	{
		string[] array = s.Split(s_Separators, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 0 || (array.Length == 1 && (string.IsNullOrWhiteSpace(array[0]) || array[0] == "none")))
		{
			return default(BoxShadows);
		}
		BoxShadow boxShadow = BoxShadow.Parse(array[0]);
		if (array.Length == 1)
		{
			return new BoxShadows(boxShadow);
		}
		BoxShadow[] array2 = new BoxShadow[array.Length - 1];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = BoxShadow.Parse(array[i + 1]);
		}
		return new BoxShadows(boxShadow, array2);
	}

	public Rect TransformBounds(in Rect rect)
	{
		Rect result = rect;
		BoxShadowsEnumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			result = result.Union(enumerator.Current.TransformBounds(in rect));
		}
		return result;
	}

	public bool Equals(BoxShadows other)
	{
		if (other.Count != Count)
		{
			return false;
		}
		for (int i = 0; i < Count; i++)
		{
			if (!this[i].Equals(other[i]))
			{
				return false;
			}
		}
		return true;
	}

	public override bool Equals(object? obj)
	{
		if (obj is BoxShadows other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = 0;
		BoxShadowsEnumerator enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			num = (num * 397) ^ enumerator.Current.GetHashCode();
		}
		return num;
	}

	public static bool operator ==(BoxShadows left, BoxShadows right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(BoxShadows left, BoxShadows right)
	{
		return !(left == right);
	}
}
