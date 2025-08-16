using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

internal abstract class BaseExifReader
{
	private delegate TDataType ConverterMethod<TDataType>(ReadOnlySpan<byte> data);

	private readonly MemoryAllocator? allocator;

	private readonly Stream data;

	private List<ExifTag>? invalidTags;

	private List<ulong>? subIfds;

	public IReadOnlyList<ExifTag> InvalidTags
	{
		get
		{
			IReadOnlyList<ExifTag> readOnlyList = invalidTags;
			return readOnlyList ?? Array.Empty<ExifTag>();
		}
	}

	public uint ThumbnailLength { get; protected set; }

	public uint ThumbnailOffset { get; protected set; }

	public bool IsBigEndian { get; protected set; }

	public List<(ulong Offset, ExifDataType DataType, ulong NumberOfComponents, ExifValue Exif)> BigValues { get; } = new List<(ulong, ExifDataType, ulong, ExifValue)>();

	protected BaseExifReader(Stream stream, MemoryAllocator? allocator)
	{
		data = stream ?? throw new ArgumentNullException("stream");
		this.allocator = allocator;
	}

	protected void ReadBigValues(List<IExifValue> values)
	{
		if (BigValues.Count == 0)
		{
			return;
		}
		int num = 0;
		foreach (var bigValue in BigValues)
		{
			ExifDataType item = bigValue.DataType;
			ulong item2 = bigValue.NumberOfComponents;
			ulong num2 = item2 * ExifDataTypes.GetSize(item);
			if ((int)num2 > num)
			{
				num = (int)num2;
			}
		}
		if (allocator != null)
		{
			using IMemoryOwner<byte> buffer = allocator.Allocate<byte>(num);
			Span<byte> span = buffer.GetSpan();
			foreach (var bigValue2 in BigValues)
			{
				ulong num3 = bigValue2.NumberOfComponents * ExifDataTypes.GetSize(bigValue2.DataType);
				ReadBigValue(values, bigValue2, span.Slice(0, (int)num3));
			}
		}
		else
		{
			Span<byte> span2 = ((num > 256) ? ((Span<byte>)new byte[num]) : stackalloc byte[256]);
			Span<byte> span3 = span2;
			foreach (var bigValue3 in BigValues)
			{
				ulong num4 = bigValue3.NumberOfComponents * ExifDataTypes.GetSize(bigValue3.DataType);
				ReadBigValue(values, bigValue3, span3.Slice(0, (int)num4));
			}
		}
		BigValues.Clear();
	}

	protected void ReadValues(List<IExifValue> values, uint offset)
	{
		if (offset <= data.Length)
		{
			Seek(offset);
			int num = ReadUInt16();
			Span<byte> offsetBuffer = stackalloc byte[4];
			for (int i = 0; i < num; i++)
			{
				ReadValue(values, offsetBuffer);
			}
		}
	}

	protected void ReadSubIfd(List<IExifValue> values)
	{
		if (subIfds == null)
		{
			return;
		}
		foreach (ulong subIfd in subIfds)
		{
			ReadValues(values, (uint)subIfd);
		}
	}

	protected void ReadValues64(List<IExifValue> values, ulong offset)
	{
		Seek(offset);
		ulong num = ReadUInt64();
		Span<byte> offsetBuffer = stackalloc byte[8];
		for (ulong num2 = 0uL; num2 < num; num2++)
		{
			ReadValue64(values, offsetBuffer);
		}
	}

	protected void ReadBigValue(IList<IExifValue> values, (ulong Offset, ExifDataType DataType, ulong NumberOfComponents, ExifValue Exif) tag, Span<byte> buffer)
	{
		Seek(tag.Offset);
		if (TryReadSpan(buffer))
		{
			object value = ConvertValue(tag.DataType, buffer, tag.NumberOfComponents > 1 || tag.Exif.IsArray);
			Add(values, tag.Exif, value);
		}
	}

	private static TDataType[] ToArray<TDataType>(ExifDataType dataType, ReadOnlySpan<byte> data, ConverterMethod<TDataType> converter)
	{
		int size = (int)ExifDataTypes.GetSize(dataType);
		int num = data.Length / size;
		TDataType[] array = new TDataType[num];
		for (int i = 0; i < num; i++)
		{
			ReadOnlySpan<byte> readOnlySpan = data.Slice(i * size, size);
			array.SetValue(converter(readOnlySpan), i);
		}
		return array;
	}

