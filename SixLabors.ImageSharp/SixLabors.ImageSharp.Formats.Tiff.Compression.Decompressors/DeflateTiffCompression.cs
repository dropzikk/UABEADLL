using System;
using System.IO.Compression;
using System.Threading;
using SixLabors.ImageSharp.Compression.Zlib;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal sealed class DeflateTiffCompression : TiffBaseDecompressor
{
	private readonly bool isBigEndian;

	private readonly TiffColorType colorType;

	public DeflateTiffCompression(MemoryAllocator memoryAllocator, int width, int bitsPerPixel, TiffColorType colorType, TiffPredictor predictor, bool isBigEndian)
		: base(memoryAllocator, width, bitsPerPixel, predictor)
	{
		this.colorType = colorType;
		this.isBigEndian = isBigEndian;
	}

	protected override void Decompress(BufferedReadStream stream, int byteCount, int stripHeight, Span<byte> buffer, CancellationToken cancellationToken)
	{
		long pos = stream.Position;
		using (ZlibInflateStream zlibInflateStream = new ZlibInflateStream(stream, delegate
		{
			int num = (int)(byteCount - (stream.Position - pos));
			return (num > 0) ? num : 0;
		}))
		{
			if (zlibInflateStream.AllocateNewBytes(byteCount, isCriticalChunk: true))
			{
				DeflateStream compressedStream = zlibInflateStream.CompressedStream;
				int num2;
				for (int i = 0; i < buffer.Length; i += num2)
				{
					num2 = compressedStream.Read(buffer, i, buffer.Length - i);
					if (num2 <= 0)
					{
						break;
					}
				}
			}
		}
		if (base.Predictor == TiffPredictor.Horizontal)
		{
			HorizontalPredictor.Undo(buffer, base.Width, colorType, isBigEndian);
		}
	}

	protected override void Dispose(bool disposing)
	{
	}
}
