using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

public class WuQuantizer : IQuantizer
{
	public QuantizerOptions Options { get; }

	public WuQuantizer()
		: this(new QuantizerOptions())
	{
	}

	public WuQuantizer(QuantizerOptions options)
	{
		Guard.NotNull(options, "options");
		Options = options;
	}

	public IQuantizer<TPixel> CreatePixelSpecificQuantizer<TPixel>(Configuration configuration) where TPixel : unmanaged, IPixel<TPixel>
	{
		return CreatePixelSpecificQuantizer<TPixel>(configuration, Options);
	}

	public IQuantizer<TPixel> CreatePixelSpecificQuantizer<TPixel>(Configuration configuration, QuantizerOptions options) where TPixel : unmanaged, IPixel<TPixel>
	{
		return new WuQuantizer<TPixel>(configuration, options);
	}
}
internal struct WuQuantizer<TPixel> : IQuantizer<TPixel>, IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
	private struct Moment
	{
		public long R;

		public long G;

		public long B;

		public long A;

		public long Weight;

		public double Moment2;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Moment operator +(Moment x, Moment y)
		{
			x.R += y.R;
			x.G += y.G;
			x.B += y.B;
			x.A += y.A;
			x.Weight += y.Weight;
			x.Moment2 += y.Moment2;
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Moment operator -(Moment x, Moment y)
		{
			x.R -= y.R;
			x.G -= y.G;
			x.B -= y.B;
			x.A -= y.A;
			x.Weight -= y.Weight;
			x.Moment2 -= y.Moment2;
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Moment operator -(Moment x)
		{
			x.R = -x.R;
			x.G = -x.G;
			x.B = -x.B;
			x.A = -x.A;
			x.Weight = -x.Weight;
			x.Moment2 = 0.0 - x.Moment2;
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Moment operator +(Moment x, Rgba32 y)
		{
			x.R += y.R;
			x.G += y.G;
			x.B += y.B;
			x.A += y.A;
			x.Weight++;
			Vector4 vector = new Vector4((int)y.R, (int)y.G, (int)y.B, (int)y.A);
			x.Moment2 += Vector4.Dot(vector, vector);
			return x;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public readonly Vector4 Normalize()
		{
			return new Vector4(R, G, B, A) / Weight / 255f;
		}
	}

	private struct Box : IEquatable<Box>
	{
		public int RMin;

		public int RMax;

		public int GMin;

		public int GMax;

		public int BMin;

		public int BMax;

		public int AMin;

		public int AMax;

		public int Volume;

		public override readonly bool Equals(object? obj)
		{
			if (obj is Box other)
			{
				return Equals(other);
			}
			return false;
		}

		public readonly bool Equals(Box other)
		{
			if (RMin == other.RMin && RMax == other.RMax && GMin == other.GMin && GMax == other.GMax && BMin == other.BMin && BMax == other.BMax && AMin == other.AMin && AMax == other.AMax)
			{
				return Volume == other.Volume;
			}
			return false;
		}

		public override readonly int GetHashCode()
		{
			HashCode hashCode = default(HashCode);
			hashCode.Add(RMin);
			hashCode.Add(RMax);
			hashCode.Add(GMin);
			hashCode.Add(GMax);
			hashCode.Add(BMin);
			hashCode.Add(BMax);
			hashCode.Add(AMin);
			hashCode.Add(AMax);
			hashCode.Add(Volume);
			return hashCode.ToHashCode();
		}
	}

	private readonly MemoryAllocator memoryAllocator;

	private const int IndexBits = 5;

	private const int IndexAlphaBits = 5;

	private const int IndexCount = 33;

	private const int IndexAlphaCount = 33;

	private const int TableLength = 1185921;

	private readonly IMemoryOwner<Moment> momentsOwner;

	private readonly IMemoryOwner<byte> tagsOwner;

	private readonly IMemoryOwner<TPixel> paletteOwner;

	private ReadOnlyMemory<TPixel> palette;

	private int maxColors;

	private readonly Box[] colorCube;

	private EuclideanPixelMap<TPixel>? pixelMap;

	private readonly bool isDithering;

	private bool isDisposed;

	public Configuration Configuration { get; }

	public QuantizerOptions Options { get; }

	public readonly ReadOnlyMemory<TPixel> Palette
	{
		get
		{
			QuantizerUtilities.CheckPaletteState(in palette);
			return palette;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public WuQuantizer(Configuration configuration, QuantizerOptions options)
	{
		Guard.NotNull(configuration, "configuration");
		Guard.NotNull(options, "options");
		Configuration = configuration;
		Options = options;
		maxColors = Options.MaxColors;
		memoryAllocator = Configuration.MemoryAllocator;
		momentsOwner = memoryAllocator.Allocate<Moment>(1185921, AllocationOptions.Clean);
		tagsOwner = memoryAllocator.Allocate<byte>(1185921, AllocationOptions.Clean);
		paletteOwner = memoryAllocator.Allocate<TPixel>(maxColors, AllocationOptions.Clean);
		colorCube = new Box[maxColors];
		isDisposed = false;
		pixelMap = null;
		palette = default(ReadOnlyMemory<TPixel>);
		isDithering = (isDithering = Options.Dither != null);
	}

	public void AddPaletteColors(Buffer2DRegion<TPixel> pixelRegion)
	{
		Build3DHistogram(pixelRegion);
		Get3DMoments(memoryAllocator);
		BuildCube();
		Span<TPixel> span = paletteOwner.GetSpan().Slice(0, maxColors);
		ReadOnlySpan<Moment> moments = momentsOwner.GetSpan();
		for (int i = 0; i < span.Length; i++)
		{
			Mark(ref colorCube[i], (byte)i);
			Moment moment = Volume(ref colorCube[i], moments);
			if (moment.Weight > 0)
			{
				span[i].FromScaledVector4(moment.Normalize());
			}
		}
		ReadOnlyMemory<TPixel> readOnlyMemory = paletteOwner.Memory.Slice(0, span.Length);
		if (isDithering)
		{
			if (pixelMap == null)
			{
				pixelMap = new EuclideanPixelMap<TPixel>(Configuration, readOnlyMemory);
			}
			else
			{
				pixelMap.Clear(readOnlyMemory);
			}
		}
		palette = readOnlyMemory;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly IndexedImageFrame<TPixel> QuantizeFrame(ImageFrame<TPixel> source, Rectangle bounds)
	{
		return QuantizerUtilities.QuantizeFrame(ref Unsafe.AsRef(in this), source, bounds);
	}

	public readonly byte GetQuantizedColor(TPixel color, out TPixel match)
	{
		if (isDithering)
		{
			return (byte)pixelMap.GetClosestColor(color, out match);
		}
		Rgba32 dest = default(Rgba32);
		color.ToRgba32(ref dest);
		int num = dest.R >> 3;
		int num2 = dest.G >> 3;
		int num3 = dest.B >> 3;
		int num4 = dest.A >> 3;
		byte b = ((ReadOnlySpan<byte>)tagsOwner.GetSpan())[GetPaletteIndex(num + 1, num2 + 1, num3 + 1, num4 + 1)];
		match = Unsafe.Add(ref MemoryMarshal.GetReference(palette.Span), (int)b);
		return b;
	}

	public void Dispose()
	{
		if (!isDisposed)
		{
			isDisposed = true;
			momentsOwner?.Dispose();
			tagsOwner?.Dispose();
			paletteOwner?.Dispose();
			pixelMap?.Dispose();
			pixelMap = null;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetPaletteIndex(int r, int g, int b, int a)
	{
		return (r << 15) + (r << 11) + (g << 10) + (r << 10) + (r << 6) + (g << 5) + (r + g + b << 5) + r + g + b + a;
	}

	private static Moment Volume(ref Box cube, ReadOnlySpan<Moment> moments)
	{
		return moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMax, cube.AMax)] - moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMax, cube.AMin)] - moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMin, cube.AMax)] + moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMin, cube.AMin)] - moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMax, cube.AMax)] + moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMax, cube.AMin)] + moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMax)] - moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMin)] - moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMax, cube.AMax)] + moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMax, cube.AMin)] + moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMax)] - moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMin)] + moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMax)] - moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMin)] - moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMax)] + moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMin)];
	}

	private static Moment Bottom(ref Box cube, int direction, ReadOnlySpan<Moment> moments)
	{
		return direction switch
		{
			3 => -moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMax, cube.AMax)] + moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMax, cube.AMin)] + moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMax)] - moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMin)] + moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMax)] - moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMin)] - moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMax)] + moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMin)], 
			2 => -moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMax, cube.AMax)] + moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMax, cube.AMin)] + moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMax)] - moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMin)] + moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMax)] - moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMin)] - moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMax)] + moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMin)], 
			1 => -moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMin, cube.AMax)] + moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMin, cube.AMin)] + moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMax)] - moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMin)] + moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMax)] - moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMin)] - moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMax)] + moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMin)], 
			0 => -moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMax, cube.AMin)] + moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMin, cube.AMin)] + moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMax, cube.AMin)] - moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMin)] + moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMax, cube.AMin)] - moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMin)] - moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMin)] + moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMin)], 
			_ => throw new ArgumentOutOfRangeException("direction"), 
		};
	}

	private static Moment Top(ref Box cube, int direction, int position, ReadOnlySpan<Moment> moments)
	{
		return direction switch
		{
			3 => moments[GetPaletteIndex(position, cube.GMax, cube.BMax, cube.AMax)] - moments[GetPaletteIndex(position, cube.GMax, cube.BMax, cube.AMin)] - moments[GetPaletteIndex(position, cube.GMax, cube.BMin, cube.AMax)] + moments[GetPaletteIndex(position, cube.GMax, cube.BMin, cube.AMin)] - moments[GetPaletteIndex(position, cube.GMin, cube.BMax, cube.AMax)] + moments[GetPaletteIndex(position, cube.GMin, cube.BMax, cube.AMin)] + moments[GetPaletteIndex(position, cube.GMin, cube.BMin, cube.AMax)] - moments[GetPaletteIndex(position, cube.GMin, cube.BMin, cube.AMin)], 
			2 => moments[GetPaletteIndex(cube.RMax, position, cube.BMax, cube.AMax)] - moments[GetPaletteIndex(cube.RMax, position, cube.BMax, cube.AMin)] - moments[GetPaletteIndex(cube.RMax, position, cube.BMin, cube.AMax)] + moments[GetPaletteIndex(cube.RMax, position, cube.BMin, cube.AMin)] - moments[GetPaletteIndex(cube.RMin, position, cube.BMax, cube.AMax)] + moments[GetPaletteIndex(cube.RMin, position, cube.BMax, cube.AMin)] + moments[GetPaletteIndex(cube.RMin, position, cube.BMin, cube.AMax)] - moments[GetPaletteIndex(cube.RMin, position, cube.BMin, cube.AMin)], 
			1 => moments[GetPaletteIndex(cube.RMax, cube.GMax, position, cube.AMax)] - moments[GetPaletteIndex(cube.RMax, cube.GMax, position, cube.AMin)] - moments[GetPaletteIndex(cube.RMax, cube.GMin, position, cube.AMax)] + moments[GetPaletteIndex(cube.RMax, cube.GMin, position, cube.AMin)] - moments[GetPaletteIndex(cube.RMin, cube.GMax, position, cube.AMax)] + moments[GetPaletteIndex(cube.RMin, cube.GMax, position, cube.AMin)] + moments[GetPaletteIndex(cube.RMin, cube.GMin, position, cube.AMax)] - moments[GetPaletteIndex(cube.RMin, cube.GMin, position, cube.AMin)], 
			0 => moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMax, position)] - moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMin, position)] - moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMax, position)] + moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, position)] - moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMax, position)] + moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, position)] + moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, position)] - moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, position)], 
			_ => throw new ArgumentOutOfRangeException("direction"), 
		};
	}

	private readonly void Build3DHistogram(Buffer2DRegion<TPixel> source)
	{
		Span<Moment> span = momentsOwner.GetSpan();
		using IMemoryOwner<Rgba32> buffer = memoryAllocator.Allocate<Rgba32>(source.Width);
		Span<Rgba32> span2 = buffer.GetSpan();
		for (int i = 0; i < source.Height; i++)
		{
			Span<TPixel> span3 = source.DangerousGetRowSpan(i);
			PixelOperations<TPixel>.Instance.ToRgba32(Configuration, span3, span2);
			for (int j = 0; j < span2.Length; j++)
			{
				Rgba32 rgba = span2[j];
				int r = (rgba.R >> 3) + 1;
				int g = (rgba.G >> 3) + 1;
				int b = (rgba.B >> 3) + 1;
				int a = (rgba.A >> 3) + 1;
				span[GetPaletteIndex(r, g, b, a)] += rgba;
			}
		}
	}

	private readonly void Get3DMoments(MemoryAllocator allocator)
	{
		using IMemoryOwner<Moment> buffer = allocator.Allocate<Moment>(1089);
		using IMemoryOwner<Moment> buffer2 = allocator.Allocate<Moment>(33);
		Span<Moment> span = momentsOwner.GetSpan();
		Span<Moment> span2 = buffer.GetSpan();
		Span<Moment> span3 = buffer2.GetSpan();
		int paletteIndex = GetPaletteIndex(1, 0, 0, 0);
		for (int i = 1; i < 33; i++)
		{
			int num = (i << 15) + (i << 11) + (i << 10) + (i << 6) + i;
			span2.Clear();
			for (int j = 1; j < 33; j++)
			{
				int num2 = num + (j << 10) + (j << 5) + j;
				int num3 = i + j;
				span3.Clear();
				for (int k = 1; k < 33; k++)
				{
					int num4 = num2 + (num3 + k << 5) + k;
					Moment moment = default(Moment);
					int num5 = k * 33;
					for (int l = 1; l < 33; l++)
					{
						int num6 = num4 + l;
						moment += span[num6];
						span3[l] += moment;
						int index = num5 + l;
						span2[index] += span3[l];
						int index2 = num6 - paletteIndex;
						span[num6] = span[index2] + span2[index];
					}
				}
			}
		}
	}

	private readonly double Variance(ref Box cube)
	{
		ReadOnlySpan<Moment> moments = momentsOwner.GetSpan();
		Moment moment = Volume(ref cube, moments);
		Moment moment2 = moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMax, cube.AMax)] - moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMax, cube.AMin)] - moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMin, cube.AMax)] + moments[GetPaletteIndex(cube.RMax, cube.GMax, cube.BMin, cube.AMin)] - moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMax, cube.AMax)] + moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMax, cube.AMin)] + moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMax)] - moments[GetPaletteIndex(cube.RMax, cube.GMin, cube.BMin, cube.AMin)] - moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMax, cube.AMax)] + moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMax, cube.AMin)] + moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMax)] - moments[GetPaletteIndex(cube.RMin, cube.GMax, cube.BMin, cube.AMin)] + moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMax)] - moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMax, cube.AMin)] - moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMax)] + moments[GetPaletteIndex(cube.RMin, cube.GMin, cube.BMin, cube.AMin)];
		Vector4 vector = new Vector4(moment.R, moment.G, moment.B, moment.A);
		return moment2.Moment2 - (double)(Vector4.Dot(vector, vector) / (float)moment.Weight);
	}

	private readonly float Maximize(ref Box cube, int direction, int first, int last, out int cut, Moment whole)
	{
		ReadOnlySpan<Moment> moments = momentsOwner.GetSpan();
		Moment moment = Bottom(ref cube, direction, moments);
		float num = 0f;
		cut = -1;
		for (int i = first; i < last; i++)
		{
			Moment moment2 = moment + Top(ref cube, direction, i, moments);
			if (moment2.Weight == 0L)
			{
				continue;
			}
			Vector4 vector = new Vector4(moment2.R, moment2.G, moment2.B, moment2.A);
			float num2 = Vector4.Dot(vector, vector) / (float)moment2.Weight;
			moment2 = whole - moment2;
			if (moment2.Weight != 0L)
			{
				vector = new Vector4(moment2.R, moment2.G, moment2.B, moment2.A);
				num2 += Vector4.Dot(vector, vector) / (float)moment2.Weight;
				if (num2 > num)
				{
					num = num2;
					cut = i;
				}
			}
		}
		return num;
	}

	private bool Cut(ref Box set1, ref Box set2)
	{
		ReadOnlySpan<Moment> moments = momentsOwner.GetSpan();
		Moment whole = Volume(ref set1, moments);
		int cut;
		float num = Maximize(ref set1, 3, set1.RMin + 1, set1.RMax, out cut, whole);
		int cut2;
		float num2 = Maximize(ref set1, 2, set1.GMin + 1, set1.GMax, out cut2, whole);
		int cut3;
		float num3 = Maximize(ref set1, 1, set1.BMin + 1, set1.BMax, out cut3, whole);
		int cut4;
		float num4 = Maximize(ref set1, 0, set1.AMin + 1, set1.AMax, out cut4, whole);
		int num5;
		if (!(num >= num2) || !(num >= num3) || !(num >= num4))
		{
			num5 = ((num2 >= num && num2 >= num3 && num2 >= num4) ? 2 : ((num3 >= num && num3 >= num2 && num3 >= num4) ? 1 : 0));
		}
		else
		{
			num5 = 3;
			if (cut < 0)
			{
				return false;
			}
		}
		set2.RMax = set1.RMax;
		set2.GMax = set1.GMax;
		set2.BMax = set1.BMax;
		set2.AMax = set1.AMax;
		switch (num5)
		{
		case 3:
			set2.RMin = (set1.RMax = cut);
			set2.GMin = set1.GMin;
			set2.BMin = set1.BMin;
			set2.AMin = set1.AMin;
			break;
		case 2:
			set2.GMin = (set1.GMax = cut2);
			set2.RMin = set1.RMin;
			set2.BMin = set1.BMin;
			set2.AMin = set1.AMin;
			break;
		case 1:
			set2.BMin = (set1.BMax = cut3);
			set2.RMin = set1.RMin;
			set2.GMin = set1.GMin;
			set2.AMin = set1.AMin;
			break;
		case 0:
			set2.AMin = (set1.AMax = cut4);
			set2.RMin = set1.RMin;
			set2.GMin = set1.GMin;
			set2.BMin = set1.BMin;
			break;
		}
		set1.Volume = (set1.RMax - set1.RMin) * (set1.GMax - set1.GMin) * (set1.BMax - set1.BMin) * (set1.AMax - set1.AMin);
		set2.Volume = (set2.RMax - set2.RMin) * (set2.GMax - set2.GMin) * (set2.BMax - set2.BMin) * (set2.AMax - set2.AMin);
		return true;
	}

	private readonly void Mark(ref Box cube, byte label)
	{
		Span<byte> span = tagsOwner.GetSpan();
		for (int i = cube.RMin + 1; i <= cube.RMax; i++)
		{
			int num = (i << 15) + (i << 11) + (i << 10) + (i << 6) + i;
			for (int j = cube.GMin + 1; j <= cube.GMax; j++)
			{
				int num2 = num + (j << 10) + (j << 5) + j;
				int num3 = i + j;
				for (int k = cube.BMin + 1; k <= cube.BMax; k++)
				{
					int num4 = num2 + (num3 + k << 5) + k;
					for (int l = cube.AMin + 1; l <= cube.AMax; l++)
					{
						int index = num4 + l;
						span[index] = label;
					}
				}
			}
		}
	}

	private void BuildCube()
	{
		using IMemoryOwner<double> buffer = Configuration.MemoryAllocator.Allocate<double>(maxColors);
		Span<double> span = buffer.GetSpan();
		ref Box arrayDataReference = ref MemoryMarshal.GetArrayDataReference(colorCube);
		arrayDataReference.RMin = (arrayDataReference.GMin = (arrayDataReference.BMin = (arrayDataReference.AMin = 0)));
		arrayDataReference.RMax = (arrayDataReference.GMax = (arrayDataReference.BMax = 32));
		arrayDataReference.AMax = 32;
		int num = 0;
		for (int i = 1; i < maxColors; i++)
		{
			ref Box reference = ref colorCube[num];
			ref Box reference2 = ref colorCube[i];
			if (Cut(ref reference, ref reference2))
			{
				span[num] = ((reference.Volume > 1) ? Variance(ref reference) : 0.0);
				span[i] = ((reference2.Volume > 1) ? Variance(ref reference2) : 0.0);
			}
			else
			{
				span[num] = 0.0;
				i--;
			}
			num = 0;
			double num2 = span[0];
			for (int j = 1; j <= i; j++)
			{
				if (span[j] > num2)
				{
					num2 = span[j];
					num = j;
				}
			}
			if (num2 <= 0.0)
			{
				maxColors = i + 1;
				break;
			}
		}
	}
}
