namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifNumberArray : ExifArrayValue<Number>
{
	public override ExifDataType DataType
	{
		get
		{
			if (base.Value != null)
			{
				Number[] value = base.Value;
				for (int i = 0; i < value.Length; i++)
				{
					if (value[i] > ushort.MaxValue)
					{
						return ExifDataType.Long;
					}
				}
			}
			return ExifDataType.Short;
		}
	}

	public ExifNumberArray(ExifTag<Number[]> tag)
		: base(tag)
	{
	}

	private ExifNumberArray(ExifNumberArray value)
		: base((ExifArrayValue<Number>)value)
	{
	}

	public override bool TrySetValue(object? value)
	{
		if (base.TrySetValue(value))
		{
			return true;
		}
		if (!(value is int num))
		{
			if (!(value is uint num2))
			{
				if (!(value is short num3))
				{
					if (!(value is ushort num4))
					{
						if (!(value is int[] array))
						{
							if (value is short[] array2)
							{
								if (value.GetType() == typeof(ushort[]))
								{
									return SetArray((ushort[])value);
								}
								return SetArray(array2);
							}
							return false;
						}
						if (value.GetType() == typeof(uint[]))
						{
							return SetArray((uint[])value);
						}
						return SetArray(array);
					}
					return SetSingle(num4);
				}
				return SetSingle(num3);
			}
			return SetSingle(num2);
		}
		return SetSingle(num);
	}

	public override IExifValue DeepClone()
	{
		return new ExifNumberArray(this);
	}

	private bool SetSingle(Number value)
	{
		base.Value = new Number[1] { value };
		return true;
	}

	private bool SetArray(int[] values)
	{
		Number[] array = new Number[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			array[i] = values[i];
		}
		base.Value = array;
		return true;
	}

	private bool SetArray(uint[] values)
	{
		Number[] array = new Number[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			array[i] = values[i];
		}
		base.Value = array;
		return true;
	}

	private bool SetArray(short[] values)
	{
		Number[] array = new Number[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			array[i] = values[i];
		}
		base.Value = array;
		return true;
	}

	private bool SetArray(ushort[] values)
	{
		Number[] array = new Number[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			array[i] = values[i];
		}
		base.Value = array;
		return true;
	}
}
