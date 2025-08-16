using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifByteArray : ExifArrayValue<byte>
{
	public override ExifDataType DataType { get; }

	public ExifByteArray(ExifTag<byte[]> tag, ExifDataType dataType)
		: base(tag)
	{
		DataType = dataType;
	}

	public ExifByteArray(ExifTagValue tag, ExifDataType dataType)
		: base(tag)
	{
		DataType = dataType;
	}

	private ExifByteArray(ExifByteArray value)
		: base((ExifArrayValue<byte>)value)
	{
		DataType = value.DataType;
	}

	public override bool TrySetValue(object? value)
	{
		if (base.TrySetValue(value))
		{
			return true;
		}
		if (value is int[] intArrayValue)
		{
			return TrySetSignedIntArray(intArrayValue);
		}
		if (value is int num)
		{
			if (num >= 0 && num <= 255)
			{
				base.Value = new byte[1] { (byte)num };
			}
			return true;
		}
		return false;
	}

	public override IExifValue DeepClone()
	{
		return new ExifByteArray(this);
	}

	private bool TrySetSignedIntArray(int[] intArrayValue)
	{
		if (Array.FindIndex(intArrayValue, (int x) => (uint)x > 255u) >= 0)
		{
			return false;
		}
		byte[] array = new byte[intArrayValue.Length];
		for (int i = 0; i < intArrayValue.Length; i++)
		{
			int num = intArrayValue[i];
			array[i] = (byte)num;
		}
		base.Value = array;
		return true;
	}
}
