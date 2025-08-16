using System;

namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Decoder;

internal readonly struct AdobeMarker : IEquatable<AdobeMarker>
{
	public const int Length = 12;

	public short DCTEncodeVersion { get; }

	public short APP14Flags0 { get; }

	public short APP14Flags1 { get; }

	public byte ColorTransform { get; }

	private AdobeMarker(short dctEncodeVersion, short app14Flags0, short app14Flags1, byte colorTransform)
	{
		DCTEncodeVersion = dctEncodeVersion;
		APP14Flags0 = app14Flags0;
		APP14Flags1 = app14Flags1;
		ColorTransform = colorTransform;
	}

	public static bool TryParse(ReadOnlySpan<byte> bytes, out AdobeMarker marker)
	{
		if (ProfileResolver.IsProfile(bytes, ProfileResolver.AdobeMarker))
		{
			short dctEncodeVersion = (short)((bytes[5] << 8) | bytes[6]);
			short app14Flags = (short)((bytes[7] << 8) | bytes[8]);
			short app14Flags2 = (short)((bytes[9] << 8) | bytes[10]);
			byte colorTransform = bytes[11];
			marker = new AdobeMarker(dctEncodeVersion, app14Flags, app14Flags2, colorTransform);
			return true;
		}
		marker = default(AdobeMarker);
		return false;
	}

	public bool Equals(AdobeMarker other)
	{
		if (DCTEncodeVersion == other.DCTEncodeVersion && APP14Flags0 == other.APP14Flags0 && APP14Flags1 == other.APP14Flags1)
		{
			return ColorTransform == other.ColorTransform;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is AdobeMarker other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(DCTEncodeVersion, APP14Flags0, APP14Flags1, ColorTransform);
	}
}
