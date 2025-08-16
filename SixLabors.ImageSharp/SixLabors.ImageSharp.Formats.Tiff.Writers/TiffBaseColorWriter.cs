using System;
using SixLabors.ImageSharp.Formats.Tiff.Compression;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.Writers;

internal abstract class TiffBaseColorWriter<TPixel> : IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
	private bool isDisposed;

	public abstract int BitsPerPixel { get; }

	public int BytesPerRow => (int)((uint)(Image.Width * BitsPerPixel + 7) / 8u);

	protected ImageFrame<TPixel> Image { get; }

	protected MemoryAllocator MemoryAllocator { get; }

	protected Configuration Configuration { get; }

	protected TiffEncoderEntriesCollector EntriesCollector { get; }

	protected TiffBaseColorWriter(ImageFrame<TPixel> image, MemoryAllocator memoryAllocator, Configuration configuration, TiffEncoderEntriesCollector entriesCollector)
	{
		Image = image;
		MemoryAllocator = memoryAllocator;
		Configuration = configuration;
		EntriesCollector = entriesCollector;
	}

	public virtual void Write(TiffBaseCompressor compressor, int rowsPerStrip)
	{
		int num = (Image.Height + rowsPerStrip - 1) / rowsPerStrip;
		uint[] array = new uint[num];
		uint[] array2 = new uint[num];
		int num2 = 0;
		compressor.Initialize(rowsPerStrip);
		for (int i = 0; i < Image.Height; i += rowsPerStrip)
		{
			long position = compressor.Output.Position;
			int height = Math.Min(rowsPerStrip, Image.Height - i);
			EncodeStrip(i, height, compressor);
			long position2 = compressor.Output.Position;
			array[num2] = (uint)position;
			array2[num2] = (uint)(position2 - position);
			num2++;
		}
		AddStripTags(rowsPerStrip, array, array2);
	}

	public void Dispose()
	{
		if (!isDisposed)
		{
			isDisposed = true;
			Dispose(disposing: true);
		}
	}

	protected abstract void EncodeStrip(int y, int height, TiffBaseCompressor compressor);

	private void AddStripTags(int rowsPerStrip, uint[] stripOffsets, uint[] stripByteCounts)
	{
		EntriesCollector.AddOrReplace(new ExifLong(ExifTagValue.RowsPerStrip)
		{
			Value = (uint)rowsPerStrip
		});
		EntriesCollector.AddOrReplace(new ExifLongArray(ExifTagValue.StripOffsets)
		{
			Value = stripOffsets
		});
		EntriesCollector.AddOrReplace(new ExifLongArray(ExifTagValue.StripByteCounts)
		{
			Value = stripByteCounts
		});
	}

	protected abstract void Dispose(bool disposing);
}
