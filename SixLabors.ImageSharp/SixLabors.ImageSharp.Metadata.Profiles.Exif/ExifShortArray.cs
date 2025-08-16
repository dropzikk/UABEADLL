using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifShortArray : ExifArrayValue<ushort>
{
	public override ExifDataType DataType => ExifDataType.Short;

	public ExifShortArray(ExifTag<ushort[]> tag)
		: base(tag)
	{
	}

	public ExifShortArray(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifShortArray(ExifShortArray value)
		: base((ExifArrayValue<ushort>)value)
	{
	}

	public override bool TrySetValue(object? value)
	{
		if (base.TrySetValue(value))
		{
			return true;
		}
		if (value is int[] signed)
		{
			return TrySetSignedIntArray(signed);
		}
		if (value is short[] signed2)
		{
			return TrySetSignedShortArray(signed2);
		}
		if (value is int num)
		{
			if (num >= 0 && num <= 65535)
			{
				base.Value = new ushort[1] { (ushort)num };
			}
			return true;
		}
		if (value is short num2)
		{
			if (num2 >= 0)
			{
				base.Value = new ushort[1] { (ushort)num2 };
			}
			return true;
		}
		return false;
	}

	public override IExifValue DeepClone()
	{
		return new ExifShortArray(this);
	}

	private bool TrySetSignedIntArray(int[] signed)
	{
		if (Array.FindIndex(signed, (int x) => x < 0 || x > 65535) > -1)
		{
			return false;
		}
		ushort[] array = new ushort[signed.Length];
		for (int i = 0; i < signed.Length; i++)
		{
			int num = signed[i];
			array[i] = (ushort)num;
		}
		base.Value = array;
		return true;
	}

	private bool TrySetSignedShortArray(short[] signed)
	{
		if (Array.FindIndex(signed, (short x) => x < 0) > -1)
		{
			return false;
		}
		ushort[] array = new ushort[signed.Length];
		for (int i = 0; i < signed.Length; i++)
		{
			short num = signed[i];
			array[i] = (ushort)num;
		}
		base.Value = array;
		return true;
	}
}
