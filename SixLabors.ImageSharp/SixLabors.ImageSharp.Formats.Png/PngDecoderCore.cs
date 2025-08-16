using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using SixLabors.ImageSharp.Common.Helpers;
using SixLabors.ImageSharp.Compression.Zlib;
using SixLabors.ImageSharp.Formats.Png.Chunks;
using SixLabors.ImageSharp.Formats.Png.Filters;
using SixLabors.ImageSharp.IO;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Memory.Internals;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Metadata.Profiles.Cicp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Metadata.Profiles.Icc;
using SixLabors.ImageSharp.Metadata.Profiles.Xmp;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Png;

internal sealed class PngDecoderCore : IImageDecoderInternals
{
	private readonly Configuration configuration;

	private readonly uint maxFrames;

	private readonly bool skipMetadata;

	private readonly bool colorMetadataOnly;

	private readonly MemoryAllocator memoryAllocator;

	private BufferedReadStream currentStream;

	private PngHeader header;

	private AnimationControl animationControl;

	private int bytesPerPixel;

	private int bytesPerSample;

	private int bytesPerScanline;

	private byte[] palette;

	private byte[] paletteAlpha;

	private IMemoryOwner<byte> previousScanline;

	private IMemoryOwner<byte> scanline;

	private PngColorType pngColorType;

	private PngChunk? nextChunk;

	private readonly PngCrcChunkHandling pngCrcChunkHandling;

	private readonly int maxUncompressedLength;

	public DecoderOptions Options { get; }

	public Size Dimensions => new Size(header.Width, header.Height);

	public PngDecoderCore(PngDecoderOptions options)
	{
		Options = options.GeneralOptions;
		configuration = options.GeneralOptions.Configuration;
		maxFrames = options.GeneralOptions.MaxFrames;
		skipMetadata = options.GeneralOptions.SkipMetadata;
		memoryAllocator = configuration.MemoryAllocator;
		pngCrcChunkHandling = options.PngCrcChunkHandling;
		maxUncompressedLength = options.MaxUncompressedAncillaryChunkSizeBytes;
	}

	internal PngDecoderCore(PngDecoderOptions options, bool colorMetadataOnly)
	{
		Options = options.GeneralOptions;
		this.colorMetadataOnly = colorMetadataOnly;
		maxFrames = options.GeneralOptions.MaxFrames;
		skipMetadata = true;
		configuration = options.GeneralOptions.Configuration;
		memoryAllocator = configuration.MemoryAllocator;
		pngCrcChunkHandling = options.PngCrcChunkHandling;
		maxUncompressedLength = options.MaxUncompressedAncillaryChunkSizeBytes;
	}

	public Image<TPixel> Decode<TPixel>(BufferedReadStream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		uint num = 0u;
		ImageMetadata imageMetadata = new ImageMetadata();
		PngMetadata pngMetadata = imageMetadata.GetPngMetadata();
		currentStream = stream;
		currentStream.Skip(8);
		Image<TPixel> image = null;
		FrameControl? previousFrameControl = null;
		FrameControl? frameControl = null;
		ImageFrame<TPixel> previousFrame = null;
		ImageFrame<TPixel> frame = null;
		Span<byte> buffer = stackalloc byte[20];
		try
		{
			PngChunk chunk;
			while (TryReadChunk(buffer, out chunk))
			{
				try
				{
					switch (chunk.Type)
					{
					case PngChunkType.Header:
						if (!object.Equals(header, default(PngHeader)))
						{
							PngThrowHelper.ThrowInvalidHeader();
						}
						ReadHeaderChunk(pngMetadata, chunk.Data.GetSpan());
						break;
					case PngChunkType.AnimationControl:
						ReadAnimationControlChunk(pngMetadata, chunk.Data.GetSpan());
						break;
					case PngChunkType.Physical:
						ReadPhysicalChunk(imageMetadata, chunk.Data.GetSpan());
						break;
					case PngChunkType.Gamma:
						ReadGammaChunk(pngMetadata, chunk.Data.GetSpan());
						break;
					case PngChunkType.Cicp:
						ReadCicpChunk(imageMetadata, chunk.Data.GetSpan());
						break;
					case PngChunkType.FrameControl:
						num++;
						if (num != maxFrames)
						{
							frame = null;
							frameControl = ReadFrameControlChunk(chunk.Data.GetSpan());
						}
						break;
					case PngChunkType.FrameData:
						if (num != maxFrames)
						{
							if (image == null)
							{
								PngThrowHelper.ThrowMissingDefaultData();
							}
							if (!frameControl.HasValue)
							{
								PngThrowHelper.ThrowMissingFrameControl();
							}
							InitializeFrame(previousFrameControl, frameControl.Value, image, previousFrame, out frame);
							currentStream.Position += 4L;
							ReadScanlines(chunk.Length - 4, frame, pngMetadata, ReadNextFrameDataChunk, frameControl.Value, cancellationToken);
							if (frameControl.Value.DisposeOperation != PngDisposalMethod.RestoreToPrevious)
							{
								previousFrame = frame;
								previousFrameControl = frameControl;
							}
						}
						break;
					case PngChunkType.Data:
					{
						pngMetadata.AnimateRootFrame = frameControl.HasValue;
						FrameControl valueOrDefault = frameControl.GetValueOrDefault();
						if (!frameControl.HasValue)
						{
							valueOrDefault = new FrameControl((uint)header.Width, (uint)header.Height);
							frameControl = valueOrDefault;
						}
						if (image == null)
						{
							InitializeImage(imageMetadata, frameControl.Value, out image);
							AssignColorPalette(palette, paletteAlpha, pngMetadata);
						}
						ReadScanlines(chunk.Length, image.Frames.RootFrame, pngMetadata, ReadNextDataChunk, frameControl.Value, cancellationToken);
						if (pngMetadata.AnimateRootFrame)
						{
							previousFrame = frame;
							previousFrameControl = frameControl;
						}
						break;
					}
					case PngChunkType.Palette:
						palette = chunk.Data.GetSpan().ToArray();
						break;
					case PngChunkType.Transparency:
						paletteAlpha = chunk.Data.GetSpan().ToArray();
						AssignTransparentMarkers(paletteAlpha, pngMetadata);
						break;
					case PngChunkType.Text:
						ReadTextChunk(imageMetadata, pngMetadata, chunk.Data.GetSpan());
						break;
					case PngChunkType.CompressedText:
						ReadCompressedTextChunk(imageMetadata, pngMetadata, chunk.Data.GetSpan());
						break;
					case PngChunkType.InternationalText:
						ReadInternationalTextChunk(imageMetadata, chunk.Data.GetSpan());
						break;
					case PngChunkType.Exif:
						if (!skipMetadata)
						{
							byte[] array = new byte[chunk.Length];
							chunk.Data.GetSpan().CopyTo(array);
							MergeOrSetExifProfile(imageMetadata, new ExifProfile(array), replaceExistingKeys: true);
						}
						break;
					case PngChunkType.EmbeddedColorProfile:
						ReadColorProfileChunk(imageMetadata, chunk.Data.GetSpan());
						break;
					case PngChunkType.ProprietaryApple:
						PngThrowHelper.ThrowInvalidChunkType("Proprietary Apple PNG detected! This PNG file is not conform to the specification and cannot be decoded.");
						break;
					case PngChunkType.End:
						goto end_IL_0048;
					}
				}
				finally
				{
					chunk.Data?.Dispose();
				}
				continue;
				end_IL_0048:
				break;
			}
			if (image == null)
			{
				PngThrowHelper.ThrowNoData();
			}
			return image;
		}
		catch
		{
			image?.Dispose();
			throw;
		}
		finally
		{
			scanline?.Dispose();
			previousScanline?.Dispose();
			nextChunk?.Data?.Dispose();
		}
	}

