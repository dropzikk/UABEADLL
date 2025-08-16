using System;
using System.Linq;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccNamedColor2TagDataEntry : IccTagDataEntry, IEquatable<IccNamedColor2TagDataEntry>
{
	public int CoordinateCount { get; }

	public string? Prefix { get; }

	public string? Suffix { get; }

	public int VendorFlags { get; }

	public IccNamedColor[] Colors { get; }

	public IccNamedColor2TagDataEntry(IccNamedColor[] colors)
		: this(0, null, null, colors, IccProfileTag.Unknown)
	{
	}

	public IccNamedColor2TagDataEntry(string prefix, string suffix, IccNamedColor[] colors)
		: this(0, prefix, suffix, colors, IccProfileTag.Unknown)
	{
	}

	public IccNamedColor2TagDataEntry(int vendorFlags, string prefix, string suffix, IccNamedColor[] colors)
		: this(vendorFlags, prefix, suffix, colors, IccProfileTag.Unknown)
	{
	}

	public IccNamedColor2TagDataEntry(IccNamedColor[] colors, IccProfileTag tagSignature)
		: this(0, null, null, colors, tagSignature)
	{
	}

	public IccNamedColor2TagDataEntry(string prefix, string suffix, IccNamedColor[] colors, IccProfileTag tagSignature)
		: this(0, prefix, suffix, colors, tagSignature)
	{
	}

	public IccNamedColor2TagDataEntry(int vendorFlags, string? prefix, string? suffix, IccNamedColor[] colors, IccProfileTag tagSignature)
		: base(IccTypeSignature.NamedColor2, tagSignature)
	{
		Guard.NotNull(colors, "colors");
		int coordinateCount = 0;
		if (colors.Length != 0)
		{
			ushort[] deviceCoordinates = colors[0].DeviceCoordinates;
			coordinateCount = ((deviceCoordinates != null) ? deviceCoordinates.Length : 0);
			Guard.IsFalse(colors.Any(delegate(IccNamedColor t)
			{
				ushort[] deviceCoordinates2 = t.DeviceCoordinates;
				return ((deviceCoordinates2 != null) ? deviceCoordinates2.Length : 0) != coordinateCount;
			}), "colors", "Device coordinate count must be the same for all colors");
		}
		VendorFlags = vendorFlags;
		CoordinateCount = coordinateCount;
		Prefix = prefix;
		Suffix = suffix;
		Colors = colors;
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccNamedColor2TagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccNamedColor2TagDataEntry? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (base.Equals(other) && CoordinateCount == other.CoordinateCount && string.Equals(Prefix, other.Prefix, StringComparison.OrdinalIgnoreCase) && string.Equals(Suffix, other.Suffix, StringComparison.OrdinalIgnoreCase) && VendorFlags == other.VendorFlags)
		{
			return Colors.AsSpan().SequenceEqual(other.Colors);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccNamedColor2TagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, CoordinateCount, Prefix, Suffix, VendorFlags, Colors);
	}
}
