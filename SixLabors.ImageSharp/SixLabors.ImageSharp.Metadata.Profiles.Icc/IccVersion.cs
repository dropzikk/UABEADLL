using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

public readonly struct IccVersion : IEquatable<IccVersion>
{
	public int Major { get; }

	public int Minor { get; }

	public int Patch { get; }

	public IccVersion(int major, int minor, int patch)
	{
		Major = major;
		Minor = minor;
		Patch = patch;
	}

	public static bool operator ==(IccVersion left, IccVersion right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(IccVersion left, IccVersion right)
	{
		return !(left == right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccVersion other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(IccVersion other)
	{
		if (Major == other.Major && Minor == other.Minor)
		{
			return Patch == other.Patch;
		}
		return false;
	}

	public override string ToString()
	{
		return string.Join(".", Major, Minor, Patch);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Major, Minor, Patch);
	}
}
