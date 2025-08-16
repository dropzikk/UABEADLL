using System;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Tiff.PhotometricInterpretation;

internal class YCbCrConverter
{
	private readonly struct CodingRangeExpander
	{
		private readonly float f1;

		private readonly float f2;

		public CodingRangeExpander(Rational referenceBlack, Rational referenceWhite, int codingRange)
		{
			float num = referenceBlack.ToSingle();
			float num2 = referenceWhite.ToSingle();
			f1 = (float)codingRange / (num2 - num);
			f2 = f1 * num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public float Expand(float code)
		{
			return code * f1 - f2;
		}
	}

	private readonly struct YCbCrToRgbConverter
	{
		private readonly float cr2R;

		private readonly float cb2B;

		private readonly float y2G;

		private readonly float cr2G;

		private readonly float cb2G;

		public YCbCrToRgbConverter(Rational lumaRed, Rational lumaGreen, Rational lumaBlue)
		{
			cr2R = 2f - 2f * lumaRed.ToSingle();
			cb2B = 2f - 2f * lumaBlue.ToSingle();
			y2G = (1f - lumaBlue.ToSingle() - lumaRed.ToSingle()) / lumaGreen.ToSingle();
			cr2G = 2f * lumaRed.ToSingle() * (lumaRed.ToSingle() - 1f) / lumaGreen.ToSingle();
			cb2G = 2f * lumaBlue.ToSingle() * (lumaBlue.ToSingle() - 1f) / lumaGreen.ToSingle();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Rgba32 Convert(float y, float cb, float cr)
		{
			Rgba32 result = default(Rgba32);
			result.R = RoundAndClampTo8Bit(cr * cr2R + y);
			result.G = RoundAndClampTo8Bit(y2G * y + cr2G * cr + cb2G * cb);
			result.B = RoundAndClampTo8Bit(cb * cb2B + y);
			result.A = byte.MaxValue;
			return result;
		}
	}

	private readonly CodingRangeExpander yExpander;

	private readonly CodingRangeExpander cbExpander;

	private readonly CodingRangeExpander crExpander;

	private readonly YCbCrToRgbConverter converter;

	private static readonly Rational[] DefaultLuma = new Rational[3]
	{
		new Rational(299u, 1000u),
		new Rational(587u, 1000u),
		new Rational(114u, 1000u)
	};

	private static readonly Rational[] DefaultReferenceBlackWhite = new Rational[6]
	{
		new Rational(0u, 1u),
		new Rational(255u, 1u),
		new Rational(128u, 1u),
		new Rational(255u, 1u),
		new Rational(128u, 1u),
		new Rational(255u, 1u)
	};

	public YCbCrConverter(Rational[] referenceBlackAndWhite, Rational[] coefficients)
	{
		if (referenceBlackAndWhite == null)
		{
			referenceBlackAndWhite = DefaultReferenceBlackWhite;
		}
		if (coefficients == null)
		{
			coefficients = DefaultLuma;
		}
		if (referenceBlackAndWhite.Length != 6)
		{
			TiffThrowHelper.ThrowImageFormatException("reference black and white array should have 6 entry's");
		}
		if (coefficients.Length != 3)
		{
			TiffThrowHelper.ThrowImageFormatException("luma coefficients array should have 6 entry's");
		}
		yExpander = new CodingRangeExpander(referenceBlackAndWhite[0], referenceBlackAndWhite[1], 255);
		cbExpander = new CodingRangeExpander(referenceBlackAndWhite[2], referenceBlackAndWhite[3], 127);
		crExpander = new CodingRangeExpander(referenceBlackAndWhite[4], referenceBlackAndWhite[5], 127);
		converter = new YCbCrToRgbConverter(coefficients[0], coefficients[1], coefficients[2]);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rgba32 ConvertToRgba32(byte y, byte cb, byte cr)
	{
		float y2 = yExpander.Expand((int)y);
		float cb2 = cbExpander.Expand((int)cb);
		float cr2 = crExpander.Expand((int)cr);
		return converter.Convert(y2, cb2, cr2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte RoundAndClampTo8Bit(float value)
	{
		return (byte)Numerics.Clamp((int)MathF.Round(value), 0, 255);
	}
}
