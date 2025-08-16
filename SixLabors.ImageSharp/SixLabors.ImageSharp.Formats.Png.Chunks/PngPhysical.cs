using System;
using System.Buffers.Binary;
using SixLabors.ImageSharp.Common.Helpers;
using SixLabors.ImageSharp.Metadata;

namespace SixLabors.ImageSharp.Formats.Png.Chunks;

internal readonly struct PngPhysical
{
	public const int Size = 9;

	public uint XAxisPixelsPerUnit { get; }

	public uint YAxisPixelsPerUnit { get; }

	public byte UnitSpecifier { get; }

	public PngPhysical(uint x, uint y, byte unitSpecifier)
	{
		XAxisPixelsPerUnit = x;
		YAxisPixelsPerUnit = y;
		UnitSpecifier = unitSpecifier;
	}

	public static PngPhysical Parse(ReadOnlySpan<byte> data)
	{
		uint x = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(0, 4));
		uint y = BinaryPrimitives.ReadUInt32BigEndian(data.Slice(4, 4));
		byte unitSpecifier = data[8];
		return new PngPhysical(x, y, unitSpecifier);
	}

	public static PngPhysical FromMetadata(ImageMetadata meta)
	{
		byte unitSpecifier;
		uint x;
		uint y;
		switch (meta.ResolutionUnits)
		{
		case PixelResolutionUnit.AspectRatio:
			unitSpecifier = 0;
			x = (uint)Math.Round(meta.HorizontalResolution);
			y = (uint)Math.Round(meta.VerticalResolution);
			break;
		case PixelResolutionUnit.PixelsPerInch:
			unitSpecifier = 1;
			x = (uint)Math.Round(UnitConverter.InchToMeter(meta.HorizontalResolution));
			y = (uint)Math.Round(UnitConverter.InchToMeter(meta.VerticalResolution));
			break;
		case PixelResolutionUnit.PixelsPerCentimeter:
			unitSpecifier = 1;
			x = (uint)Math.Round(UnitConverter.CmToMeter(meta.HorizontalResolution));
			y = (uint)Math.Round(UnitConverter.CmToMeter(meta.VerticalResolution));
			break;
		default:
			unitSpecifier = 1;
			x = (uint)Math.Round(meta.HorizontalResolution);
			y = (uint)Math.Round(meta.VerticalResolution);
			break;
		}
		return new PngPhysical(x, y, unitSpecifier);
	}

	public void WriteTo(Span<byte> buffer)
	{
		BinaryPrimitives.WriteUInt32BigEndian(buffer.Slice(0, 4), XAxisPixelsPerUnit);
		BinaryPrimitives.WriteUInt32BigEndian(buffer.Slice(4, 4), YAxisPixelsPerUnit);
		buffer[8] = UnitSpecifier;
	}
}
