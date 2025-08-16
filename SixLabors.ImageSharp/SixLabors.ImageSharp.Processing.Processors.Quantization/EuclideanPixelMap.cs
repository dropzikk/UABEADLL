using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

internal sealed class EuclideanPixelMap<TPixel> : IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
	private struct ColorDistanceCache : IDisposable
	{
		private const int IndexRBits = 5;

		private const int IndexGBits = 5;

		private const int IndexBBits = 5;

		private const int IndexABits = 6;

		private const int IndexRCount = 33;

		private const int IndexGCount = 33;

		private const int IndexBCount = 33;

		private const int IndexACount = 65;

		private const int RShift = 3;

		private const int GShift = 3;

		private const int BShift = 3;

		private const int AShift = 2;

		private const int Entries = 2335905;

		private MemoryHandle tableHandle;

		private readonly IMemoryOwner<short> table;

		private unsafe readonly short* tablePointer;

		public unsafe ColorDistanceCache(MemoryAllocator allocator)
		{
			table = allocator.Allocate<short>(2335905);
			table.GetSpan().Fill(-1);
			tableHandle = table.Memory.Pin();
			tablePointer = (short*)tableHandle.Pointer;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe readonly void Add(Rgba32 rgba, byte index)
		{
			int paletteIndex = GetPaletteIndex(rgba);
			tablePointer[paletteIndex] = index;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe readonly bool TryGetValue(Rgba32 rgba, out short match)
		{
			int paletteIndex = GetPaletteIndex(rgba);
			match = tablePointer[paletteIndex];
			return match > -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly void Clear()
		{
			table.GetSpan().Fill(-1);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetPaletteIndex(Rgba32 rgba)
		{
			int num = rgba.R >> 3;
			int num2 = rgba.G >> 3;
			int num3 = rgba.B >> 3;
			return (rgba.A >> 2) * 35937 + num * 1089 + num2 * 33 + num3;
		}

		public void Dispose()
		{
			if (table != null)
			{
				tableHandle.Dispose();
				table.Dispose();
			}
		}
	}

	private Rgba32[] rgbaPalette;

	private int transparentIndex;

	private readonly TPixel transparentMatch;

	private ColorDistanceCache cache;

	private readonly Configuration configuration;

	public ReadOnlyMemory<TPixel> Palette { get; private set; }

	public EuclideanPixelMap(Configuration configuration, ReadOnlyMemory<TPixel> palette)
		: this(configuration, palette, -1)
	{
	}

	public EuclideanPixelMap(Configuration configuration, ReadOnlyMemory<TPixel> palette, int transparentIndex = -1)
	{
		this.configuration = configuration;
		Palette = palette;
		rgbaPalette = new Rgba32[palette.Length];
		cache = new ColorDistanceCache(configuration.MemoryAllocator);
		PixelOperations<TPixel>.Instance.ToRgba32(configuration, Palette.Span, rgbaPalette);
		this.transparentIndex = transparentIndex;
		Unsafe.SkipInit<TPixel>(out transparentMatch);
		transparentMatch.FromRgba32(default(Rgba32));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetClosestColor(TPixel color, out TPixel match)
	{
		ref TPixel reference = ref MemoryMarshal.GetReference(Palette.Span);
		Unsafe.SkipInit<Rgba32>(out var value);
		color.ToRgba32(ref value);
		if (!cache.TryGetValue(value, out var match2))
		{
			return GetClosestColorSlow(value, ref reference, out match);
		}
		match = Unsafe.Add(ref reference, (int)(ushort)match2);
		return match2;
	}

	public void Clear(ReadOnlyMemory<TPixel> palette)
	{
		Palette = palette;
		rgbaPalette = new Rgba32[palette.Length];
		PixelOperations<TPixel>.Instance.ToRgba32(configuration, Palette.Span, rgbaPalette);
		transparentIndex = -1;
		cache.Clear();
	}

	public void SetTransparentIndex(int index)
	{
		if (index != transparentIndex)
		{
			cache.Clear();
		}
		transparentIndex = index;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int GetClosestColorSlow(Rgba32 rgba, ref TPixel paletteRef, out TPixel match)
	{
		int num = 0;
		if (transparentIndex >= 0 && rgba == default(Rgba32))
		{
			num = transparentIndex;
			cache.Add(rgba, (byte)num);
			match = transparentMatch;
			return num;
		}
		float num2 = float.MaxValue;
		for (int i = 0; i < rgbaPalette.Length; i++)
		{
			Rgba32 b = rgbaPalette[i];
			float num3 = DistanceSquared(rgba, b);
			if (num3 == 0f)
			{
				num = i;
				break;
			}
			if (num3 < num2)
			{
				num = i;
				num2 = num3;
			}
		}
		cache.Add(rgba, (byte)num);
		match = Unsafe.Add(ref paletteRef, (uint)num);
		return num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float DistanceSquared(Rgba32 a, Rgba32 b)
	{
		float num = a.R - b.R;
		float num2 = a.G - b.G;
		float num3 = a.B - b.B;
		float num4 = a.A - b.A;
		return num * num + num2 * num2 + num3 * num3 + num4 * num4;
	}

	public void Dispose()
	{
		cache.Dispose();
	}
}
