using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal readonly struct IccNamedColor : IEquatable<IccNamedColor>
{
	public string Name { get; }

	public ushort[] PcsCoordinates { get; }

	public ushort[] DeviceCoordinates { get; }

	public IccNamedColor(string name, ushort[] pcsCoordinates, ushort[] deviceCoordinates)
	{
		Guard.NotNull(name, "name");
		Guard.NotNull(pcsCoordinates, "pcsCoordinates");
		Guard.IsTrue(pcsCoordinates.Length == 3, "pcsCoordinates", "Must have a length of 3");
		Name = name;
		PcsCoordinates = pcsCoordinates;
		DeviceCoordinates = deviceCoordinates;
	}

	public static bool operator ==(IccNamedColor left, IccNamedColor right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(IccNamedColor left, IccNamedColor right)
	{
		return !left.Equals(right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccNamedColor other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(IccNamedColor other)
	{
		if (Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) && PcsCoordinates.AsSpan().SequenceEqual(other.PcsCoordinates))
		{
			return DeviceCoordinates.AsSpan().SequenceEqual(other.DeviceCoordinates);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Name, PcsCoordinates, DeviceCoordinates);
	}

	public override string ToString()
	{
		return Name;
	}
}
