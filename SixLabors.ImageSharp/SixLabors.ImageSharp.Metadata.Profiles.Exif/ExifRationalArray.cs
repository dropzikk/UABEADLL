using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifRationalArray : ExifArrayValue<Rational>
{
	public override ExifDataType DataType => ExifDataType.Rational;

	public ExifRationalArray(ExifTag<Rational[]> tag)
		: base(tag)
	{
	}

	public ExifRationalArray(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifRationalArray(ExifRationalArray value)
		: base((ExifArrayValue<Rational>)value)
	{
	}

	public override bool TrySetValue(object? value)
	{
		if (base.TrySetValue(value))
		{
			return true;
		}
		if (value is SignedRational[] signed)
		{
			return TrySetSignedArray(signed);
		}
		if (value is SignedRational signedRational)
		{
			if (signedRational.Numerator >= 0 && signedRational.Denominator >= 0)
			{
				base.Value = new Rational[1]
				{
					new Rational((uint)signedRational.Numerator, (uint)signedRational.Denominator)
				};
			}
			return true;
		}
		return false;
	}

	public override IExifValue DeepClone()
	{
		return new ExifRationalArray(this);
	}

	private bool TrySetSignedArray(SignedRational[] signed)
	{
		if (Array.FindIndex(signed, (SignedRational x) => x.Numerator < 0 || x.Denominator < 0) > -1)
		{
			return false;
		}
		Rational[] array = new Rational[signed.Length];
		for (int i = 0; i < signed.Length; i++)
		{
			SignedRational signedRational = signed[i];
			array[i] = new Rational((uint)signedRational.Numerator, (uint)signedRational.Denominator);
		}
		base.Value = array;
		return true;
	}
}
