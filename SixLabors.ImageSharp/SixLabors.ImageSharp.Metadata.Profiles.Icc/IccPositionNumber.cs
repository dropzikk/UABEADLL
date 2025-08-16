using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal readonly struct IccPositionNumber : IEquatable<IccPositionNumber>
{
	public uint Offset { get; }

	public uint Size { get; }

	public IccPositionNumber(uint offset, uint size)
	{
		Offset = offset;
		Size = size;
	}

	public static bool operator ==(IccPositionNumber left, IccPositionNumber right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(IccPositionNumber left, IccPositionNumber right)
	{
		return !left.Equals(right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccPositionNumber other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(IccPositionNumber other)
	{
		if (Offset == other.Offset)
		{
			return Size == other.Size;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (int)(Offset ^ Size);
	}

	public override string ToString()
	{
		return $"{Offset}; {Size}";
	}
}
