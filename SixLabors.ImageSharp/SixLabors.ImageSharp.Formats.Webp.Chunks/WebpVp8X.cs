using System;
using System.IO;
using SixLabors.ImageSharp.Common.Helpers;

namespace SixLabors.ImageSharp.Formats.Webp.Chunks;

internal readonly struct WebpVp8X : IEquatable<WebpVp8X>
{
	public bool HasAnimation { get; }

	public bool HasXmp { get; }

	public bool HasExif { get; }

	public bool HasAlpha { get; }

	public bool HasIcc { get; }

	public uint Width { get; }

	public uint Height { get; }

	public WebpVp8X(bool hasAnimation, bool hasXmp, bool hasExif, bool hasAlpha, bool hasIcc, uint width, uint height)
	{
		HasAnimation = hasAnimation;
		HasXmp = hasXmp;
		HasExif = hasExif;
		HasAlpha = hasAlpha;
		HasIcc = hasIcc;
		Width = width;
		Height = height;
	}

	public static bool operator ==(WebpVp8X left, WebpVp8X right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(WebpVp8X left, WebpVp8X right)
	{
		return !(left == right);
	}

	public override bool Equals(object? obj)
	{
		if (obj is WebpVp8X other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(WebpVp8X other)
	{
		if (HasAnimation == other.HasAnimation && HasXmp == other.HasXmp && HasExif == other.HasExif && HasAlpha == other.HasAlpha && HasIcc == other.HasIcc && Width == other.Width)
		{
			return Height == other.Height;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(HasAnimation, HasXmp, HasExif, HasAlpha, HasIcc, Width, Height);
	}

	public void Validate(uint maxDimension, ulong maxCanvasPixels)
	{
		if (Width > maxDimension || Height > maxDimension)
		{
			WebpThrowHelper.ThrowInvalidImageDimensions($"Image width or height exceeds maximum allowed dimension of {maxDimension}");
		}
		if (Width * Height > maxCanvasPixels)
		{
			WebpThrowHelper.ThrowInvalidImageDimensions("The product of image width and height MUST be at most 2^32 - 1");
		}
	}

	public WebpVp8X WithAlpha(bool hasAlpha)
	{
		return new WebpVp8X(HasAnimation, HasXmp, HasExif, hasAlpha, HasIcc, Width, Height);
	}

	public void WriteTo(Stream stream)
	{
		byte b = 0;
		if (HasAnimation)
		{
			b |= 2;
		}
		if (HasXmp)
		{
			b |= 4;
		}
		if (HasExif)
		{
			b |= 8;
		}
		if (HasAlpha)
		{
			b |= 0x10;
		}
		if (HasIcc)
		{
			b |= 0x20;
		}
		long sizePosition = RiffHelper.BeginWriteChunk(stream, 1448097880u);
		stream.WriteByte(b);
		stream.Position += 3L;
		WebpChunkParsingUtils.WriteUInt24LittleEndian(stream, Width - 1);
		WebpChunkParsingUtils.WriteUInt24LittleEndian(stream, Height - 1);
		RiffHelper.EndWriteChunk(stream, sizePosition);
	}
}
