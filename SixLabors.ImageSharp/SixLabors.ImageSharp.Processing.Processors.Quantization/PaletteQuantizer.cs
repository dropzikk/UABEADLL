using System;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

public class PaletteQuantizer : IQuantizer
{
	private readonly ReadOnlyMemory<Color> colorPalette;

	private readonly int transparentIndex;

	public QuantizerOptions Options { get; }

	public PaletteQuantizer(ReadOnlyMemory<Color> palette)
		: this(palette, new QuantizerOptions())
	{
	}

	public PaletteQuantizer(ReadOnlyMemory<Color> palette, QuantizerOptions options)
		: this(palette, options, -1)
	{
	}

	internal PaletteQuantizer(ReadOnlyMemory<Color> palette, QuantizerOptions options, int transparentIndex)
	{
		Guard.MustBeGreaterThan(palette.Length, 0, "palette");
		Guard.NotNull(options, "options");
		colorPalette = palette;
		Options = options;
		this.transparentIndex = transparentIndex;
	}

	public IQuantizer<TPixel> CreatePixelSpecificQuantizer<TPixel>(Configuration configuration) where TPixel : unmanaged, IPixel<TPixel>
	{
		return CreatePixelSpecificQuantizer<TPixel>(configuration, Options);
	}

	public IQuantizer<TPixel> CreatePixelSpecificQuantizer<TPixel>(Configuration configuration, QuantizerOptions options) where TPixel : unmanaged, IPixel<TPixel>
	{
		Guard.NotNull(options, "options");
		TPixel[] array = new TPixel[colorPalette.Length];
		Color.ToPixel(colorPalette.Span, array.AsSpan());
		return new PaletteQuantizer<TPixel>(configuration, options, array, transparentIndex);
	}
}
internal readonly struct PaletteQuantizer<TPixel> : IQuantizer<TPixel>, IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly EuclideanPixelMap<TPixel> pixelMap;

	public Configuration Configuration { get; }

	public QuantizerOptions Options { get; }

	public ReadOnlyMemory<TPixel> Palette => pixelMap.Palette;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PaletteQuantizer(Configuration configuration, QuantizerOptions options, ReadOnlyMemory<TPixel> palette, int transparentIndex)
	{
		Guard.NotNull(configuration, "configuration");
		Guard.NotNull(options, "options");
		Configuration = configuration;
		Options = options;
		pixelMap = new EuclideanPixelMap<TPixel>(configuration, palette, transparentIndex);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IndexedImageFrame<TPixel> QuantizeFrame(ImageFrame<TPixel> source, Rectangle bounds)
	{
		return QuantizerUtilities.QuantizeFrame(ref Unsafe.AsRef(in this), source, bounds);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddPaletteColors(Buffer2DRegion<TPixel> pixelRegion)
	{
	}

	public void SetTransparentIndex(int index)
	{
		pixelMap.SetTransparentIndex(index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public byte GetQuantizedColor(TPixel color, out TPixel match)
	{
		return (byte)pixelMap.GetClosestColor(color, out match);
	}

	public void Dispose()
	{
		pixelMap.Dispose();
	}
}
