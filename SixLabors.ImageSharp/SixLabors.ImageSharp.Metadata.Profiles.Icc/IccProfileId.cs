using System;
using System.Globalization;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

public readonly struct IccProfileId : IEquatable<IccProfileId>
{
	public static readonly IccProfileId Zero;

	public uint Part1 { get; }

	public uint Part2 { get; }

	public uint Part3 { get; }

	public uint Part4 { get; }

	public bool IsSet => !Equals(Zero);

	public IccProfileId(uint p1, uint p2, uint p3, uint p4)
	{
		Part1 = p1;
		Part2 = p2;
		Part3 = p3;
		Part4 = p4;
	}

	public static bool operator ==(IccProfileId left, IccProfileId right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(IccProfileId left, IccProfileId right)
	{
		return !left.Equals(right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccProfileId other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(IccProfileId other)
	{
		if (Part1 == other.Part1 && Part2 == other.Part2 && Part3 == other.Part3)
		{
			return Part4 == other.Part4;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Part1, Part2, Part3, Part4);
	}

	public override string ToString()
	{
		return $"{ToHex(Part1)}-{ToHex(Part2)}-{ToHex(Part3)}-{ToHex(Part4)}";
	}

	private static string ToHex(uint value)
	{
		return value.ToString("X", CultureInfo.InvariantCulture).PadLeft(8, '0');
	}
}
