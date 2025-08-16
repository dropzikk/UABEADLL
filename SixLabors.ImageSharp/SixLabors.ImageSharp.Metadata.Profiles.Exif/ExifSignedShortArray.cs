using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifSignedShortArray : ExifArrayValue<short>
{
	public override ExifDataType DataType => ExifDataType.SignedShort;

	public ExifSignedShortArray(ExifTag<short[]> tag)
		: base(tag)
	{
	}

	public ExifSignedShortArray(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifSignedShortArray(ExifSignedShortArray value)
		: base((ExifArrayValue<short>)value)
	{
	}

	public override bool TrySetValue(object? value)
	{
		if (base.TrySetValue(value))
		{
			return true;
		}
		if (value is int[] intArray)
		{
			return TrySetSignedArray(intArray);
		}
		if (value is int num)
		{
			if (num >= -32768 && num <= 32767)
			{
				base.Value = new short[1] { (short)num };
			}
			return true;
		}
		return false;
	}

	public override IExifValue DeepClone()
	{
		return new ExifSignedShortArray(this);
	}

	private bool TrySetSignedArray(int[] intArray)
	{
		if (Array.FindIndex(intArray, (int x) => x < -32768 || x > 32767) > -1)
		{
			return false;
		}
		short[] array = new short[intArray.Length];
		for (int i = 0; i < intArray.Length; i++)
		{
			int num = intArray[i];
			array[i] = (short)num;
		}
		base.Value = array;
		return true;
	}
}