	private static string ConvertToString(Encoding encoding, ReadOnlySpan<byte> buffer)
	{
		int num = buffer.IndexOf<byte>(0);
		if (num > -1)
		{
			buffer = buffer.Slice(0, num);
		}
		return encoding.GetString(buffer);
	}

	private static byte ConvertToByte(ReadOnlySpan<byte> buffer)
	{
		return buffer[0];
	}

	private object? ConvertValue(ExifDataType dataType, ReadOnlySpan<byte> buffer, bool isArray)
	{
		if (buffer.Length == 0)
		{
			return null;
		}
		switch (dataType)
		{
		case ExifDataType.Unknown:
			return null;
		case ExifDataType.Ascii:
			return ConvertToString(ExifConstants.DefaultEncoding, buffer);
		case ExifDataType.Byte:
		case ExifDataType.Undefined:
			if (!isArray)
			{
				return ConvertToByte(buffer);
			}
			return buffer.ToArray();
		case ExifDataType.DoubleFloat:
			if (!isArray)
			{
				return ConvertToDouble(buffer);
			}
			return ToArray(dataType, buffer, ConvertToDouble);
		case ExifDataType.Long:
		case ExifDataType.Ifd:
			if (!isArray)
			{
				return ConvertToUInt32(buffer);
			}
			return ToArray(dataType, buffer, ConvertToUInt32);
		case ExifDataType.Rational:
			if (!isArray)
			{
				return ToRational(buffer);
			}
			return ToArray(dataType, buffer, ToRational);
		case ExifDataType.Short:
			if (!isArray)
			{
				return ConvertToShort(buffer);
			}
			return ToArray(dataType, buffer, ConvertToShort);
		case ExifDataType.SignedByte:
			if (!isArray)
			{
				return ConvertToSignedByte(buffer);
			}
			return ToArray(dataType, buffer, ConvertToSignedByte);
		case ExifDataType.SignedLong:
			if (!isArray)
			{
				return ConvertToInt32(buffer);
			}
			return ToArray(dataType, buffer, ConvertToInt32);
		case ExifDataType.SignedRational:
			if (!isArray)
			{
				return ToSignedRational(buffer);
			}
			return ToArray(dataType, buffer, ToSignedRational);
		case ExifDataType.SignedShort:
			if (!isArray)
			{
				return ConvertToSignedShort(buffer);
			}
			return ToArray(dataType, buffer, ConvertToSignedShort);
		case ExifDataType.SingleFloat:
			if (!isArray)
			{
				return ConvertToSingle(buffer);
			}
			return ToArray(dataType, buffer, ConvertToSingle);
		case ExifDataType.Long8:
		case ExifDataType.Ifd8:
			if (!isArray)
			{
				return ConvertToUInt64(buffer);
			}
			return ToArray(dataType, buffer, ConvertToUInt64);
		case ExifDataType.SignedLong8:
			if (!isArray)
			{
				return ConvertToInt64(buffer);
			}
			return ToArray(dataType, buffer, ConvertToUInt64);
		default:
			throw new NotSupportedException($"Data type {dataType} is not supported.");
		}
	}

	private void ReadValue(List<IExifValue> values, Span<byte> offsetBuffer)
	{
		if (data.Length - data.Position < 12)
		{
			return;
		}
		ExifTagValue exifTagValue = (ExifTagValue)ReadUInt16();
		ExifDataType exifDataType = EnumUtils.Parse(ReadUInt16(), ExifDataType.Unknown);
		uint num = ReadUInt32();
		TryReadSpan(offsetBuffer);
		if (exifDataType == ExifDataType.Unknown)
		{
			return;
		}
		if (num == 0)
		{
			num = 4 / ExifDataTypes.GetSize(exifDataType);
		}
		ExifValue exifValue = ExifValues.Create(exifTagValue) ?? ExifValues.Create(exifTagValue, exifDataType, num);
		if ((object)exifValue == null)
		{
			AddInvalidTag(new UnkownExifTag(exifTagValue));
			return;
		}
		uint num2 = num * ExifDataTypes.GetSize(exifDataType);
		if (num2 > 4)
		{
			uint num3 = ConvertToUInt32(offsetBuffer);
			if (num3 > int.MaxValue || num3 + num2 > data.Length)
			{
				AddInvalidTag(new UnkownExifTag(exifTagValue));
			}
			else
			{
				BigValues.Add((num3, exifDataType, num, exifValue));
			}
		}
		else
		{
			object value = ConvertValue(exifDataType, offsetBuffer.Slice(0, (int)num2), num > 1 || exifValue.IsArray);
			Add(values, exifValue, value);
		}
	}

