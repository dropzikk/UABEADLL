using System;
using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

public class OctreeQuantizer : IQuantizer
{
	public QuantizerOptions Options { get; }

	public OctreeQuantizer()
		: this(new QuantizerOptions())
	{
	}

	public OctreeQuantizer(QuantizerOptions options)
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
		return new OctreeQuantizer<TPixel>(configuration, options);
	}
}
public struct OctreeQuantizer<TPixel> : IQuantizer<TPixel>, IDisposable where TPixel : unmanaged, IPixel<TPixel>
{
	private sealed class Octree
	{
		public sealed class OctreeNode
		{
			private readonly OctreeNode?[]? children;

			private bool leaf;

			private int pixelCount;

			private int red;

			private int green;

			private int blue;

			private int paletteIndex;

			public OctreeNode? NextReducible
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get;
			}

			public OctreeNode(int level, int colorBits, Octree octree)
			{
				leaf = level == colorBits;
				red = (green = (blue = 0));
				pixelCount = 0;
				if (leaf)
				{
					octree.Leaves++;
					NextReducible = null;
					children = null;
				}
				else
				{
					NextReducible = octree.ReducibleNodes[level];
					octree.ReducibleNodes[level] = this;
					children = new OctreeNode[8];
				}
			}

			public void AddColor(ref Rgba32 color, int colorBits, int level, Octree octree)
			{
				if (leaf)
				{
					Increment(ref color);
					octree.TrackPrevious(this);
					return;
				}
				int colorIndex = GetColorIndex(ref color, level);
				OctreeNode octreeNode = children[colorIndex];
				if (octreeNode == null)
				{
					octreeNode = new OctreeNode(level + 1, colorBits, octree);
					children[colorIndex] = octreeNode;
				}
				octreeNode.AddColor(ref color, colorBits, level + 1, octree);
			}

			public int Reduce()
			{
				red = (green = (blue = 0));
				int num = 0;
				for (int i = 0; i < 8; i++)
				{
					OctreeNode octreeNode = children[i];
					if (octreeNode != null)
					{
						red += octreeNode.red;
						green += octreeNode.green;
						blue += octreeNode.blue;
						pixelCount += octreeNode.pixelCount;
						num++;
						children[i] = null;
					}
				}
				leaf = true;
				return num - 1;
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			public void ConstructPalette(Span<TPixel> palette, ref int index)
			{
				if (leaf)
				{
					Vector3 vector = Vector3.Clamp(new Vector3(red, green, blue) / pixelCount, Vector3.Zero, new Vector3(255f));
					Unsafe.SkipInit<TPixel>(out var value);
					value.FromRgba32(new Rgba32((byte)vector.X, (byte)vector.Y, (byte)vector.Z, byte.MaxValue));
					palette[index] = value;
					paletteIndex = index++;
				}
				else
				{
					for (int i = 0; i < 8; i++)
					{
						children[i]?.ConstructPalette(palette, ref index);
					}
				}
			}

			[MethodImpl(MethodImplOptions.NoInlining)]
			public int GetPaletteIndex(ref Rgba32 pixel, int level)
			{
				if (leaf)
				{
					return paletteIndex;
				}
				int colorIndex = GetColorIndex(ref pixel, level);
				OctreeNode octreeNode = children[colorIndex];
				int result = 0;
				if (octreeNode != null)
				{
					result = octreeNode.GetPaletteIndex(ref pixel, level + 1);
				}
				else
				{
					for (int i = 0; i < children.Length; i++)
					{
						octreeNode = children[i];
						if (octreeNode != null)
						{
							int num = octreeNode.GetPaletteIndex(ref pixel, level + 1);
							if (num != 0)
							{
								return num;
							}
						}
					}
				}
				return result;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static int GetColorIndex(ref Rgba32 color, int level)
			{
				int num = 7 - level;
				byte b = (byte)(1 << num);
				return ((color.R & b) >> num) | ((color.G & b) >> num << 1) | ((color.B & b) >> num << 2);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Increment(ref Rgba32 color)
			{
				pixelCount++;
				red += color.R;
				green += color.G;
				blue += color.B;
			}
		}

		private readonly OctreeNode root;

		private readonly int maxColorBits;

		private OctreeNode? previousNode;

		private Rgba32 previousColor;

		public int Leaves
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set;
		}

		private OctreeNode?[] ReducibleNodes
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
		}

