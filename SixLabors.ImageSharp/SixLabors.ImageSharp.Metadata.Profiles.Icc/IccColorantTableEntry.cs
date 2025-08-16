using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal readonly struct IccColorantTableEntry : IEquatable<IccColorantTableEntry>
{
	public string Name { get; }

	public ushort Pcs1 { get; }

	public ushort Pcs2 { get; }

	public ushort Pcs3 { get; }

	public IccColorantTableEntry(string name)
		: this(name, 0, 0, 0)
	{
	}

	public IccColorantTableEntry(string name, ushort pcs1, ushort pcs2, ushort pcs3)
	{
		Name = name ?? throw new ArgumentNullException("name");
		Pcs1 = pcs1;
		Pcs2 = pcs2;
		Pcs3 = pcs3;
	}

	public static bool operator ==(IccColorantTableEntry left, IccColorantTableEntry right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(IccColorantTableEntry left, IccColorantTableEntry right)
	{
		return !left.Equals(right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccColorantTableEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(IccColorantTableEntry other)
	{
		if (Name == other.Name && Pcs1 == other.Pcs1 && Pcs2 == other.Pcs2)
		{
			return Pcs3 == other.Pcs3;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Name, Pcs1, Pcs2, Pcs3);
	}

	public override string ToString()
	{
		return $"{Name}: {Pcs1}; {Pcs2}; {Pcs3}";
	}
}