	private void ReadValue64(List<IExifValue> values, Span<byte> offsetBuffer)
	{
		if (data.Length - data.Position < 20)
		{
			return;
		}
		ExifTagValue exifTagValue = (ExifTagValue)ReadUInt16();
		ExifDataType exifDataType = EnumUtils.Parse(ReadUInt16(), ExifDataType.Unknown);
		ulong num = ReadUInt64();
		TryReadSpan(offsetBuffer);
		if (exifDataType == ExifDataType.Unknown)
		{
			return;
		}
		if (num == 0L)
		{
			num = 8 / ExifDataTypes.GetSize(exifDataType);
		}
		ExifValue exifValue = exifTagValue switch
		{
			ExifTagValue.StripOffsets => new ExifLong8Array(ExifTagValue.StripOffsets), 
			ExifTagValue.StripByteCounts => new ExifLong8Array(ExifTagValue.StripByteCounts), 
			ExifTagValue.TileOffsets => new ExifLong8Array(ExifTagValue.TileOffsets), 
			ExifTagValue.TileByteCounts => new ExifLong8Array(ExifTagValue.TileByteCounts), 
			_ => ExifValues.Create(exifTagValue) ?? ExifValues.Create(exifTagValue, exifDataType, num), 
		};
		if ((object)exifValue == null)
		{
			AddInvalidTag(new UnkownExifTag(exifTagValue));
			return;
		}
		ulong num2 = num * ExifDataTypes.GetSize(exifDataType);
		if (num2 > 8)
		{
			ulong num3 = ConvertToUInt64(offsetBuffer);
			if (num3 > ulong.MaxValue || num3 > (ulong)(data.Length - (long)num2))
			{
				AddInvalidTag(new UnkownExifTag(exifTagValue));
			}
			else
			{
				BigValues.Add((num3, exifDataType, num, exifValue));
			}
		}
		else
		{
			object value = ConvertValue(exifDataType, offsetBuffer.Slice(0, (int)num2), num > 1 || exifValue.IsArray);
			Add(values, exifValue, value);
		}
	}

	private void Add(IList<IExifValue> values, IExifValue exif, object? value)
	{
		if (!exif.TrySetValue(value))
		{
			return;
		}
		foreach (IExifValue value2 in values)
		{
			if (value2 == exif)
			{
				return;
			}
		}
		if (exif.Tag == ExifTag.SubIFDOffset)
		{
			AddSubIfd(value);
		}
		else if (exif.Tag == ExifTag.GPSIFDOffset)
		{
			AddSubIfd(value);
		}
		else
		{
			values.Add(exif);
		}
	}

	private void AddInvalidTag(ExifTag tag)
	{
		(invalidTags ?? (invalidTags = new List<ExifTag>())).Add(tag);
	}

	private void AddSubIfd(object? val)
	{
		(subIfds ?? (subIfds = new List<ulong>())).Add(Convert.ToUInt64(val, CultureInfo.InvariantCulture));
	}

	private void Seek(ulong pos)
	{
		data.Seek((long)pos, SeekOrigin.Begin);
	}

	private bool TryReadSpan(Span<byte> span)
	{
		int length = span.Length;
		if (data.Length - data.Position < length)
		{
			return false;
		}
		return data.Read(span) == length;
	}

	protected ulong ReadUInt64()
	{
		Span<byte> span = stackalloc byte[8];
		if (!TryReadSpan(span))
		{
			return 0uL;
		}
		return ConvertToUInt64(span);
	}