		public Octree(int maxColorBits)
		{
			this.maxColorBits = maxColorBits;
			Leaves = 0;
			ReducibleNodes = new OctreeNode[9];
			root = new OctreeNode(0, this.maxColorBits, this);
			previousColor = default(Rgba32);
			previousNode = null;
		}

		public void AddColor(Rgba32 color)
		{
			if (previousColor.Equals(color))
			{
				if (previousNode == null)
				{
					previousColor = color;
					root.AddColor(ref color, maxColorBits, 0, this);
				}
				else
				{
					previousNode.Increment(ref color);
				}
			}
			else
			{
				previousColor = color;
				root.AddColor(ref color, maxColorBits, 0, this);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Palletize(Span<TPixel> palette, int colorCount, ref int paletteIndex)
		{
			while (Leaves > colorCount)
			{
				Reduce();
			}
			root.ConstructPalette(palette, ref paletteIndex);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetPaletteIndex(TPixel color)
		{
			Unsafe.SkipInit<Rgba32>(out var value);
			color.ToRgba32(ref value);
			return root.GetPaletteIndex(ref value, 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void TrackPrevious(OctreeNode node)
		{
			previousNode = node;
		}

		private void Reduce()
		{
			int num = maxColorBits - 1;
			while (num > 0 && ReducibleNodes[num] == null)
			{
				num--;
			}
			OctreeNode octreeNode = ReducibleNodes[num];
			ReducibleNodes[num] = octreeNode.NextReducible;
			Leaves -= octreeNode.Reduce();
			previousNode = null;
		}
	}

	private readonly int maxColors;

	private readonly int bitDepth;

	private readonly Octree octree;

	private readonly IMemoryOwner<TPixel> paletteOwner;

	private ReadOnlyMemory<TPixel> palette;

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
	public OctreeQuantizer(Configuration configuration, QuantizerOptions options)
	{
		Configuration = configuration;
		Options = options;
		maxColors = Options.MaxColors;
		bitDepth = Numerics.Clamp(ColorNumerics.GetBitsNeededForColorDepth(maxColors), 1, 8);
		octree = new Octree(bitDepth);
		paletteOwner = configuration.MemoryAllocator.Allocate<TPixel>(maxColors, AllocationOptions.Clean);
		pixelMap = null;
		palette = default(ReadOnlyMemory<TPixel>);
		isDithering = Options.Dither != null;
		isDisposed = false;
	}

	public void AddPaletteColors(Buffer2DRegion<TPixel> pixelRegion)
	{
		using (IMemoryOwner<Rgba32> buffer = Configuration.MemoryAllocator.Allocate<Rgba32>(pixelRegion.Width))
		{
			Span<Rgba32> span = buffer.GetSpan();
			for (int i = 0; i < pixelRegion.Height; i++)
			{
				Span<TPixel> span2 = pixelRegion.DangerousGetRowSpan(i);
				PixelOperations<TPixel>.Instance.ToRgba32(Configuration, span2, span);
				for (int j = 0; j < span.Length; j++)
				{
					Rgba32 color = span[j];
					octree.AddColor(color);
				}
			}
		}
		int paletteIndex = 0;
		Span<TPixel> span3 = paletteOwner.GetSpan();
		int num = maxColors;
		if (bitDepth >= 4)
		{
			num--;
		}
		octree.Palletize(span3, num, ref paletteIndex);
		ReadOnlyMemory<TPixel> readOnlyMemory = paletteOwner.Memory.Slice(0, span3.Length);
		if (pixelMap == null)
		{
			pixelMap = new EuclideanPixelMap<TPixel>(Configuration, readOnlyMemory);
		}
		else
		{
			pixelMap.Clear(readOnlyMemory);
		}
		palette = readOnlyMemory;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly IndexedImageFrame<TPixel> QuantizeFrame(ImageFrame<TPixel> source, Rectangle bounds)
	{
		return QuantizerUtilities.QuantizeFrame(ref Unsafe.AsRef(in this), source, bounds);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public readonly byte GetQuantizedColor(TPixel color, out TPixel match)
	{
		if (isDithering || color.Equals(default(TPixel)))
		{
			return (byte)pixelMap.GetClosestColor(color, out match);
		}
		ref TPixel reference = ref MemoryMarshal.GetReference(palette.Span);
		byte b = (byte)octree.GetPaletteIndex(color);
		match = Unsafe.Add(ref reference, (int)b);
		return b;
	}

	public void Dispose()
	{
		if (!isDisposed)
		{
			isDisposed = true;
			paletteOwner.Dispose();
			pixelMap?.Dispose();
			pixelMap = null;
		}
	}
}
