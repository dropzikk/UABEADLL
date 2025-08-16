using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal readonly struct IccTagTableEntry : IEquatable<IccTagTableEntry>
{
	public IccProfileTag Signature { get; }

	public uint Offset { get; }

	public uint DataSize { get; }

	public IccTagTableEntry(IccProfileTag signature, uint offset, uint dataSize)
	{
		Signature = signature;
		Offset = offset;
		DataSize = dataSize;
	}

	public static bool operator ==(IccTagTableEntry left, IccTagTableEntry right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(IccTagTableEntry left, IccTagTableEntry right)
	{
		return !left.Equals(right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccTagTableEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(IccTagTableEntry other)
	{
		if (Signature.Equals(other.Signature) && Offset.Equals(other.Offset))
		{
			return DataSize.Equals(other.DataSize);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Signature, Offset, DataSize);
	}

	public override string ToString()
	{
		return $"{Signature} (Offset: {Offset}; Size: {DataSize})";
	}
}
