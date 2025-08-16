using System;
using System.Collections;
using System.Collections.Generic;
using Avalonia.Utilities;

namespace Avalonia.Media.Fonts;

public sealed class FamilyNameCollection : IReadOnlyList<string>, IEnumerable<string>, IEnumerable, IReadOnlyCollection<string>
{
	private readonly string[] _names;

	public string PrimaryFamilyName { get; }

	public bool HasFallbacks { get; }

	public int Count => _names.Length;

	public string this[int index] => _names[index];

	public FamilyNameCollection(string familyNames)
	{
		if (familyNames == null)
		{
			throw new ArgumentNullException("familyNames");
		}
		_names = SplitNames(familyNames);
		PrimaryFamilyName = _names[0];
		HasFallbacks = _names.Length > 1;
	}

	private static string[] SplitNames(string names)
	{
		return names.Split(',', StringSplitOptions.TrimEntries);
	}

	public ImmutableReadOnlyListStructEnumerator<string> GetEnumerator()
	{
		return new ImmutableReadOnlyListStructEnumerator<string>(this);
	}

	IEnumerator<string> IEnumerable<string>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public override string ToString()
	{
		return string.Join(", ", _names);
	}

	public override int GetHashCode()
	{
		if (_names.Length == 0)
		{
			return 0;
		}
		int num = 17;
		for (int i = 0; i < _names.Length; i++)
		{
			string text = _names[i];
			num = num * 23 + text.GetHashCode();
		}
		return num;
	}

	public static bool operator !=(FamilyNameCollection? a, FamilyNameCollection? b)
	{
		return !(a == b);
	}

	public static bool operator ==(FamilyNameCollection? a, FamilyNameCollection? b)
	{
		if ((object)a == b)
		{
			return true;
		}
		return a?.Equals(b) ?? false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is FamilyNameCollection familyNameCollection)
		{
			return _names.AsSpan().SequenceEqual(familyNameCollection._names);
		}
		return false;
	}
}