	protected uint ReadUInt32()
	{
		Span<byte> span = stackalloc byte[4];
		if (!TryReadSpan(span))
		{
			return 0u;
		}
		return ConvertToUInt32(span);
	}

	protected ushort ReadUInt16()
	{
		Span<byte> span = stackalloc byte[2];
		if (!TryReadSpan(span))
		{
			return 0;
		}
		return ConvertToShort(span);
	}

	private long ConvertToInt64(ReadOnlySpan<byte> buffer)
	{
		if (buffer.Length < 8)
		{
			return 0L;
		}
		if (!IsBigEndian)
		{
			return BinaryPrimitives.ReadInt64LittleEndian(buffer);
		}
		return BinaryPrimitives.ReadInt64BigEndian(buffer);
	}

	private ulong ConvertToUInt64(ReadOnlySpan<byte> buffer)
	{
		if (buffer.Length < 8)
		{
			return 0uL;
		}
		if (!IsBigEndian)
		{
			return BinaryPrimitives.ReadUInt64LittleEndian(buffer);
		}
		return BinaryPrimitives.ReadUInt64BigEndian(buffer);
	}

	private double ConvertToDouble(ReadOnlySpan<byte> buffer)
	{
		if (buffer.Length < 8)
		{
			return 0.0;
		}
		long source = (IsBigEndian ? BinaryPrimitives.ReadInt64BigEndian(buffer) : BinaryPrimitives.ReadInt64LittleEndian(buffer));
		return Unsafe.As<long, double>(ref source);
	}

	private uint ConvertToUInt32(ReadOnlySpan<byte> buffer)
	{
		if (buffer.Length < 4)
		{
			return 0u;
		}
		if (!IsBigEndian)
		{
			return BinaryPrimitives.ReadUInt32LittleEndian(buffer);
		}
		return BinaryPrimitives.ReadUInt32BigEndian(buffer);
	}

	private ushort ConvertToShort(ReadOnlySpan<byte> buffer)
	{
		if (buffer.Length < 2)
		{
			return 0;
		}
		if (!IsBigEndian)
		{
			return BinaryPrimitives.ReadUInt16LittleEndian(buffer);
		}
		return BinaryPrimitives.ReadUInt16BigEndian(buffer);
	}

	private float ConvertToSingle(ReadOnlySpan<byte> buffer)
	{
		if (buffer.Length < 4)
		{
			return 0f;
		}
		int source = (IsBigEndian ? BinaryPrimitives.ReadInt32BigEndian(buffer) : BinaryPrimitives.ReadInt32LittleEndian(buffer));
		return Unsafe.As<int, float>(ref source);
	}

	private Rational ToRational(ReadOnlySpan<byte> buffer)
	{
		if (buffer.Length < 8)
		{
			return default(Rational);
		}
		uint numerator = ConvertToUInt32(buffer.Slice(0, 4));
		uint denominator = ConvertToUInt32(buffer.Slice(4, 4));
		return new Rational(numerator, denominator, simplify: false);
	}

	private sbyte ConvertToSignedByte(ReadOnlySpan<byte> buffer)
	{
		return (sbyte)buffer[0];
	}

	private int ConvertToInt32(ReadOnlySpan<byte> buffer)
	{
		if (buffer.Length < 4)
		{
			return 0;
		}
		if (!IsBigEndian)
		{
			return BinaryPrimitives.ReadInt32LittleEndian(buffer);
		}
		return BinaryPrimitives.ReadInt32BigEndian(buffer);
	}

	private SignedRational ToSignedRational(ReadOnlySpan<byte> buffer)
	{
		if (buffer.Length < 8)
		{
			return default(SignedRational);
		}
		int numerator = ConvertToInt32(buffer.Slice(0, 4));
		int denominator = ConvertToInt32(buffer.Slice(4, 4));
		return new SignedRational(numerator, denominator, simplify: false);
	}

	private short ConvertToSignedShort(ReadOnlySpan<byte> buffer)
	{
		if (buffer.Length < 2)
		{
			return 0;
		}
		if (!IsBigEndian)
		{
			return BinaryPrimitives.ReadInt16LittleEndian(buffer);
		}
		return BinaryPrimitives.ReadInt16BigEndian(buffer);
	}
}
