using System;
using System.IO;
using SixLabors.ImageSharp.Common.Helpers;

namespace SixLabors.ImageSharp.Formats.Webp.Chunks;

internal readonly struct WebpFrameData
{
	public const uint HeaderSize = 16u;

	public uint DataSize { get; }

	public uint X { get; }

	public uint Y { get; }

	public uint Width { get; }

	public uint Height { get; }

	public uint Duration { get; }

	public WebpBlendMethod BlendingMethod { get; }

	public WebpDisposalMethod DisposalMethod { get; }

	public Rectangle Bounds => new Rectangle((int)X, (int)Y, (int)Width, (int)Height);

	public WebpFrameData(uint dataSize, uint x, uint y, uint width, uint height, uint duration, WebpBlendMethod blendingMethod, WebpDisposalMethod disposalMethod)
	{
		DataSize = dataSize;
		X = x;
		Y = y;
		Width = width;
		Height = height;
		Duration = duration;
		DisposalMethod = disposalMethod;
		BlendingMethod = blendingMethod;
	}

	public WebpFrameData(uint dataSize, uint x, uint y, uint width, uint height, uint duration, int flags)
		: this(dataSize, x, y, width, height, duration, ((flags & 2) == 0) ? WebpBlendMethod.Over : WebpBlendMethod.Source, ((flags & 1) == 1) ? WebpDisposalMethod.RestoreToBackground : WebpDisposalMethod.DoNotDispose)
	{
	}

	public WebpFrameData(uint x, uint y, uint width, uint height, uint duration, WebpBlendMethod blendingMethod, WebpDisposalMethod disposalMethod)
		: this(0u, x, y, width, height, duration, blendingMethod, disposalMethod)
	{
	}

	public long WriteHeaderTo(Stream stream)
	{
		byte b = 0;
		if (BlendingMethod == WebpBlendMethod.Source)
		{
			b |= 2;
		}
		if (DisposalMethod == WebpDisposalMethod.RestoreToBackground)
		{
			b |= 1;
		}
		long result = RiffHelper.BeginWriteChunk(stream, 1095650630u);
		WebpChunkParsingUtils.WriteUInt24LittleEndian(stream, (uint)Math.Round((float)X / 2f));
		WebpChunkParsingUtils.WriteUInt24LittleEndian(stream, (uint)Math.Round((float)Y / 2f));
		WebpChunkParsingUtils.WriteUInt24LittleEndian(stream, Width - 1);
		WebpChunkParsingUtils.WriteUInt24LittleEndian(stream, Height - 1);
		WebpChunkParsingUtils.WriteUInt24LittleEndian(stream, Duration);
		stream.WriteByte(b);
		return result;
	}

	public static WebpFrameData Parse(Stream stream)
	{
		Span<byte> buffer = stackalloc byte[4];
		return new WebpFrameData(WebpChunkParsingUtils.ReadChunkSize(stream, buffer), WebpChunkParsingUtils.ReadUInt24LittleEndian(stream, buffer) * 2, WebpChunkParsingUtils.ReadUInt24LittleEndian(stream, buffer) * 2, WebpChunkParsingUtils.ReadUInt24LittleEndian(stream, buffer) + 1, WebpChunkParsingUtils.ReadUInt24LittleEndian(stream, buffer) + 1, WebpChunkParsingUtils.ReadUInt24LittleEndian(stream, buffer), stream.ReadByte());
	}
}
