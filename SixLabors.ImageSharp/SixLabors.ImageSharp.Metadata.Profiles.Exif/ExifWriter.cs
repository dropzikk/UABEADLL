using System;
using System.Buffers.Binary;
using System.Collections.Generic;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal sealed class ExifWriter
{
	private readonly ExifParts allowedParts;

	private readonly IList<IExifValue> values;

	private List<int>? dataOffsets;

	private readonly List<IExifValue> ifdValues;

	private readonly List<IExifValue> exifValues;

	private readonly List<IExifValue> gpsValues;

	public ExifWriter(IList<IExifValue> values, ExifParts allowedParts)
	{
		this.values = values;
		this.allowedParts = allowedParts;
		ifdValues = GetPartValues(ExifParts.IfdTags);
		exifValues = GetPartValues(ExifParts.ExifTags);
		gpsValues = GetPartValues(ExifParts.GpsTags);
	}

	public byte[] GetData()
	{
		IExifValue offsetValue = GetOffsetValue(ifdValues, exifValues, ExifTag.SubIFDOffset);
		IExifValue offsetValue2 = GetOffsetValue(ifdValues, gpsValues, ExifTag.GPSIFDOffset);
		uint length = GetLength(ifdValues);
		uint length2 = GetLength(exifValues);
		uint length3 = GetLength(gpsValues);
		uint num = length + length2 + length3;
		if (num == 0)
		{
			return Array.Empty<byte>();
		}
		num += (uint)ExifConstants.LittleEndianByteOrderMarker.Length;
		num += 4;
		byte[] array = new byte[num];
		int num2 = 0;
		ExifConstants.LittleEndianByteOrderMarker.CopyTo(array.AsSpan(num2));
		num2 += ExifConstants.LittleEndianByteOrderMarker.Length;
		uint num3 = (uint)(num2 + 4);
		offsetValue?.TrySetValue(num3 + length);
		offsetValue2?.TrySetValue(num3 + length + length2);
		num2 = WriteUInt32(num3, array, num2);
		num2 = WriteHeaders(ifdValues, array, num2);
		num2 = WriteData(0u, ifdValues, array, num2);
		if (length2 != 0)
		{
			num2 = WriteHeaders(exifValues, array, num2);
			num2 = WriteData(0u, exifValues, array, num2);
		}
		if (length3 != 0)
		{
			num2 = WriteHeaders(gpsValues, array, num2);
			WriteData(0u, gpsValues, array, num2);
		}
		return array;
	}

	private unsafe static int WriteSingle(float value, Span<byte> destination, int offset)
	{
		BinaryPrimitives.WriteInt32LittleEndian(destination.Slice(offset, 4), *(int*)(&value));
		return offset + 4;
	}

	private unsafe static int WriteDouble(double value, Span<byte> destination, int offset)
	{
		BinaryPrimitives.WriteInt64LittleEndian(destination.Slice(offset, 8), *(long*)(&value));
		return offset + 8;
	}

	private static int Write(ReadOnlySpan<byte> source, Span<byte> destination, int offset)
	{
		source.CopyTo(destination.Slice(offset, source.Length));
		return offset + source.Length;
	}

	private static int WriteInt16(short value, Span<byte> destination, int offset)
	{
		BinaryPrimitives.WriteInt16LittleEndian(destination.Slice(offset, 2), value);
		return offset + 2;
	}

	private static int WriteUInt16(ushort value, Span<byte> destination, int offset)
	{
		BinaryPrimitives.WriteUInt16LittleEndian(destination.Slice(offset, 2), value);
		return offset + 2;
	}

	private static int WriteUInt32(uint value, Span<byte> destination, int offset)
	{
		BinaryPrimitives.WriteUInt32LittleEndian(destination.Slice(offset, 4), value);
		return offset + 4;
	}

	private static int WriteInt64(long value, Span<byte> destination, int offset)
	{
		BinaryPrimitives.WriteInt64LittleEndian(destination.Slice(offset, 8), value);
		return offset + 8;
	}

	private static int WriteUInt64(ulong value, Span<byte> destination, int offset)
	{
		BinaryPrimitives.WriteUInt64LittleEndian(destination.Slice(offset, 8), value);
		return offset + 8;
	}

	private static int WriteInt32(int value, Span<byte> destination, int offset)
	{
		BinaryPrimitives.WriteInt32LittleEndian(destination.Slice(offset, 4), value);
		return offset + 4;
	}

	private static IExifValue? GetOffsetValue(List<IExifValue> ifdValues, List<IExifValue> values, ExifTag offset)
	{
		int num = -1;
		for (int i = 0; i < ifdValues.Count; i++)
		{
			if (ifdValues[i].Tag == offset)
			{
				num = i;
			}
		}
		if (values.Count > 0)
		{
			if (num != -1)
			{
				return ifdValues[num];
			}
			ExifValue exifValue = ExifValues.Create(offset);
			if ((object)exifValue != null)
			{
				ifdValues.Add(exifValue);
			}
			return exifValue;
		}
		if (num != -1)
		{
			ifdValues.RemoveAt(num);
		}
		return null;
	}

	private List<IExifValue> GetPartValues(ExifParts part)
	{
		List<IExifValue> list = new List<IExifValue>();
		if (!EnumUtils.HasFlag(allowedParts, part))
		{
			return list;
		}
		foreach (IExifValue value in values)
		{
			if (HasValue(value) && ExifTags.GetPart(value.Tag) == part)
			{
				list.Add(value);
			}
		}
		return list;
	}

	private static bool HasValue(IExifValue exifValue)
	{
		object value = exifValue.GetValue();
		if (value == null)
		{
			return false;
		}
		if (exifValue.DataType == ExifDataType.Ascii && value is string text)
		{
			return text.Length > 0;
		}
		if (value is Array array)
		{
			return array.Length > 0;
		}
		return true;
	}

	private static uint GetLength(IList<IExifValue> values)
	{
		if (values.Count == 0)
		{
			return 0u;
		}
		uint num = 2u;
		foreach (IExifValue value in values)
		{
			uint length = GetLength(value);
			num += 12;
			if (length > 4)
			{
				num += length;
			}
		}
		return num + 4;
	}

	internal static uint GetLength(IExifValue value)
	{
		return GetNumberOfComponents(value) * ExifDataTypes.GetSize(value.DataType);
	}

	internal static uint GetNumberOfComponents(IExifValue exifValue)
	{
		object value = exifValue.GetValue();
		if (ExifUcs2StringHelpers.IsUcs2Tag((ExifTagValue)(ushort)exifValue.Tag))
		{
			return (uint)ExifUcs2StringHelpers.Ucs2Encoding.GetByteCount((string)value);
		}
		if (value is EncodedString encodedString)
		{
			return ExifEncodedStringHelpers.GetDataLength(encodedString);
		}
		if (exifValue.DataType == ExifDataType.Ascii)
		{
			return (uint)(ExifConstants.DefaultEncoding.GetByteCount((string)value) + 1);
		}
		if (value is Array array)
		{
			return (uint)array.Length;
		}
		return 1u;
	}

	private static int WriteArray(IExifValue value, Span<byte> destination, int offset)
	{
		int num = offset;
		foreach (object item in (Array)value.GetValue())
		{
			num = WriteValue(value.DataType, item, destination, num);
		}
		return num;
	}

	private int WriteData(uint startIndex, List<IExifValue> values, Span<byte> destination, int offset)
	{
		if (dataOffsets == null || dataOffsets.Count == 0)
		{
			return offset;
		}
		int num = offset;
		int num2 = 0;
		foreach (IExifValue value in values)
		{
			if (GetLength(value) > 4)
			{
				WriteUInt32((uint)(num - startIndex), destination, dataOffsets[num2++]);
				num = WriteValue(value, destination, num);
			}
		}
		return num;
	}

	private int WriteHeaders(List<IExifValue> values, Span<byte> destination, int offset)
	{
		dataOffsets = new List<int>();
		int num = WriteUInt16((ushort)values.Count, destination, offset);
		if (values.Count == 0)
		{
			return num;
		}
		foreach (IExifValue value in values)
		{
			num = WriteUInt16((ushort)value.Tag, destination, num);
			num = WriteUInt16((ushort)value.DataType, destination, num);
			num = WriteUInt32(GetNumberOfComponents(value), destination, num);
			if (GetLength(value) > 4)
			{
				dataOffsets.Add(num);
			}
			else
			{
				WriteValue(value, destination, num);
			}
			num += 4;
		}
		return WriteUInt32(0u, destination, num);
	}

	private static void WriteRational(Span<byte> destination, in Rational value)
	{
		BinaryPrimitives.WriteUInt32LittleEndian(destination.Slice(0, 4), value.Numerator);
		BinaryPrimitives.WriteUInt32LittleEndian(destination.Slice(4, 4), value.Denominator);
	}

	private static void WriteSignedRational(Span<byte> destination, in SignedRational value)
	{
		BinaryPrimitives.WriteInt32LittleEndian(destination.Slice(0, 4), value.Numerator);
		BinaryPrimitives.WriteInt32LittleEndian(destination.Slice(4, 4), value.Denominator);
	}

	private static int WriteValue(ExifDataType dataType, object value, Span<byte> destination, int offset)
	{
		switch (dataType)
		{
		case ExifDataType.Ascii:
			offset = Write(ExifConstants.DefaultEncoding.GetBytes((string)value), destination, offset);
			destination[offset] = 0;
			return offset + 1;
		case ExifDataType.Byte:
		case ExifDataType.Undefined:
			destination[offset] = (byte)value;
			return offset + 1;
		case ExifDataType.DoubleFloat:
			return WriteDouble((double)value, destination, offset);
		case ExifDataType.Short:
			if (value is Number number2)
			{
				return WriteUInt16((ushort)number2, destination, offset);
			}
			return WriteUInt16((ushort)value, destination, offset);
		case ExifDataType.Long:
			if (value is Number number)
			{
				return WriteUInt32((uint)number, destination, offset);
			}
			return WriteUInt32((uint)value, destination, offset);
		case ExifDataType.Long8:
			return WriteUInt64((ulong)value, destination, offset);
		case ExifDataType.SignedLong8:
			return WriteInt64((long)value, destination, offset);
		case ExifDataType.Rational:
			WriteRational(destination.Slice(offset, 8), (Rational)value);
			return offset + 8;
		case ExifDataType.SignedByte:
			destination[offset] = (byte)(sbyte)value;
			return offset + 1;
		case ExifDataType.SignedLong:
			return WriteInt32((int)value, destination, offset);
		case ExifDataType.SignedShort:
			return WriteInt16((short)value, destination, offset);
		case ExifDataType.SignedRational:
			WriteSignedRational(destination.Slice(offset, 8), (SignedRational)value);
			return offset + 8;
		case ExifDataType.SingleFloat:
			return WriteSingle((float)value, destination, offset);
		default:
			throw new NotImplementedException();
		}
	}

	internal static int WriteValue(IExifValue exifValue, Span<byte> destination, int offset)
	{
		object value = exifValue.GetValue();
		Guard.NotNull(value, "value");
		if (ExifUcs2StringHelpers.IsUcs2Tag((ExifTagValue)(ushort)exifValue.Tag))
		{
			string value2 = (string)value;
			ref Span<byte> reference = ref destination;
			int num = offset;
			return offset + ExifUcs2StringHelpers.Write(value2, reference.Slice(num, reference.Length - num));
		}
		if (value is EncodedString encodedString)
		{
			ref Span<byte> reference = ref destination;
			int num = offset;
			return offset + ExifEncodedStringHelpers.Write(encodedString, reference.Slice(num, reference.Length - num));
		}
		if (exifValue.IsArray)
		{
			return WriteArray(exifValue, destination, offset);
		}
		return WriteValue(exifValue.DataType, value, destination, offset);
	}
}