	public ImageInfo Identify(BufferedReadStream stream, CancellationToken cancellationToken)
	{
		uint num = 0u;
		ImageMetadata imageMetadata = new ImageMetadata();
		PngMetadata pngMetadata = imageMetadata.GetPngMetadata();
		currentStream = stream;
		FrameControl? frameControl = null;
		Span<byte> buffer = stackalloc byte[20];
		currentStream.Skip(8);
		try
		{
			PngChunk chunk;
			while (TryReadChunk(buffer, out chunk))
			{
				try
				{
					switch (chunk.Type)
					{
					case PngChunkType.Header:
						ReadHeaderChunk(pngMetadata, chunk.Data.GetSpan());
						goto end_IL_0047;
					case PngChunkType.AnimationControl:
						ReadAnimationControlChunk(pngMetadata, chunk.Data.GetSpan());
						goto end_IL_0047;
					case PngChunkType.Physical:
						if (colorMetadataOnly)
						{
							SkipChunkDataAndCrc(in chunk);
						}
						else
						{
							ReadPhysicalChunk(imageMetadata, chunk.Data.GetSpan());
						}
						goto end_IL_0047;
					case PngChunkType.Gamma:
						if (colorMetadataOnly)
						{
							SkipChunkDataAndCrc(in chunk);
						}
						else
						{
							ReadGammaChunk(pngMetadata, chunk.Data.GetSpan());
						}
						goto end_IL_0047;
					case PngChunkType.Cicp:
						if (colorMetadataOnly)
						{
							SkipChunkDataAndCrc(in chunk);
						}
						else
						{
							ReadCicpChunk(imageMetadata, chunk.Data.GetSpan());
						}
						goto end_IL_0047;
					case PngChunkType.FrameControl:
						num++;
						if (num != maxFrames)
						{
							frameControl = ReadFrameControlChunk(chunk.Data.GetSpan());
						}
						goto end_IL_0047;
					case PngChunkType.FrameData:
						if (num != maxFrames)
						{
							if (colorMetadataOnly)
							{
								break;
							}
							if (!frameControl.HasValue)
							{
								PngThrowHelper.ThrowMissingFrameControl();
							}
							currentStream.Skip(4);
							SkipChunkDataAndCrc(in chunk);
						}
						goto end_IL_0047;
					case PngChunkType.Data:
						if (colorMetadataOnly)
						{
							break;
						}
						SkipChunkDataAndCrc(in chunk);
						goto end_IL_0047;
					case PngChunkType.Palette:
						palette = chunk.Data.GetSpan().ToArray();
						goto end_IL_0047;
					case PngChunkType.Transparency:
						paletteAlpha = chunk.Data.GetSpan().ToArray();
						AssignTransparentMarkers(paletteAlpha, pngMetadata);
						if (colorMetadataOnly)
						{
							break;
						}
						goto end_IL_0047;
					case PngChunkType.Text:
						if (colorMetadataOnly)
						{
							SkipChunkDataAndCrc(in chunk);
						}
						else
						{
							ReadTextChunk(imageMetadata, pngMetadata, chunk.Data.GetSpan());
						}
						goto end_IL_0047;
					case PngChunkType.CompressedText:
						if (colorMetadataOnly)
						{
							SkipChunkDataAndCrc(in chunk);
						}
						else
						{
							ReadCompressedTextChunk(imageMetadata, pngMetadata, chunk.Data.GetSpan());
						}
						goto end_IL_0047;
					case PngChunkType.InternationalText:
						if (colorMetadataOnly)
						{
							SkipChunkDataAndCrc(in chunk);
						}
						else
						{
							ReadInternationalTextChunk(imageMetadata, chunk.Data.GetSpan());
						}
						goto end_IL_0047;
					case PngChunkType.Exif:
						if (colorMetadataOnly)
						{
							SkipChunkDataAndCrc(in chunk);
						}
						else if (!skipMetadata)
						{
							byte[] array = new byte[chunk.Length];
							chunk.Data.GetSpan().CopyTo(array);
							MergeOrSetExifProfile(imageMetadata, new ExifProfile(array), replaceExistingKeys: true);
						}
						goto end_IL_0047;
					default:
						if (colorMetadataOnly)
						{
							SkipChunkDataAndCrc(in chunk);
						}
						goto end_IL_0047;
					case PngChunkType.End:
						break;
					}
					break;
					end_IL_0047:;
				}
				finally
				{
					chunk.Data?.Dispose();
				}
			}
			if (header.Width == 0 && header.Height == 0)
			{
				PngThrowHelper.ThrowInvalidHeader();
			}
			AssignColorPalette(palette, paletteAlpha, pngMetadata);
			return new ImageInfo(new PixelTypeInfo(CalculateBitsPerPixel()), new Size(header.Width, header.Height), imageMetadata);
		}
		finally
		{
			scanline?.Dispose();
			previousScanline?.Dispose();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte ReadByteLittleEndian(ReadOnlySpan<byte> buffer, int offset)
	{
		return (byte)(((buffer[offset] & 0xFF) << 16) | (buffer[offset + 1] & 0xFF));
	}

	private bool TryScaleUpTo8BitArray(ReadOnlySpan<byte> source, int bytesPerScanline, int bits, [NotNullWhen(true)] out IMemoryOwner<byte>? buffer)
	{
		if (bits >= 8)
		{
			buffer = null;
			return false;
		}
		buffer = memoryAllocator.Allocate<byte>(bytesPerScanline * 8 / bits, AllocationOptions.Clean);
		ref byte reference = ref MemoryMarshal.GetReference(source);
		ref byte reference2 = ref buffer.GetReference();
		int num = 255 >> 8 - bits;
		int num2 = 0;
		for (int i = 0; i < bytesPerScanline; i++)
		{
			byte b = Unsafe.Add(ref reference, (uint)i);
			for (int j = 0; j < 8; j += bits)
			{
				int num3 = (b >> 8 - bits - j) & num;
				Unsafe.Add(ref reference2, (uint)num2) = (byte)num3;
				num2++;
			}
		}
		return true;
	}

	private static void ReadPhysicalChunk(ImageMetadata metadata, ReadOnlySpan<byte> data)
	{
		PngPhysical pngPhysical = PngPhysical.Parse(data);
		metadata.ResolutionUnits = ((pngPhysical.UnitSpecifier != 0) ? PixelResolutionUnit.PixelsPerMeter : PixelResolutionUnit.AspectRatio);
		metadata.HorizontalResolution = pngPhysical.XAxisPixelsPerUnit;
		metadata.VerticalResolution = pngPhysical.YAxisPixelsPerUnit;
	}

	private static void ReadGammaChunk(PngMetadata pngMetadata, ReadOnlySpan<byte> data)
	{
		if (data.Length >= 4)
		{
			pngMetadata.Gamma = (float)BinaryPrimitives.ReadUInt32BigEndian(data) * 1E-05f;
		}
	}

	private void InitializeImage<TPixel>(ImageMetadata metadata, FrameControl frameControl, out Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		image = new Image<TPixel>(configuration, header.Width, header.Height, metadata);
		image.Frames.RootFrame.Metadata.GetPngMetadata().FromChunk(in frameControl);
		bytesPerPixel = CalculateBytesPerPixel();
		bytesPerScanline = CalculateScanlineLength(header.Width) + 1;
		bytesPerSample = 1;
		if (header.BitDepth >= 8)
		{
			bytesPerSample = header.BitDepth / 8;
		}
		previousScanline?.Dispose();
		scanline?.Dispose();
		previousScanline = memoryAllocator.Allocate<byte>(bytesPerScanline, AllocationOptions.Clean);
		scanline = configuration.MemoryAllocator.Allocate<byte>(bytesPerScanline, AllocationOptions.Clean);
	}

	private void InitializeFrame<TPixel>(FrameControl? previousFrameControl, FrameControl currentFrameControl, Image<TPixel> image, ImageFrame<TPixel>? previousFrame, out ImageFrame<TPixel> frame) where TPixel : unmanaged, IPixel<TPixel>
	{
		frame = image.Frames.AddFrame(previousFrame ?? image.Frames.RootFrame);
		if (!previousFrameControl.HasValue || (previousFrame == null && previousFrameControl.Value.DisposeOperation == PngDisposalMethod.RestoreToPrevious))
		{
			frame.PixelBuffer.GetRegion().Clear();
		}
		else if (previousFrameControl.Value.DisposeOperation == PngDisposalMethod.RestoreToBackground)
		{
			Rectangle bounds = previousFrameControl.Value.Bounds;
			frame.PixelBuffer.GetRegion(bounds).Clear();
		}
		frame.Metadata.GetPngMetadata().FromChunk(in currentFrameControl);
		previousScanline?.Dispose();
		scanline?.Dispose();
		previousScanline = memoryAllocator.Allocate<byte>(bytesPerScanline, AllocationOptions.Clean);
		scanline = configuration.MemoryAllocator.Allocate<byte>(bytesPerScanline, AllocationOptions.Clean);
	}

	private int CalculateBitsPerPixel()
	{
		switch (pngColorType)
		{
		case PngColorType.Grayscale:
		case PngColorType.Palette:
			return header.BitDepth;
		case PngColorType.GrayscaleWithAlpha:
			return header.BitDepth * 2;
		case PngColorType.Rgb:
			return header.BitDepth * 3;
		case PngColorType.RgbWithAlpha:
			return header.BitDepth * 4;
		default:
			PngThrowHelper.ThrowNotSupportedColor();
			return -1;
		}
	}

	private int CalculateBytesPerPixel()
	{
		return pngColorType switch
		{
			PngColorType.Grayscale => (header.BitDepth != 16) ? 1 : 2, 
			PngColorType.GrayscaleWithAlpha => (header.BitDepth == 16) ? 4 : 2, 
			PngColorType.Palette => 1, 
			PngColorType.Rgb => (header.BitDepth == 16) ? 6 : 3, 
			_ => (header.BitDepth == 16) ? 8 : 4, 
		};
	}

	private int CalculateScanlineLength(int width)
	{
		int num = ((header.BitDepth == 16) ? 16 : 8);
		int num2 = width * header.BitDepth * bytesPerPixel;
		int num3 = num2 % num;
		if (num3 != 0)
		{
			num2 += num - num3;
		}
		return num2 / num;
	}

	private void ReadScanlines<TPixel>(int chunkLength, ImageFrame<TPixel> image, PngMetadata pngMetadata, Func<int> getData, in FrameControl frameControl, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		using ZlibInflateStream zlibInflateStream = new ZlibInflateStream(currentStream, getData);
		zlibInflateStream.AllocateNewBytes(chunkLength, isCriticalChunk: true);
		DeflateStream compressedStream = zlibInflateStream.CompressedStream;
		if (header.InterlaceMethod == PngInterlaceMode.Adam7)
		{
			DecodeInterlacedPixelData(in frameControl, compressedStream, image, pngMetadata, cancellationToken);
		}
		else
		{
			DecodePixelData(frameControl, compressedStream, image, pngMetadata, cancellationToken);
		}
	}

	private void DecodePixelData<TPixel>(FrameControl frameControl, DeflateStream compressedStream, ImageFrame<TPixel> imageFrame, PngMetadata pngMetadata, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		int i = (int)frameControl.YOffset;
		int j = 0;
		int yMax = (int)frameControl.YMax;
		IMemoryOwner<TPixel> memoryOwner = null;
		Span<TPixel> blendRowBuffer = Span<TPixel>.Empty;
		if (frameControl.BlendOperation == PngBlendMethod.Over)
		{
			memoryOwner = memoryAllocator.Allocate<TPixel>(imageFrame.Width, AllocationOptions.Clean);
			blendRowBuffer = memoryOwner.Memory.Span;
		}
		Span<byte> span2;
		for (; i < yMax; ProcessDefilteredScanline(in frameControl, i, span2, imageFrame, pngMetadata, blendRowBuffer), SwapScanlineBuffers(), i++)
		{
			cancellationToken.ThrowIfCancellationRequested();
			int num = CalculateScanlineLength((int)frameControl.Width) + 1;
			Span<byte> span = scanline.GetSpan();
			span2 = span.Slice(0, num);
			span = previousScanline.GetSpan();
			Span<byte> span3 = span.Slice(0, num);
			int num2;
			for (; j < num; j += num2)
			{
				num2 = compressedStream.Read(span2, j, num - j);
				if (num2 <= 0)
				{
					return;
				}
			}
			j = 0;
			switch ((FilterType)span2[0])
			{
			case FilterType.Sub:
				SubFilter.Decode(span2, bytesPerPixel);
				continue;
			case FilterType.Up:
				UpFilter.Decode(span2, span3);
				continue;
			case FilterType.Average:
				AverageFilter.Decode(span2, span3, bytesPerPixel);
				continue;
			case FilterType.Paeth:
				PaethFilter.Decode(span2, span3, bytesPerPixel);
				continue;
			default:
			{
				PngCrcChunkHandling pngCrcChunkHandling = this.pngCrcChunkHandling;
				if ((uint)(pngCrcChunkHandling - 2) > 1u)
				{
					PngThrowHelper.ThrowUnknownFilter();
					continue;
				}
				break;
			}
			case FilterType.None:
				continue;
			}
			break;
		}
		memoryOwner?.Dispose();
	}

	private void DecodeInterlacedPixelData<TPixel>(in FrameControl frameControl, DeflateStream compressedStream, ImageFrame<TPixel> imageFrame, PngMetadata pngMetadata, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		int num = Adam7.FirstRow[0] + (int)frameControl.YOffset;
		int i = 0;
		int num2 = 0;
		int width = (int)frameControl.Width;
		int yMax = (int)frameControl.YMax;
		Buffer2D<TPixel> pixelBuffer = imageFrame.PixelBuffer;
		IMemoryOwner<TPixel> memoryOwner = null;
		Span<TPixel> blendRowBuffer = Span<TPixel>.Empty;
		if (frameControl.BlendOperation == PngBlendMethod.Over)
		{
			memoryOwner = memoryAllocator.Allocate<TPixel>(imageFrame.Width, AllocationOptions.Clean);
			blendRowBuffer = memoryOwner.Memory.Span;
		}
		while (true)
		{
			int num3 = Adam7.ComputeColumns(width, num2);
			if (num3 == 0)
			{
				num2++;
				continue;
			}
			int num4 = CalculateScanlineLength(num3) + 1;
			while (true)
			{
				if (num < yMax)
				{
					cancellationToken.ThrowIfCancellationRequested();
					int num5;
					for (; i < num4; i += num5)
					{
						num5 = compressedStream.Read(scanline.GetSpan(), i, num4 - i);
						if (num5 <= 0)
						{
							return;
						}
					}
					i = 0;
					Span<byte> span = scanline.Slice(0, num4);
					Span<byte> span2 = previousScanline.Slice(0, num4);
					switch ((FilterType)span[0])
					{
					case FilterType.Sub:
						SubFilter.Decode(span, bytesPerPixel);
						goto IL_015e;
					case FilterType.Up:
						UpFilter.Decode(span, span2);
						goto IL_015e;
					case FilterType.Average:
						AverageFilter.Decode(span, span2, bytesPerPixel);
						goto IL_015e;
					case FilterType.Paeth:
						PaethFilter.Decode(span, span2, bytesPerPixel);
						goto IL_015e;
					default:
					{
						PngCrcChunkHandling pngCrcChunkHandling = this.pngCrcChunkHandling;
						if ((uint)(pngCrcChunkHandling - 2) > 1u)
						{
							PngThrowHelper.ThrowUnknownFilter();
							goto IL_015e;
						}
						break;
					}
					case FilterType.None:
						goto IL_015e;
					}
				}
				else
				{
					num2++;
					previousScanline.Clear();
					if (num2 < 7)
					{
						break;
					}
					num2 = 0;
				}
				memoryOwner?.Dispose();
				return;
				IL_015e:
				Span<TPixel> destination = pixelBuffer.DangerousGetRowSpan(num);
				ProcessInterlacedDefilteredScanline(in frameControl, scanline.GetSpan(), destination, pngMetadata, blendRowBuffer, Adam7.FirstColumn[num2], Adam7.ColumnIncrement[num2]);
				blendRowBuffer.Clear();
				SwapScanlineBuffers();
				num += Adam7.RowIncrement[num2];
			}
			num = Adam7.FirstRow[num2];
		}
	}

	private void ProcessDefilteredScanline<TPixel>(in FrameControl frameControl, int currentRow, ReadOnlySpan<byte> scanline, ImageFrame<TPixel> pixels, PngMetadata pngMetadata, Span<TPixel> blendRowBuffer) where TPixel : unmanaged, IPixel<TPixel>
	{
		Span<TPixel> span = pixels.PixelBuffer.DangerousGetRowSpan(currentRow);
		bool flag = frameControl.BlendOperation == PngBlendMethod.Over;
		Span<TPixel> span2 = (flag ? blendRowBuffer : span);
		ReadOnlySpan<byte> readOnlySpan = scanline.Slice(1, scanline.Length - 1);
		IMemoryOwner<byte> buffer = null;
		try
		{
			ReadOnlySpan<byte> scanlineSpan = (TryScaleUpTo8BitArray(readOnlySpan, bytesPerScanline - 1, header.BitDepth, out buffer) ? ((ReadOnlySpan<byte>)buffer.GetSpan()) : readOnlySpan);
			switch (pngColorType)
			{
			case PngColorType.Grayscale:
				PngScanlineProcessor.ProcessGrayscaleScanline(header.BitDepth, in frameControl, scanlineSpan, span2, pngMetadata.TransparentColor);
				break;
			case PngColorType.GrayscaleWithAlpha:
				PngScanlineProcessor.ProcessGrayscaleWithAlphaScanline(header.BitDepth, in frameControl, scanlineSpan, span2, (uint)bytesPerPixel, (uint)bytesPerSample);
				break;
			case PngColorType.Palette:
				PngScanlineProcessor.ProcessPaletteScanline(in frameControl, scanlineSpan, span2, pngMetadata.ColorTable);
				break;
			case PngColorType.Rgb:
				PngScanlineProcessor.ProcessRgbScanline(configuration, header.BitDepth, in frameControl, scanlineSpan, span2, bytesPerPixel, bytesPerSample, pngMetadata.TransparentColor);
				break;
			case PngColorType.RgbWithAlpha:
				PngScanlineProcessor.ProcessRgbaScanline(configuration, header.BitDepth, in frameControl, scanlineSpan, span2, bytesPerPixel, bytesPerSample);
				break;
			}
			if (flag)
			{
				PixelOperations<TPixel>.Instance.GetPixelBlender(PixelColorBlendingMode.Normal, PixelAlphaCompositionMode.SrcOver).Blend<TPixel>(configuration, span, span, span2, 1f);
			}
		}
		finally
		{
			buffer?.Dispose();
		}
	}

	private void ProcessInterlacedDefilteredScanline<TPixel>(in FrameControl frameControl, ReadOnlySpan<byte> scanline, Span<TPixel> destination, PngMetadata pngMetadata, Span<TPixel> blendRowBuffer, int pixelOffset = 0, int increment = 1) where TPixel : unmanaged, IPixel<TPixel>
	{
		bool flag = frameControl.BlendOperation == PngBlendMethod.Over;
		Span<TPixel> span = (flag ? blendRowBuffer : destination);
		ReadOnlySpan<byte> readOnlySpan = scanline.Slice(1, scanline.Length - 1);
		IMemoryOwner<byte> buffer = null;
		try
		{
			ReadOnlySpan<byte> scanlineSpan = (TryScaleUpTo8BitArray(readOnlySpan, bytesPerScanline, header.BitDepth, out buffer) ? ((ReadOnlySpan<byte>)buffer.GetSpan()) : readOnlySpan);
			switch (pngColorType)
			{
			case PngColorType.Grayscale:
				PngScanlineProcessor.ProcessInterlacedGrayscaleScanline(header.BitDepth, in frameControl, scanlineSpan, span, (uint)pixelOffset, (uint)increment, pngMetadata.TransparentColor);
				break;
			case PngColorType.GrayscaleWithAlpha:
				PngScanlineProcessor.ProcessInterlacedGrayscaleWithAlphaScanline(header.BitDepth, in frameControl, scanlineSpan, span, (uint)pixelOffset, (uint)increment, (uint)bytesPerPixel, (uint)bytesPerSample);
				break;
			case PngColorType.Palette:
				PngScanlineProcessor.ProcessInterlacedPaletteScanline(in frameControl, scanlineSpan, span, (uint)pixelOffset, (uint)increment, pngMetadata.ColorTable);
				break;
			case PngColorType.Rgb:
				PngScanlineProcessor.ProcessInterlacedRgbScanline(configuration, header.BitDepth, in frameControl, scanlineSpan, span, (uint)pixelOffset, (uint)increment, bytesPerPixel, bytesPerSample, pngMetadata.TransparentColor);
				break;
			case PngColorType.RgbWithAlpha:
				PngScanlineProcessor.ProcessInterlacedRgbaScanline(configuration, header.BitDepth, in frameControl, scanlineSpan, span, (uint)pixelOffset, (uint)increment, bytesPerPixel, bytesPerSample);
				break;
			}
			if (flag)
			{
				PixelOperations<TPixel>.Instance.GetPixelBlender(PixelColorBlendingMode.Normal, PixelAlphaCompositionMode.SrcOver).Blend<TPixel>(configuration, destination, destination, span, 1f);
			}
		}
		finally
		{
			buffer?.Dispose();
		}
	}

	private static void AssignColorPalette(ReadOnlySpan<byte> palette, ReadOnlySpan<byte> alpha, PngMetadata pngMetadata)
	{
		if (palette.Length == 0)
		{
			return;
		}
		Color[] array = new Color[palette.Length / Unsafe.SizeOf<Rgb24>()];
		ReadOnlySpan<Rgb24> readOnlySpan = MemoryMarshal.Cast<byte, Rgb24>(palette);
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new Color(readOnlySpan[i]);
		}
		if (alpha.Length > 0)
		{
			for (int j = 0; j < alpha.Length; j++)
			{
				ref Color reference = ref array[j];
				reference = reference.WithAlpha((float)(int)alpha[j] / 255f);
			}
		}
		pngMetadata.ColorTable = array;
	}

	private void AssignTransparentMarkers(ReadOnlySpan<byte> alpha, PngMetadata pngMetadata)
	{
		if (pngColorType == PngColorType.Rgb)
		{
			if (alpha.Length >= 6)
			{
				if (header.BitDepth == 16)
				{
					ushort r = BinaryPrimitives.ReadUInt16LittleEndian(alpha.Slice(0, 2));
					ushort g = BinaryPrimitives.ReadUInt16LittleEndian(alpha.Slice(2, 2));
					ushort b = BinaryPrimitives.ReadUInt16LittleEndian(alpha.Slice(4, 2));
					pngMetadata.TransparentColor = new Color(new Rgb48(r, g, b));
				}
				else
				{
					byte r2 = ReadByteLittleEndian(alpha, 0);
					byte g2 = ReadByteLittleEndian(alpha, 2);
					byte b2 = ReadByteLittleEndian(alpha, 4);
					pngMetadata.TransparentColor = new Color(new Rgb24(r2, g2, b2));
				}
			}
		}
		else if (pngColorType == PngColorType.Grayscale && alpha.Length >= 2)
		{
			if (header.BitDepth == 16)
			{
				pngMetadata.TransparentColor = Color.FromPixel(new L16(BinaryPrimitives.ReadUInt16LittleEndian(alpha.Slice(0, 2))));
			}
			else
			{
				pngMetadata.TransparentColor = Color.FromPixel(new L8(ReadByteLittleEndian(alpha, 0)));
			}
		}
	}

	private void ReadAnimationControlChunk(PngMetadata pngMetadata, ReadOnlySpan<byte> data)
	{
		animationControl = AnimationControl.Parse(data);
		pngMetadata.RepeatCount = animationControl.NumberPlays;
	}

	private FrameControl ReadFrameControlChunk(ReadOnlySpan<byte> data)
	{
		FrameControl result = FrameControl.Parse(data);
		result.Validate(header);
		return result;
	}

	private void ReadHeaderChunk(PngMetadata pngMetadata, ReadOnlySpan<byte> data)
	{
		header = PngHeader.Parse(data);
		header.Validate();
		pngMetadata.BitDepth = (PngBitDepth)header.BitDepth;
		pngMetadata.ColorType = header.ColorType;
		pngMetadata.InterlaceMethod = header.InterlaceMethod;
		pngColorType = header.ColorType;
	}

	private void ReadTextChunk(ImageMetadata baseMetadata, PngMetadata metadata, ReadOnlySpan<byte> data)
	{
		if (skipMetadata)
		{
			return;
		}
		int num = data.IndexOf<byte>(0);
		bool flag = ((num < 1 || num > 79) ? true : false);
		if (!flag && TryReadTextKeyword(data.Slice(0, num), out string name))
		{
			Encoding encoding = PngConstants.Encoding;
			int num2 = num + 1;
			string @string = encoding.GetString(data.Slice(num2, data.Length - num2));
			if (!TryReadTextChunkMetadata(baseMetadata, name, @string))
			{
				metadata.TextData.Add(new PngTextData(name, @string, string.Empty, string.Empty));
			}
		}
	}

	private void ReadCompressedTextChunk(ImageMetadata baseMetadata, PngMetadata metadata, ReadOnlySpan<byte> data)
	{
		if (skipMetadata)
		{
			return;
		}
		int num = data.IndexOf<byte>(0);
		bool flag = ((num < 1 || num > 79) ? true : false);
		if (!flag && data[num + 1] == 0 && TryReadTextKeyword(data.Slice(0, num), out string name))
		{
			int num2 = num + 2;
			ReadOnlySpan<byte> compressedData = data.Slice(num2, data.Length - num2);
			if (TryDecompressTextData(compressedData, PngConstants.Encoding, out string value) && !TryReadTextChunkMetadata(baseMetadata, name, value))
			{
				metadata.TextData.Add(new PngTextData(name, value, string.Empty, string.Empty));
			}
		}
	}

	private static bool TryReadTextChunkMetadata(ImageMetadata baseMetadata, string chunkName, string chunkText)
	{
		if (chunkName.Equals("Raw profile type exif", StringComparison.OrdinalIgnoreCase) && TryReadLegacyExifTextChunk(baseMetadata, chunkText))
		{
			return true;
		}
		return false;
	}

	private static void ReadCicpChunk(ImageMetadata metadata, ReadOnlySpan<byte> data)
	{
		if (data.Length >= 4)
		{
			byte colorPrimaries = data[0];
			byte transferCharacteristics = data[1];
			byte matrixCoefficients = data[2];
			bool? fullRange = ((data[3] == 1) ? new bool?(true) : ((data[3] == 0) ? new bool?(false) : ((bool?)null)));
			metadata.CicpProfile = new CicpProfile(colorPrimaries, transferCharacteristics, matrixCoefficients, fullRange);
		}
	}

	private static bool TryReadLegacyExifTextChunk(ImageMetadata metadata, string data)
	{
		ReadOnlySpan<char> readOnlySpan = data.AsSpan();
		readOnlySpan = readOnlySpan.TrimStart();
		if (!StringEqualsInsensitive(readOnlySpan.Slice(0, 4), "exif".AsSpan()))
		{
			return false;
		}
		ref ReadOnlySpan<char> reference = ref readOnlySpan;
		readOnlySpan = reference.Slice(4, reference.Length - 4).TrimStart();
		int num = readOnlySpan.IndexOf('\n');
		int num2 = ParseInt32(readOnlySpan.Slice(0, readOnlySpan.IndexOf('\n')));
		reference = ref readOnlySpan;
		int num3 = num;
		readOnlySpan = reference.Slice(num3, reference.Length - num3).Trim();
		ReadOnlySpan<byte> other = new byte[6] { 69, 120, 105, 102, 0, 0 };
		if (num2 < other.Length)
		{
			return false;
		}
		byte[] array = new byte[num2 - other.Length];
		try
		{
			byte[] array2 = array;
			if (array.Length < other.Length)
			{
				array2 = new byte[other.Length];
			}
			HexConverter.HexStringToBytes(readOnlySpan.Slice(0, other.Length * 2), array2);
			if (!array2.AsSpan().Slice(0, other.Length).SequenceEqual(other))
			{
				return false;
			}
			reference = ref readOnlySpan;
			num3 = other.Length * 2;
			readOnlySpan = reference.Slice(num3, reference.Length - num3);
			num2 -= other.Length;
			int num4 = 0;
			while (num4 < num2)
			{
				ReadOnlySpan<char> readOnlySpan2 = readOnlySpan;
				int num5 = readOnlySpan.IndexOf('\n');
				if (num5 != -1)
				{
					readOnlySpan2 = readOnlySpan.Slice(0, num5);
				}
				int num6 = num4;
				ReadOnlySpan<char> chars = readOnlySpan2;
				Span<byte> span = array.AsSpan();
				num3 = num4;
				num4 = num6 + HexConverter.HexStringToBytes(chars, span.Slice(num3, span.Length - num3));
				reference = ref readOnlySpan;
				num3 = num5 + 1;
				readOnlySpan = reference.Slice(num3, reference.Length - num3);
			}
		}
		catch
		{
			return false;
		}
		MergeOrSetExifProfile(metadata, new ExifProfile(array), replaceExistingKeys: false);
		return true;
	}

	private void ReadColorProfileChunk(ImageMetadata metadata, ReadOnlySpan<byte> data)
	{
		int num = data.IndexOf<byte>(0);
		bool flag = ((num < 1 || num > 79) ? true : false);
		if (!flag && data[num + 1] == 0 && TryReadTextKeyword(data.Slice(0, num), out string _))
		{
			int num2 = num + 2;
			ReadOnlySpan<byte> compressedData = data.Slice(num2, data.Length - num2);
			if (TryDecompressZlibData(compressedData, maxUncompressedLength, out byte[] uncompressedBytesArray))
			{
				metadata.IccProfile = new IccProfile(uncompressedBytesArray);
			}
		}
	}

	private unsafe bool TryDecompressZlibData(ReadOnlySpan<byte> compressedData, int maxLength, out byte[] uncompressedBytesArray)
	{
		fixed (byte* pointer = compressedData)
		{
			using IMemoryOwner<byte> buffer = memoryAllocator.Allocate<byte>(configuration.StreamProcessingBufferSize);
			using MemoryStream memoryStream = new MemoryStream(compressedData.Length);
			using UnmanagedMemoryStream stream = new UnmanagedMemoryStream(pointer, compressedData.Length);
			using BufferedReadStream innerStream = new BufferedReadStream(configuration, stream);
			using ZlibInflateStream zlibInflateStream = new ZlibInflateStream(innerStream);
			Span<byte> span = buffer.GetSpan();
			if (!zlibInflateStream.AllocateNewBytes(compressedData.Length, isCriticalChunk: false))
			{
				uncompressedBytesArray = Array.Empty<byte>();
				return false;
			}
			for (int num = zlibInflateStream.CompressedStream.Read(span, 0, span.Length); num != 0; num = zlibInflateStream.CompressedStream.Read(span, 0, span.Length))
			{
				if (memoryStream.Length > maxLength)
				{
					uncompressedBytesArray = Array.Empty<byte>();
					return false;
				}
				memoryStream.Write(span.Slice(0, num));
			}
			uncompressedBytesArray = memoryStream.ToArray();
			return true;
		}
	}

	private static bool StringEqualsInsensitive(ReadOnlySpan<char> span1, ReadOnlySpan<char> span2)
	{
		return MemoryExtensions.Equals(span1, span2, StringComparison.OrdinalIgnoreCase);
	}

	private static int ParseInt32(ReadOnlySpan<char> span)
	{
		return int.Parse(span, NumberStyles.Integer, CultureInfo.InvariantCulture);
	}

	private static void MergeOrSetExifProfile(ImageMetadata metadata, ExifProfile newProfile, bool replaceExistingKeys)
	{
		if (metadata.ExifProfile == null)
		{
			metadata.ExifProfile = newProfile;
			return;
		}
		foreach (IExifValue value in newProfile.Values)
		{
			if (replaceExistingKeys || metadata.ExifProfile.GetValueInternal(value.Tag) == null)
			{
				metadata.ExifProfile.SetValueInternal(value.Tag, value.GetValue());
			}
		}
	}

	private void ReadInternationalTextChunk(ImageMetadata metadata, ReadOnlySpan<byte> data)
	{
		if (skipMetadata)
		{
			return;
		}
		PngMetadata pngMetadata = metadata.GetPngMetadata();
		int num = data.IndexOf<byte>(0);
		if ((num < 1 || num > 79) ? true : false)
		{
			return;
		}
		byte b = data[num + 1];
		bool flag = (uint)b <= 1u;
		if (!flag || data[num + 2] != 0)
		{
			return;
		}
		int num2 = num + 3;
		ref ReadOnlySpan<byte> reference = ref data;
		int num3 = num2;
		int num4 = reference.Slice(num3, reference.Length - num3).IndexOf<byte>(0);
		if (num4 < 0)
		{
			return;
		}
		string @string = PngConstants.LanguageEncoding.GetString(data.Slice(num2, num4));
		int num5 = num2 + num4 + 1;
		reference = ref data;
		num3 = num5;
		int num6 = reference.Slice(num3, reference.Length - num3).IndexOf<byte>(0);
		string string2 = PngConstants.TranslatedEncoding.GetString(data.Slice(num5, num6));
		ReadOnlySpan<byte> keywordBytes = data.Slice(0, num);
		if (!TryReadTextKeyword(keywordBytes, out string name))
		{
			return;
		}
		int num7 = num5 + num6 + 1;
		if (b == 1)
		{
			reference = ref data;
			num3 = num7;
			ReadOnlySpan<byte> compressedData = reference.Slice(num3, reference.Length - num3);
			if (TryDecompressTextData(compressedData, PngConstants.TranslatedEncoding, out string value))
			{
				pngMetadata.TextData.Add(new PngTextData(name, value, @string, string2));
			}
		}
		else if (IsXmpTextData(keywordBytes))
		{
			reference = ref data;
			num3 = num7;
			metadata.XmpProfile = new XmpProfile(reference.Slice(num3, reference.Length - num3).ToArray());
		}
		else
		{
			Encoding translatedEncoding = PngConstants.TranslatedEncoding;
			reference = ref data;
			num3 = num7;
			string string3 = translatedEncoding.GetString(reference.Slice(num3, reference.Length - num3));
			pngMetadata.TextData.Add(new PngTextData(name, string3, @string, string2));
		}
	}

	private bool TryDecompressTextData(ReadOnlySpan<byte> compressedData, Encoding encoding, [NotNullWhen(true)] out string? value)
	{
		if (TryDecompressZlibData(compressedData, maxUncompressedLength, out byte[] uncompressedBytesArray))
		{
			value = encoding.GetString(uncompressedBytesArray);
			return true;
		}
		value = null;
		return false;
	}

	private int ReadNextDataChunk()
	{
		if (nextChunk.HasValue)
		{
			return 0;
		}
		Span<byte> buffer = stackalloc byte[20];
		if (currentStream.Read(buffer, 0, 4) == 0)
		{
			return 0;
		}
		if (TryReadChunk(buffer, out var chunk))
		{
			PngChunkType type = chunk.Type;
			if ((type == PngChunkType.Data || type == PngChunkType.FrameData) ? true : false)
			{
				chunk.Data?.Dispose();
				return chunk.Length;
			}
			nextChunk = chunk;
		}
		return 0;
	}

	private int ReadNextFrameDataChunk()
	{
		if (nextChunk.HasValue)
		{
			return 0;
		}
		Span<byte> buffer = stackalloc byte[20];
		if (currentStream.Read(buffer, 0, 4) == 0)
		{
			return 0;
		}
		if (TryReadChunk(buffer, out var chunk))
		{
			if (chunk.Type == PngChunkType.FrameData)
			{
				chunk.Data?.Dispose();
				currentStream.Position += 4L;
				return chunk.Length - 4;
			}
			nextChunk = chunk;
		}
		return 0;
	}

	private bool TryReadChunk(Span<byte> buffer, out PngChunk chunk)
	{
		if (nextChunk.HasValue)
		{
			chunk = nextChunk.Value;
			nextChunk = null;
			return true;
		}
		if (currentStream.Position >= currentStream.Length - 1)
		{
			chunk = default(PngChunk);
			return false;
		}
		if (!TryReadChunkLength(buffer, out var result))
		{
			chunk = default(PngChunk);
			return false;
		}
		while (result < 0)
		{
			if (!TryReadChunkLength(buffer, out result))
			{
				chunk = default(PngChunk);
				return false;
			}
		}
		PngChunkType pngChunkType = ReadChunkType(buffer);
		if (colorMetadataOnly && pngChunkType != PngChunkType.Header && pngChunkType != PngChunkType.Transparency && pngChunkType != PngChunkType.Palette && pngChunkType != PngChunkType.AnimationControl && pngChunkType != PngChunkType.FrameControl)
		{
			chunk = new PngChunk(result, pngChunkType);
			return true;
		}
		long position = currentStream.Position;
		chunk = new PngChunk((int)Math.Min(result, currentStream.Length - position), pngChunkType, ReadChunkData(result));
		ValidateChunk(in chunk, buffer);
		if ((pngChunkType == PngChunkType.Data || pngChunkType == PngChunkType.FrameData) ? true : false)
		{
			currentStream.Position = position;
		}
		return true;
	}

	private void ValidateChunk(in PngChunk chunk, Span<byte> buffer)
	{
		uint num = ReadChunkCrc(buffer);
		if (chunk.IsCritical(pngCrcChunkHandling))
		{
			Span<byte> span = stackalloc byte[4];
			BinaryPrimitives.WriteUInt32BigEndian(span, (uint)chunk.Type);
			if (Crc32.Calculate(Crc32.Calculate(span), chunk.Data.GetSpan()) != num)
			{
				string @string = Encoding.ASCII.GetString(span);
				chunk.Data?.Dispose();
				PngThrowHelper.ThrowInvalidChunkCrc(@string);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint ReadChunkCrc(Span<byte> buffer)
	{
		uint result = 0u;
		if (currentStream.Read(buffer, 0, 4) == 4)
		{
			result = BinaryPrimitives.ReadUInt32BigEndian(buffer);
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SkipChunkDataAndCrc(in PngChunk chunk)
	{
		currentStream.Skip(chunk.Length);
		currentStream.Skip(4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private IMemoryOwner<byte> ReadChunkData(int length)
	{
		if (length == 0)
		{
			return new BasicArrayBuffer<byte>(Array.Empty<byte>());
		}
		length = (int)Math.Min(length, currentStream.Length - currentStream.Position);
		IMemoryOwner<byte> memoryOwner = configuration.MemoryAllocator.Allocate<byte>(length, AllocationOptions.Clean);
		currentStream.Read(memoryOwner.GetSpan(), 0, length);
		return memoryOwner;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private PngChunkType ReadChunkType(Span<byte> buffer)
	{
		if (currentStream.Read(buffer, 0, 4) == 4)
		{
			return (PngChunkType)BinaryPrimitives.ReadUInt32BigEndian(buffer);
		}
		PngThrowHelper.ThrowInvalidChunkType();
		return (PngChunkType)0u;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool TryReadChunkLength(Span<byte> buffer, out int result)
	{
		if (currentStream.Read(buffer, 0, 4) == 4)
		{
			result = BinaryPrimitives.ReadInt32BigEndian(buffer);
			return true;
		}
		result = 0;
		return false;
	}

	private static bool TryReadTextKeyword(ReadOnlySpan<byte> keywordBytes, out string name)
	{
		name = string.Empty;
		ReadOnlySpan<byte> readOnlySpan = keywordBytes;
		int num = 0;
		while (num < readOnlySpan.Length)
		{
			byte b = readOnlySpan[num];
			if (b <= 126)
			{
				if (b >= 32)
				{
					goto IL_002b;
				}
			}
			else if (b >= 161)
			{
				goto IL_002b;
			}
			bool flag = false;
			goto IL_0031;
			IL_0031:
			if (!flag)
			{
				return false;
			}
			num++;
			continue;
			IL_002b:
			flag = true;
			goto IL_0031;
		}
		name = PngConstants.Encoding.GetString(keywordBytes);
		if (!string.IsNullOrWhiteSpace(name) && !name.StartsWith(" ", StringComparison.Ordinal))
		{
			return !name.EndsWith(" ", StringComparison.Ordinal);
		}
		return false;
	}

	private static bool IsXmpTextData(ReadOnlySpan<byte> keywordBytes)
	{
		return keywordBytes.SequenceEqual(PngConstants.XmpKeyword);
	}

	private void SwapScanlineBuffers()
	{
		IMemoryOwner<byte> memoryOwner = previousScanline;
		IMemoryOwner<byte> memoryOwner2 = scanline;
		scanline = memoryOwner;
		previousScanline = memoryOwner2;
	}
}
