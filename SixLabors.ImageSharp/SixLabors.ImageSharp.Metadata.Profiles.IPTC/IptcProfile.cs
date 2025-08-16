using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using SixLabors.ImageSharp.Metadata.Profiles.IPTC;

namespace SixLabors.ImageSharp.Metadata.Profiles.Iptc;

public sealed class IptcProfile : IDeepCloneable<IptcProfile>
{
	private readonly Collection<IptcValue> values = new Collection<IptcValue>();

	private const byte IptcTagMarkerByte = 28;

	private const uint MaxStandardDataTagSize = 32767u;

	private const byte IptcEnvelopeCodedCharacterSet = 90;

	private static ReadOnlySpan<byte> CodedCharacterSetUtf8Value => new byte[3] { 27, 37, 71 };

	public byte[]? Data { get; private set; }

	public IEnumerable<IptcValue> Values => values;

	public IptcProfile()
		: this((byte[]?)null)
	{
	}

	public IptcProfile(byte[]? data)
	{
		Data = data;
		Initialize();
	}

	private IptcProfile(IptcProfile other)
	{
		Guard.NotNull(other, "other");
		foreach (IptcValue value in other.Values)
		{
			values.Add(value.DeepClone());
		}
		if (other.Data != null)
		{
			Data = new byte[other.Data.Length];
			other.Data.AsSpan().CopyTo(Data);
		}
	}

	public IptcProfile DeepClone()
	{
		return new IptcProfile(this);
	}

	public List<IptcValue> GetValues(IptcTag tag)
	{
		List<IptcValue> list = new List<IptcValue>();
		foreach (IptcValue value in Values)
		{
			if (value.Tag == tag)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public bool RemoveValue(IptcTag tag)
	{
		bool result = false;
		for (int num = values.Count - 1; num >= 0; num--)
		{
			if (values[num].Tag == tag)
			{
				values.RemoveAt(num);
				result = true;
			}
		}
		return result;
	}

	public bool RemoveValue(IptcTag tag, string value)
	{
		bool result = false;
		for (int num = values.Count - 1; num >= 0; num--)
		{
			if (values[num].Tag == tag && values[num].Value.Equals(value, StringComparison.OrdinalIgnoreCase))
			{
				values.RemoveAt(num);
				result = true;
			}
		}
		return result;
	}

	public void SetEncoding(Encoding encoding)
	{
		Guard.NotNull(encoding, "encoding");
		foreach (IptcValue value in Values)
		{
			value.Encoding = encoding;
		}
	}

	public void SetValue(IptcTag tag, Encoding encoding, string value, bool strict = true)
	{
		Guard.NotNull(encoding, "encoding");
		Guard.NotNull(value, "value");
		if (!tag.IsRepeatable())
		{
			foreach (IptcValue value2 in Values)
			{
				if (value2.Tag == tag)
				{
					value2.Strict = strict;
					value2.Encoding = encoding;
					value2.Value = value;
					return;
				}
			}
		}
		values.Add(new IptcValue(tag, encoding, value, strict));
	}

	public void SetValue(IptcTag tag, string value, bool strict = true)
	{
		SetValue(tag, Encoding.UTF8, value, strict);
	}

	public void SetDateTimeValue(IptcTag tag, DateTimeOffset dateTimeOffset)
	{
		if (!tag.IsDate() && !tag.IsTime())
		{
			throw new ArgumentException("Iptc tag is not a time or date type.");
		}
		string value = (tag.IsDate() ? dateTimeOffset.ToString("yyyyMMdd", CultureInfo.InvariantCulture) : dateTimeOffset.ToString("HHmmsszzzz", CultureInfo.InvariantCulture).Replace(":", string.Empty));
		SetValue(tag, Encoding.UTF8, value);
	}

	public void UpdateData()
	{
		int num = 0;
		foreach (IptcValue value in Values)
		{
			num += value.Length + 5;
		}
		bool num2 = HasValuesInUtf8();
		if (num2)
		{
			num += 5 + CodedCharacterSetUtf8Value.Length;
		}
		Data = new byte[num];
		int offset = 0;
		if (num2)
		{
			offset = WriteRecord(offset, CodedCharacterSetUtf8Value, IptcRecordNumber.Envelope, 90);
		}
		foreach (IptcValue value2 in Values)
		{
			offset = WriteRecord(offset, value2.ToByteArray(), IptcRecordNumber.Application, (byte)value2.Tag);
		}
	}

	private int WriteRecord(int offset, ReadOnlySpan<byte> recordData, IptcRecordNumber recordNumber, byte recordBinaryRepresentation)
	{
		Span<byte> span = Data.AsSpan(offset, 5);
		span[0] = 28;
		span[1] = (byte)recordNumber;
		span[2] = recordBinaryRepresentation;
		span[3] = (byte)(recordData.Length >> 8);
		span[4] = (byte)recordData.Length;
		offset += 5;
		if (recordData.Length > 0)
		{
			recordData.CopyTo(Data.AsSpan(offset));
			offset += recordData.Length;
		}
		return offset;
	}

	private void Initialize()
	{
		if (Data == null || Data[0] != 28)
		{
			return;
		}
		int num = 0;
		while (num < Data.Length - 4)
		{
			bool num2 = Data[num++] == 28;
			byte b = Data[num++];
			bool flag = b >= 1 && b <= 9;
			IptcTag tag = (IptcTag)Data[num++];
			bool flag2 = num2 && flag;
			bool flag3 = b == 2;
			uint num3 = BinaryPrimitives.ReadUInt16BigEndian(Data.AsSpan(num, 2));
			num += 2;
			if (num3 <= 32767)
			{
				if (flag2 && flag3 && num3 != 0 && num <= Data.Length - num3)
				{
					byte[] array = new byte[num3];
					Buffer.BlockCopy(Data, num, array, 0, (int)num3);
					values.Add(new IptcValue(tag, array, strict: false));
				}
				num += (int)num3;
				continue;
			}
			break;
		}
	}

	private bool HasValuesInUtf8()
	{
		foreach (IptcValue value in values)
		{
			if (value.Encoding == Encoding.UTF8)
			{
				return true;
			}
		}
		return false;
	}
}
