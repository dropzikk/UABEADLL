using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Processing.Processors.Normalization;

public abstract class HistogramEqualizationProcessor : IImageProcessor
{
	public int LuminanceLevels { get; }

	public bool ClipHistogram { get; }

	public int ClipLimit { get; }

	protected HistogramEqualizationProcessor(int luminanceLevels, bool clipHistogram, int clipLimit)
	{
		LuminanceLevels = luminanceLevels;
		ClipHistogram = clipHistogram;
		ClipLimit = clipLimit;
	}

	public abstract IImageProcessor<TPixel> CreatePixelSpecificProcessor<TPixel>(Configuration configuration, Image<TPixel> source, Rectangle sourceRectangle) where TPixel : unmanaged, IPixel<TPixel>;

	public static HistogramEqualizationProcessor FromOptions(HistogramEqualizationOptions options)
	{
		return options.Method switch
		{
			HistogramEqualizationMethod.Global => new GlobalHistogramEqualizationProcessor(options.LuminanceLevels, options.ClipHistogram, options.ClipLimit), 
			HistogramEqualizationMethod.AdaptiveTileInterpolation => new AdaptiveHistogramEqualizationProcessor(options.LuminanceLevels, options.ClipHistogram, options.ClipLimit, options.NumberOfTiles), 
			HistogramEqualizationMethod.AdaptiveSlidingWindow => new AdaptiveHistogramEqualizationSlidingWindowProcessor(options.LuminanceLevels, options.ClipHistogram, options.ClipLimit, options.NumberOfTiles), 
			HistogramEqualizationMethod.AutoLevel => new AutoLevelProcessor(options.LuminanceLevels, options.ClipHistogram, options.ClipLimit, options.SyncChannels), 
			_ => new GlobalHistogramEqualizationProcessor(options.LuminanceLevels, options.ClipHistogram, options.ClipLimit), 
		};
	}
}
internal abstract class HistogramEqualizationProcessor<TPixel> : ImageProcessor<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{
	private readonly float luminanceLevelsFloat;

	public int LuminanceLevels { get; }

	public bool ClipHistogramEnabled { get; }

	public int ClipLimit { get; }

	protected HistogramEqualizationProcessor(Configuration configuration, int luminanceLevels, bool clipHistogram, int clipLimit, Image<TPixel> source, Rectangle sourceRectangle)
		: base(configuration, source, sourceRectangle)
	{
		Guard.MustBeGreaterThan(luminanceLevels, 0, "luminanceLevels");
		Guard.MustBeGreaterThan(clipLimit, 1, "clipLimit");
		LuminanceLevels = luminanceLevels;
		luminanceLevelsFloat = luminanceLevels;
		ClipHistogramEnabled = clipHistogram;
		ClipLimit = clipLimit;
	}

	public static int CalculateCdf(ref int cdfBase, ref int histogramBase, int maxIdx)
	{
		int num = 0;
		int num2 = 0;
		bool flag = false;
		for (nuint num3 = 0u; num3 <= (uint)maxIdx; num3++)
		{
			num += Unsafe.Add(ref histogramBase, num3);
			if (!flag && num != 0)
			{
				num2 = num;
				flag = true;
			}
			Unsafe.Add(ref cdfBase, num3) = Math.Max(0, num - num2);
		}
		return num2;
	}

	public void ClipHistogram(Span<int> histogram, int clipLimit)
	{
		int num = 0;
		ref int reference = ref MemoryMarshal.GetReference(histogram);
		for (nuint num2 = 0u; num2 < (uint)histogram.Length; num2++)
		{
			ref int reference2 = ref Unsafe.Add(ref reference, num2);
			if (reference2 > clipLimit)
			{
				num += reference2 - clipLimit;
				reference2 = clipLimit;
			}
		}
		int num3 = ((num > 0) ? ((int)MathF.Floor((float)num / luminanceLevelsFloat)) : 0);
		if (num3 > 0)
		{
			for (nuint num4 = 0u; num4 < (uint)histogram.Length; num4++)
			{
				Unsafe.Add(ref reference, num4) += num3;
			}
		}
		int num5 = num - num3 * LuminanceLevels;
		if (num5 != 0)
		{
			uint num6 = (uint)Math.Max(LuminanceLevels / num5, 1);
			nuint num7 = 0u;
			while (num7 < (uint)LuminanceLevels && num5 > 0)
			{
				Unsafe.Add(ref reference, num7)++;
				num7 += num6;
				num5--;
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetLuminance(TPixel sourcePixel, int luminanceLevels)
	{
		Vector4 vector = sourcePixel.ToVector4();
		return ColorNumerics.GetBT709Luminance(ref vector, luminanceLevels);
	}
}
