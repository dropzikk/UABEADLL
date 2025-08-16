namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifLong8Array : ExifArrayValue<ulong>
{
	public override ExifDataType DataType
	{
		get
		{
			if (base.Value != null)
			{
				ulong[] value = base.Value;
				for (int i = 0; i < value.Length; i++)
				{
					if (value[i] > uint.MaxValue)
					{
						return ExifDataType.Long8;
					}
				}
			}
			return ExifDataType.Long;
		}
	}

	public ExifLong8Array(ExifTagValue tag)
		: base(tag)
	{
	}

	private ExifLong8Array(ExifLong8Array value)
		: base((ExifArrayValue<ulong>)value)
	{
	}

	public override bool TrySetValue(object? value)
	{
		if (base.TrySetValue(value))
		{
			return true;
		}
		if (!(value is int value2))
		{
			if (!(value is uint num))
			{
				if (!(value is short value3))
				{
					if (!(value is ushort num2))
					{
						if (!(value is long num3))
						{
							if (!(value is long[] array))
							{
								if (!(value is int[] array2))
								{
									if (value is short[] array3)
									{
										if (value.GetType() == typeof(ushort[]))
										{
											return SetArray((ushort[])value);
										}
										return SetArray(array3);
									}
									return false;
								}
								if (value.GetType() == typeof(uint[]))
								{
									return SetArray((uint[])value);
								}
								return SetArray(array2);
							}
							if (value.GetType() == typeof(ulong[]))
							{
								return SetArray((ulong[])value);
							}
							return SetArray(array);
						}
						return SetSingle((ulong)Numerics.Clamp(num3, 0f, 9.223372E+18f));
					}
					return SetSingle(num2);
				}
				return SetSingle((ulong)Numerics.Clamp(value3, 0, 32767));
			}
			return SetSingle(num);
		}
		return SetSingle((ulong)Numerics.Clamp(value2, 0, int.MaxValue));
	}

	public override IExifValue DeepClone()
	{
		return new ExifLong8Array(this);
	}

	private bool SetSingle(ulong value)
	{
		base.Value = new ulong[1] { value };
		return true;
	}

	private bool SetArray(long[] values)
	{
		ulong[] array = new ulong[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			array[i] = (ulong)((values[i] < 0) ? 0 : values[i]);
		}
		base.Value = array;
		return true;
	}

	private bool SetArray(ulong[] values)
	{
		base.Value = values;
		return true;
	}

	private bool SetArray(int[] values)
	{
		ulong[] array = new ulong[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			array[i] = (ulong)Numerics.Clamp(values[i], 0, int.MaxValue);
		}
		base.Value = array;
		return true;
	}

	private bool SetArray(uint[] values)
	{
		ulong[] array = new ulong[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			array[i] = values[i];
		}
		base.Value = array;
		return true;
	}

	private bool SetArray(short[] values)
	{
		ulong[] array = new ulong[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			array[i] = (ulong)Numerics.Clamp(values[i], 0, 32767);
		}
		base.Value = array;
		return true;
	}

	private bool SetArray(ushort[] values)
	{
		ulong[] array = new ulong[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			array[i] = values[i];
		}
		base.Value = array;
		return true;
	}
}
