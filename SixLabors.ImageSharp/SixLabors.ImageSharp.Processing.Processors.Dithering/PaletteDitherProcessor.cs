using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing.Processors.Quantization;

namespace SixLabors.ImageSharp.Processing.Processors.Dithering;

public sealed class PaletteDitherProcessor : IImageProcessor
{
	public IDither Dither { get; }

	public float DitherScale { get; }

	public ReadOnlyMemory<Color> Palette { get; }

	public PaletteDitherProcessor(IDither dither)
		: this(dither, 1f)
	{
	}

	public PaletteDitherProcessor(IDither dither, float ditherScale)
		: this(dither, ditherScale, Color.WebSafePalette)
	{
	}

	public PaletteDitherProcessor(IDither dither, ReadOnlyMemory<Color> palette)
		: this(dither, 1f, palette)
	{
	}

	public PaletteDitherProcessor(IDither dither, float ditherScale, ReadOnlyMemory<Color> palette)
	{
		Guard.MustBeGreaterThan(palette.Length, 0, "palette");
		Guard.NotNull(dither, "dither");
		Dither = dither;
		DitherScale = Numerics.Clamp(ditherScale, 0f, 1f);
		Palette = palette;
	}

	public IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new PaletteDitherProcessor<TPixel>(configuration, this, source, sourceRectangle);
	}
}
internal sealed class PaletteDitherProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	internal readonly struct DitherProcessor : IPaletteDitherImageProcessor<TPixel>, IDisposable
	{
		private readonly EuclideanPixelMap<TPixel> pixelMap;

		public Configuration Configuration { get; }

		public ReadOnlyMemory<TPixel> Palette { get; }

		public float DitherScale { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DitherProcessor(Configuration configuration, ReadOnlyMemory<TPixel> palette, float ditherScale)
		{
			Configuration = configuration;
			pixelMap = new EuclideanPixelMap<TPixel>(configuration, palette);
			Palette = palette;
			DitherScale = ditherScale;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TPixel GetPaletteColor(TPixel color)
		{
			pixelMap.GetClosestColor(color, out var match);
			return match;
		}

		public void Dispose()
		{
			pixelMap.Dispose();
		}
	}

	private readonly DitherProcessor ditherProcessor;

	private readonly IDither dither;

	private IMemoryOwner<TPixel>? paletteOwner;

	private bool isDisposed;

	public PaletteDitherProcessor(Configuration configuration, PaletteDitherProcessor definition, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		dither = definition.Dither;
		ReadOnlySpan<Color> span = definition.Palette.Span;
		paletteOwner = base.Configuration.MemoryAllocator.Allocate<TPixel>(span.Length);
		Color.ToPixel(span, paletteOwner.Memory.Span);
		ditherProcessor = new DitherProcessor(base.Configuration, paletteOwner.Memory, definition.DitherScale);
	}

	protected override void OnFrameApply(ImageFrame<TPixel> source)
	{
		Rectangle bounds = Rectangle.Intersect(base.SourceRectangle, source.Bounds());
		dither.ApplyPaletteDither(in ditherProcessor, source, bounds);
	}

	protected override void Dispose(bool disposing)
	{
		if (!isDisposed)
		{
			isDisposed = true;
			if (disposing)
			{
				paletteOwner?.Dispose();
				ditherProcessor.Dispose();
			}
			paletteOwner = null;
			base.Dispose(disposing);
		}
	}
}
