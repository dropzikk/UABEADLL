using System;
using System.Diagnostics;

namespace Avalonia.Media;

[DebuggerDisplay("CharacterHit({FirstCharacterIndex}, {TrailingLength})")]
public readonly struct CharacterHit : IEquatable<CharacterHit>
{
	public int FirstCharacterIndex { get; }

	public int TrailingLength { get; }

	[DebuggerStepThrough]
	public CharacterHit(int firstCharacterIndex, int trailingLength = 0)
	{
		FirstCharacterIndex = firstCharacterIndex;
		TrailingLength = trailingLength;
	}

	public bool Equals(CharacterHit other)
	{
		if (FirstCharacterIndex == other.FirstCharacterIndex)
		{
			return TrailingLength == other.TrailingLength;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is CharacterHit other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (FirstCharacterIndex * 397) ^ TrailingLength;
	}

	public static bool operator ==(CharacterHit left, CharacterHit right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(CharacterHit left, CharacterHit right)
	{
		return !left.Equals(right);
	}
}
