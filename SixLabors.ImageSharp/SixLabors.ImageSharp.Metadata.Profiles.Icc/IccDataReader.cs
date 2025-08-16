using System;
using System.Buffers.Binary;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccDataReader
{
	private readonly byte[] data;

	private int currentIndex;

	public int DataLength => data.Length;

	public IccDataReader(byte[] data)
	{
		this.data = data ?? throw new ArgumentNullException("data");
	}

	public void SetIndex(int index)
	{
		currentIndex = Numerics.Clamp(index, 0, data.Length);
	}

	private int AddIndex(int increment)
	{
		int result = currentIndex;
		currentIndex += increment;
		return result;
	}

	private void AddPadding()
	{
		currentIndex += CalcPadding();
	}

	private int CalcPadding()
	{
		int num = 4 - currentIndex % 4;
		if (num < 4)
		{
			return num;
		}
		return 0;
	}

	private static bool GetBit(byte value, int position)
	{
		return ((value >> 7 - position) & 1) == 1;
	}

	public IccOneDimensionalCurve ReadOneDimensionalCurve()
	{
		ushort num = ReadUInt16();
		AddIndex(2);
		float[] array = new float[num - 1];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = ReadSingle();
		}
		IccCurveSegment[] array2 = new IccCurveSegment[num];
		for (int j = 0; j < num; j++)
		{
			array2[j] = ReadCurveSegment();
		}
		return new IccOneDimensionalCurve(array, array2);
	}

	public IccResponseCurve ReadResponseCurve(int channelCount)
	{
		IccCurveMeasurementEncodings curveType = (IccCurveMeasurementEncodings)ReadUInt32();
		uint[] array = new uint[channelCount];
		for (int i = 0; i < channelCount; i++)
		{
			array[i] = ReadUInt32();
		}
		Vector3[] array2 = new Vector3[channelCount];
		for (int j = 0; j < channelCount; j++)
		{
			array2[j] = ReadXyzNumber();
		}
		IccResponseNumber[][] array3 = new IccResponseNumber[channelCount][];
		for (int k = 0; k < channelCount; k++)
		{
			array3[k] = new IccResponseNumber[array[k]];
			for (uint num = 0u; num < array[k]; num++)
			{
				array3[k][num] = ReadResponseNumber();
			}
		}
		return new IccResponseCurve(curveType, array2, array3);
	}

	public IccParametricCurve ReadParametricCurve()
	{
		ushort num = ReadUInt16();
		AddIndex(2);
		float a;
		float b;
		float c;
		float d;
		float e;
		float f;
		float g = (a = (b = (c = (d = (e = (f = 0f))))));
		if (num <= 4)
		{
			g = ReadFix16();
		}
		if (num > 0 && num <= 4)
		{
			a = ReadFix16();
			b = ReadFix16();
		}
		if (num > 1 && num <= 4)
		{
			c = ReadFix16();
		}
		if (num > 2 && num <= 4)
		{
			d = ReadFix16();
		}
		if (num == 4)
		{
			e = ReadFix16();
			f = ReadFix16();
		}
		return num switch
		{
			0 => new IccParametricCurve(g), 
			1 => new IccParametricCurve(g, a, b), 
			2 => new IccParametricCurve(g, a, b, c), 
			3 => new IccParametricCurve(g, a, b, c, d), 
			4 => new IccParametricCurve(g, a, b, c, d, e, f), 
			_ => throw new InvalidIccProfileException($"Invalid parametric curve type of {num}"), 
		};
	}

	public IccCurveSegment ReadCurveSegment()
	{
		IccCurveSegmentSignature iccCurveSegmentSignature = (IccCurveSegmentSignature)ReadUInt32();
		AddIndex(4);
		return iccCurveSegmentSignature switch
		{
			IccCurveSegmentSignature.FormulaCurve => ReadFormulaCurveElement(), 
			IccCurveSegmentSignature.SampledCurve => ReadSampledCurveElement(), 
			_ => throw new InvalidIccProfileException($"Invalid curve segment type of {iccCurveSegmentSignature}"), 
		};
	}

	public IccFormulaCurveElement ReadFormulaCurveElement()
	{
		IccFormulaCurveType iccFormulaCurveType = (IccFormulaCurveType)ReadUInt16();
		AddIndex(2);
		float d;
		float e;
		float gamma = (d = (e = 0f));
		if (iccFormulaCurveType == IccFormulaCurveType.Type1 || iccFormulaCurveType == IccFormulaCurveType.Type2)
		{
			gamma = ReadSingle();
		}
		float a = ReadSingle();
		float b = ReadSingle();
		float c = ReadSingle();
		if (iccFormulaCurveType == IccFormulaCurveType.Type2 || iccFormulaCurveType == IccFormulaCurveType.Type3)
		{
			d = ReadSingle();
		}
		if (iccFormulaCurveType == IccFormulaCurveType.Type3)
		{
			e = ReadSingle();
		}
		return new IccFormulaCurveElement(iccFormulaCurveType, gamma, a, b, c, d, e);
	}

	public IccSampledCurveElement ReadSampledCurveElement()
	{
		uint num = ReadUInt32();
		float[] array = new float[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadSingle();
		}
		return new IccSampledCurveElement(array);
	}

	private IccTagDataEntry[] ReadCurves(int count)
	{
		IccTagDataEntry[] array = new IccTagDataEntry[count];
		for (int i = 0; i < count; i++)
		{
			IccTypeSignature iccTypeSignature = ReadTagDataEntryHeader();
			if (iccTypeSignature != IccTypeSignature.Curve && iccTypeSignature != IccTypeSignature.ParametricCurve)
			{
				throw new InvalidIccProfileException("Curve has to be either \"IccTypeSignature.Curve\" or \"IccTypeSignature.ParametricCurve\" for LutAToB- and LutBToA-TagDataEntries");
			}
			switch (iccTypeSignature)
			{
			case IccTypeSignature.Curve:
				array[i] = ReadCurveTagDataEntry();
				break;
			case IccTypeSignature.ParametricCurve:
				array[i] = ReadParametricCurveTagDataEntry();
				break;
			}
			AddPadding();
		}
		return array;
	}

	public IccLut ReadLut8()
	{
		return new IccLut(ReadBytes(256));
	}

	public IccLut ReadLut16(int count)
	{
		ushort[] array = new ushort[count];
		for (int i = 0; i < count; i++)
		{
			array[i] = ReadUInt16();
		}
		return new IccLut(array);
	}

	public IccClut ReadClut(int inChannelCount, int outChannelCount, bool isFloat)
	{
		byte[] array = new byte[inChannelCount];
		Buffer.BlockCopy(data, AddIndex(16), array, 0, inChannelCount);
		if (!isFloat)
		{
			byte b = data[AddIndex(4)];
			return b switch
			{
				1 => ReadClut8(inChannelCount, outChannelCount, array), 
				2 => ReadClut16(inChannelCount, outChannelCount, array), 
				_ => throw new InvalidIccProfileException($"Invalid CLUT size of {b}"), 
			};
		}
		return ReadClutF32(inChannelCount, outChannelCount, array);
	}

	public IccClut ReadClut8(int inChannelCount, int outChannelCount, byte[] gridPointCount)
	{
		int num = currentIndex;
		int num2 = 0;
		for (int i = 0; i < inChannelCount; i++)
		{
			num2 += (int)Math.Pow((int)gridPointCount[i], inChannelCount);
		}
		num2 /= inChannelCount;
		float[][] array = new float[num2][];
		for (int j = 0; j < num2; j++)
		{
			array[j] = new float[outChannelCount];
			for (int k = 0; k < outChannelCount; k++)
			{
				array[j][k] = (float)(int)data[currentIndex++] / 255f;
			}
		}
		currentIndex = num + num2 * outChannelCount;
		return new IccClut(array, gridPointCount, IccClutDataType.UInt8);
	}

	public IccClut ReadClut16(int inChannelCount, int outChannelCount, byte[] gridPointCount)
	{
		int num = currentIndex;
		int num2 = 0;
		for (int i = 0; i < inChannelCount; i++)
		{
			num2 += (int)Math.Pow((int)gridPointCount[i], inChannelCount);
		}
		num2 /= inChannelCount;
		float[][] array = new float[num2][];
		for (int j = 0; j < num2; j++)
		{
			array[j] = new float[outChannelCount];
			for (int k = 0; k < outChannelCount; k++)
			{
				array[j][k] = (float)(int)ReadUInt16() / 65535f;
			}
		}
		currentIndex = num + num2 * outChannelCount * 2;
		return new IccClut(array, gridPointCount, IccClutDataType.UInt16);
	}

	public IccClut ReadClutF32(int inChCount, int outChCount, byte[] gridPointCount)
	{
		int num = currentIndex;
		int num2 = 0;
		for (int i = 0; i < inChCount; i++)
		{
			num2 += (int)Math.Pow((int)gridPointCount[i], inChCount);
		}
		num2 /= inChCount;
		float[][] array = new float[num2][];
		for (int j = 0; j < num2; j++)
		{
			array[j] = new float[outChCount];
			for (int k = 0; k < outChCount; k++)
			{
				array[j][k] = ReadSingle();
			}
		}
		currentIndex = num + num2 * outChCount * 4;
		return new IccClut(array, gridPointCount, IccClutDataType.Float);
	}

	public float[,] ReadMatrix(int xCount, int yCount, bool isSingle)
	{
		float[,] array = new float[xCount, yCount];
		for (int i = 0; i < yCount; i++)
		{
			for (int j = 0; j < xCount; j++)
			{
				if (isSingle)
				{
					array[j, i] = ReadSingle();
				}
				else
				{
					array[j, i] = ReadFix16();
				}
			}
		}
		return array;
	}

	public float[] ReadMatrix(int yCount, bool isSingle)
	{
		float[] array = new float[yCount];
		for (int i = 0; i < yCount; i++)
		{
			if (isSingle)
			{
				array[i] = ReadSingle();
			}
			else
			{
				array[i] = ReadFix16();
			}
		}
		return array;
	}

	public IccMultiProcessElement ReadMultiProcessElement()
	{
		IccMultiProcessElementSignature iccMultiProcessElementSignature = (IccMultiProcessElementSignature)ReadUInt32();
		ushort inChannelCount = ReadUInt16();
		ushort outChannelCount = ReadUInt16();
		switch (iccMultiProcessElementSignature)
		{
		case IccMultiProcessElementSignature.CurveSet:
			return ReadCurveSetProcessElement(inChannelCount, outChannelCount);
		case IccMultiProcessElementSignature.Matrix:
			return ReadMatrixProcessElement(inChannelCount, outChannelCount);
		case IccMultiProcessElementSignature.Clut:
			return ReadClutProcessElement(inChannelCount, outChannelCount);
		case IccMultiProcessElementSignature.BAcs:
			AddIndex(8);
			return new IccBAcsProcessElement(inChannelCount, outChannelCount);
		case IccMultiProcessElementSignature.EAcs:
			AddIndex(8);
			return new IccEAcsProcessElement(inChannelCount, outChannelCount);
		default:
			throw new InvalidIccProfileException($"Invalid MultiProcessElement type of {iccMultiProcessElementSignature}");
		}
	}

	public IccCurveSetProcessElement ReadCurveSetProcessElement(int inChannelCount, int outChannelCount)
	{
		IccOneDimensionalCurve[] array = new IccOneDimensionalCurve[inChannelCount];
		for (int i = 0; i < inChannelCount; i++)
		{
			array[i] = ReadOneDimensionalCurve();
			AddPadding();
		}
		return new IccCurveSetProcessElement(array);
	}

	public IccMatrixProcessElement ReadMatrixProcessElement(int inChannelCount, int outChannelCount)
	{
		return new IccMatrixProcessElement(ReadMatrix(inChannelCount, outChannelCount, isSingle: true), ReadMatrix(outChannelCount, isSingle: true));
	}

	public IccClutProcessElement ReadClutProcessElement(int inChannelCount, int outChannelCount)
	{
		return new IccClutProcessElement(ReadClut(inChannelCount, outChannelCount, isFloat: true));
	}

	public DateTime ReadDateTime()
	{
		try
		{
			return new DateTime(ReadUInt16(), ReadUInt16(), ReadUInt16(), ReadUInt16(), ReadUInt16(), ReadUInt16(), DateTimeKind.Utc);
		}
		catch (ArgumentOutOfRangeException)
		{
			return DateTime.MinValue;
		}
	}

	public IccVersion ReadVersionNumber()
	{
		int num = ReadInt32();
		int major = (num >> 24) & 0xFF;
		int minor = (num >> 20) & 0xF;
		int patch = (num >> 16) & 0xF;
		return new IccVersion(major, minor, patch);
	}

	public Vector3 ReadXyzNumber()
	{
		return new Vector3(ReadFix16(), ReadFix16(), ReadFix16());
	}

	public IccProfileId ReadProfileId()
	{
		return new IccProfileId(ReadUInt32(), ReadUInt32(), ReadUInt32(), ReadUInt32());
	}

	public IccPositionNumber ReadPositionNumber()
	{
		return new IccPositionNumber(ReadUInt32(), ReadUInt32());
	}

	public IccResponseNumber ReadResponseNumber()
	{
		return new IccResponseNumber(ReadUInt16(), ReadFix16());
	}

	public IccNamedColor ReadNamedColor(uint deviceCoordCount)
	{
		string name = ReadAsciiString(32);
		ushort[] pcsCoordinates = new ushort[3]
		{
			ReadUInt16(),
			ReadUInt16(),
			ReadUInt16()
		};
		ushort[] array = new ushort[deviceCoordCount];
		for (int i = 0; i < deviceCoordCount; i++)
		{
			array[i] = ReadUInt16();
		}
		return new IccNamedColor(name, pcsCoordinates, array);
	}

	public IccProfileDescription ReadProfileDescription()
	{
		uint deviceManufacturer = ReadUInt32();
		uint deviceModel = ReadUInt32();
		IccDeviceAttribute deviceAttributes = (IccDeviceAttribute)ReadInt64();
		IccProfileTag technologyInformation = (IccProfileTag)ReadUInt32();
		IccMultiLocalizedUnicodeTagDataEntry iccMultiLocalizedUnicodeTagDataEntry = ReadText();
		return new IccProfileDescription(deviceModelInfo: ReadText().Texts, deviceManufacturer: deviceManufacturer, deviceModel: deviceModel, deviceAttributes: deviceAttributes, technologyInformation: technologyInformation, deviceManufacturerInfo: iccMultiLocalizedUnicodeTagDataEntry.Texts);
		IccMultiLocalizedUnicodeTagDataEntry ReadText()
		{
			return ReadTagDataEntryHeader() switch
			{
				IccTypeSignature.MultiLocalizedUnicode => ReadMultiLocalizedUnicodeTagDataEntry(), 
				IccTypeSignature.TextDescription => (IccMultiLocalizedUnicodeTagDataEntry)ReadTextDescriptionTagDataEntry(), 
				_ => throw new InvalidIccProfileException("Profile description can only have multi-localized Unicode or text description entries"), 
			};
		}
	}

	public IccColorantTableEntry ReadColorantTableEntry()
	{
		return new IccColorantTableEntry(ReadAsciiString(32), ReadUInt16(), ReadUInt16(), ReadUInt16());
	}

	public IccScreeningChannel ReadScreeningChannel()
	{
		return new IccScreeningChannel(ReadFix16(), ReadFix16(), (IccScreeningSpotType)ReadInt32());
	}

	public ushort ReadUInt16()
	{
		return BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(AddIndex(2), 2));
	}

	public short ReadInt16()
	{
		return BinaryPrimitives.ReadInt16BigEndian(data.AsSpan(AddIndex(2), 2));
	}

	public uint ReadUInt32()
	{
		return BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(AddIndex(4), 4));
	}

	public int ReadInt32()
	{
		return BinaryPrimitives.ReadInt32BigEndian(data.AsSpan(AddIndex(4), 4));
	}

	public ulong ReadUInt64()
	{
		return BinaryPrimitives.ReadUInt64BigEndian(data.AsSpan(AddIndex(8), 8));
	}

	public long ReadInt64()
	{
		return BinaryPrimitives.ReadInt64BigEndian(data.AsSpan(AddIndex(8), 8));
	}

	public float ReadSingle()
	{
		int source = ReadInt32();
		return Unsafe.As<int, float>(ref source);
	}

	public double ReadDouble()
	{
		long source = ReadInt64();
		return Unsafe.As<long, double>(ref source);
	}

	public string ReadAsciiString(int length)
	{
		if (length == 0)
		{
			return string.Empty;
		}
		Guard.MustBeGreaterThan(length, 0, "length");
		string text = Encoding.ASCII.GetString(data, AddIndex(length), length);
		int num = text.IndexOf('\0');
		if (num >= 0)
		{
			text = text.Substring(0, num);
		}
		return text;
	}

	public string ReadUnicodeString(int length)
	{
		if (length == 0)
		{
			return string.Empty;
		}
		Guard.MustBeGreaterThan(length, 0, "length");
		return Encoding.BigEndianUnicode.GetString(data, AddIndex(length), length);
	}

	public float ReadFix16()
	{
		return (float)ReadInt32() / 65536f;
	}

	public float ReadUFix16()
	{
		return (float)ReadUInt32() / 65536f;
	}

	public float ReadU1Fix15()
	{
		return (float)(int)ReadUInt16() / 32768f;
	}

	public float ReadUFix8()
	{
		return (float)(int)ReadUInt16() / 256f;
	}

	public byte[] ReadBytes(int count)
	{
		byte[] array = new byte[count];
		Buffer.BlockCopy(data, AddIndex(count), array, 0, count);
		return array;
	}

	public IccTagDataEntry ReadTagDataEntry(IccTagTableEntry info)
	{
		currentIndex = (int)info.Offset;
		return ReadTagDataEntryHeader() switch
		{
			IccTypeSignature.Chromaticity => ReadChromaticityTagDataEntry(), 
			IccTypeSignature.ColorantOrder => ReadColorantOrderTagDataEntry(), 
			IccTypeSignature.ColorantTable => ReadColorantTableTagDataEntry(), 
			IccTypeSignature.Curve => ReadCurveTagDataEntry(), 
			IccTypeSignature.Data => ReadDataTagDataEntry(info.DataSize), 
			IccTypeSignature.DateTime => ReadDateTimeTagDataEntry(), 
			IccTypeSignature.Lut16 => ReadLut16TagDataEntry(), 
			IccTypeSignature.Lut8 => ReadLut8TagDataEntry(), 
			IccTypeSignature.LutAToB => ReadLutAtoBTagDataEntry(), 
			IccTypeSignature.LutBToA => ReadLutBtoATagDataEntry(), 
			IccTypeSignature.Measurement => ReadMeasurementTagDataEntry(), 
			IccTypeSignature.MultiLocalizedUnicode => ReadMultiLocalizedUnicodeTagDataEntry(), 
			IccTypeSignature.MultiProcessElements => ReadMultiProcessElementsTagDataEntry(), 
			IccTypeSignature.NamedColor2 => ReadNamedColor2TagDataEntry(), 
			IccTypeSignature.ParametricCurve => ReadParametricCurveTagDataEntry(), 
			IccTypeSignature.ProfileSequenceDesc => ReadProfileSequenceDescTagDataEntry(), 
			IccTypeSignature.ProfileSequenceIdentifier => ReadProfileSequenceIdentifierTagDataEntry(), 
			IccTypeSignature.ResponseCurveSet16 => ReadResponseCurveSet16TagDataEntry(), 
			IccTypeSignature.S15Fixed16Array => ReadFix16ArrayTagDataEntry(info.DataSize), 
			IccTypeSignature.Signature => ReadSignatureTagDataEntry(), 
			IccTypeSignature.Text => ReadTextTagDataEntry(info.DataSize), 
			IccTypeSignature.U16Fixed16Array => ReadUFix16ArrayTagDataEntry(info.DataSize), 
			IccTypeSignature.UInt16Array => ReadUInt16ArrayTagDataEntry(info.DataSize), 
			IccTypeSignature.UInt32Array => ReadUInt32ArrayTagDataEntry(info.DataSize), 
			IccTypeSignature.UInt64Array => ReadUInt64ArrayTagDataEntry(info.DataSize), 
			IccTypeSignature.UInt8Array => ReadUInt8ArrayTagDataEntry(info.DataSize), 
			IccTypeSignature.ViewingConditions => ReadViewingConditionsTagDataEntry(), 
			IccTypeSignature.Xyz => ReadXyzTagDataEntry(info.DataSize), 
			IccTypeSignature.TextDescription => ReadTextDescriptionTagDataEntry(), 
			IccTypeSignature.CrdInfo => ReadCrdInfoTagDataEntry(), 
			IccTypeSignature.Screening => ReadScreeningTagDataEntry(), 
			IccTypeSignature.UcrBg => ReadUcrBgTagDataEntry(info.DataSize), 
			_ => ReadUnknownTagDataEntry(info.DataSize), 
		};
	}

	public IccTypeSignature ReadTagDataEntryHeader()
	{
		uint result = ReadUInt32();
		AddIndex(4);
		return (IccTypeSignature)result;
	}

	public void ReadCheckTagDataEntryHeader(IccTypeSignature expected)
	{
		IccTypeSignature iccTypeSignature = ReadTagDataEntryHeader();
		if (expected != (IccTypeSignature)4294967295u && iccTypeSignature != expected)
		{
			throw new InvalidIccProfileException($"Read signature {iccTypeSignature} is not the expected {expected}");
		}
	}

	public IccUnknownTagDataEntry ReadUnknownTagDataEntry(uint size)
	{
		int count = (int)(size - 8);
		return new IccUnknownTagDataEntry(ReadBytes(count));
	}

	public IccChromaticityTagDataEntry ReadChromaticityTagDataEntry()
	{
		ushort num = ReadUInt16();
		IccColorantEncoding iccColorantEncoding = (IccColorantEncoding)ReadUInt16();
		if (Enum.IsDefined(typeof(IccColorantEncoding), iccColorantEncoding) && iccColorantEncoding != 0)
		{
			return new IccChromaticityTagDataEntry(iccColorantEncoding);
		}
		double[][] array = new double[num][];
		for (int i = 0; i < num; i++)
		{
			array[i] = new double[2]
			{
				ReadUFix16(),
				ReadUFix16()
			};
		}
		return new IccChromaticityTagDataEntry(array);
	}

	public IccColorantOrderTagDataEntry ReadColorantOrderTagDataEntry()
	{
		uint count = ReadUInt32();
		return new IccColorantOrderTagDataEntry(ReadBytes((int)count));
	}

	public IccColorantTableTagDataEntry ReadColorantTableTagDataEntry()
	{
		uint num = ReadUInt32();
		IccColorantTableEntry[] array = new IccColorantTableEntry[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadColorantTableEntry();
		}
		return new IccColorantTableTagDataEntry(array);
	}

	public IccCurveTagDataEntry ReadCurveTagDataEntry()
	{
		uint num = ReadUInt32();
		switch (num)
		{
		case 0u:
			return new IccCurveTagDataEntry();
		case 1u:
			return new IccCurveTagDataEntry(ReadUFix8());
		default:
		{
			float[] array = new float[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = (float)(int)ReadUInt16() / 65535f;
			}
			return new IccCurveTagDataEntry(array);
		}
		}
	}

	public IccDataTagDataEntry ReadDataTagDataEntry(uint size)
	{
		AddIndex(3);
		bool bit = GetBit(data[AddIndex(1)], 7);
		int count = (int)(size - 12);
		return new IccDataTagDataEntry(ReadBytes(count), bit);
	}

	public IccDateTimeTagDataEntry ReadDateTimeTagDataEntry()
	{
		return new IccDateTimeTagDataEntry(ReadDateTime());
	}

	public IccLut16TagDataEntry ReadLut16TagDataEntry()
	{
		byte b = data[AddIndex(1)];
		byte b2 = data[AddIndex(1)];
		byte b3 = data[AddIndex(1)];
		AddIndex(1);
		float[,] matrix = ReadMatrix(3, 3, isSingle: false);
		ushort count = ReadUInt16();
		ushort count2 = ReadUInt16();
		IccLut[] array = new IccLut[b];
		byte[] array2 = new byte[b];
		for (int i = 0; i < b; i++)
		{
			array[i] = ReadLut16(count);
			array2[i] = b3;
		}
		IccClut clutValues = ReadClut16(b, b2, array2);
		IccLut[] array3 = new IccLut[b2];
		for (int j = 0; j < b2; j++)
		{
			array3[j] = ReadLut16(count2);
		}
		return new IccLut16TagDataEntry(matrix, array, clutValues, array3);
	}

	public IccLut8TagDataEntry ReadLut8TagDataEntry()
	{
		byte b = data[AddIndex(1)];
		byte b2 = data[AddIndex(1)];
		byte b3 = data[AddIndex(1)];
		AddIndex(1);
		float[,] matrix = ReadMatrix(3, 3, isSingle: false);
		IccLut[] array = new IccLut[b];
		byte[] array2 = new byte[b];
		for (int i = 0; i < b; i++)
		{
			array[i] = ReadLut8();
			array2[i] = b3;
		}
		IccClut clutValues = ReadClut8(b, b2, array2);
		IccLut[] array3 = new IccLut[b2];
		for (int j = 0; j < b2; j++)
		{
			array3[j] = ReadLut8();
		}
		return new IccLut8TagDataEntry(matrix, array, clutValues, array3);
	}

	public IccLutAToBTagDataEntry ReadLutAtoBTagDataEntry()
	{
		int num = currentIndex - 8;
		byte b = data[AddIndex(1)];
		byte b2 = data[AddIndex(1)];
		AddIndex(2);
		uint num2 = ReadUInt32();
		uint num3 = ReadUInt32();
		uint num4 = ReadUInt32();
		uint num5 = ReadUInt32();
		uint num6 = ReadUInt32();
		IccTagDataEntry[] curveB = null;
		IccTagDataEntry[] curveM = null;
		IccTagDataEntry[] curveA = null;
		IccClut clutValues = null;
		float[,] matrix3x = null;
		float[] matrix3x2 = null;
		if (num2 != 0)
		{
			currentIndex = (int)num2 + num;
			curveB = ReadCurves(b2);
		}
		if (num4 != 0)
		{
			currentIndex = (int)num4 + num;
			curveM = ReadCurves(b2);
		}
		if (num6 != 0)
		{
			currentIndex = (int)num6 + num;
			curveA = ReadCurves(b);
		}
		if (num5 != 0)
		{
			currentIndex = (int)num5 + num;
			clutValues = ReadClut(b, b2, isFloat: false);
		}
		if (num3 != 0)
		{
			currentIndex = (int)num3 + num;
			matrix3x = ReadMatrix(3, 3, isSingle: false);
			matrix3x2 = ReadMatrix(3, isSingle: false);
		}
		return new IccLutAToBTagDataEntry(curveB, matrix3x, matrix3x2, curveM, clutValues, curveA);
	}

	public IccLutBToATagDataEntry ReadLutBtoATagDataEntry()
	{
		int num = currentIndex - 8;
		byte b = data[AddIndex(1)];
		byte b2 = data[AddIndex(1)];
		AddIndex(2);
		uint num2 = ReadUInt32();
		uint num3 = ReadUInt32();
		uint num4 = ReadUInt32();
		uint num5 = ReadUInt32();
		uint num6 = ReadUInt32();
		IccTagDataEntry[] curveB = null;
		IccTagDataEntry[] curveM = null;
		IccTagDataEntry[] curveA = null;
		IccClut clutValues = null;
		float[,] matrix3x = null;
		float[] matrix3x2 = null;
		if (num2 != 0)
		{
			currentIndex = (int)num2 + num;
			curveB = ReadCurves(b);
		}
		if (num4 != 0)
		{
			currentIndex = (int)num4 + num;
			curveM = ReadCurves(b);
		}
		if (num6 != 0)
		{
			currentIndex = (int)num6 + num;
			curveA = ReadCurves(b2);
		}
		if (num5 != 0)
		{
			currentIndex = (int)num5 + num;
			clutValues = ReadClut(b, b2, isFloat: false);
		}
		if (num3 != 0)
		{
			currentIndex = (int)num3 + num;
			matrix3x = ReadMatrix(3, 3, isSingle: false);
			matrix3x2 = ReadMatrix(3, isSingle: false);
		}
		return new IccLutBToATagDataEntry(curveB, matrix3x, matrix3x2, curveM, clutValues, curveA);
	}

	public IccMeasurementTagDataEntry ReadMeasurementTagDataEntry()
	{
		return new IccMeasurementTagDataEntry((IccStandardObserver)ReadUInt32(), ReadXyzNumber(), (IccMeasurementGeometry)ReadUInt32(), ReadUFix16(), (IccStandardIlluminant)ReadUInt32());
	}

	public IccMultiLocalizedUnicodeTagDataEntry ReadMultiLocalizedUnicodeTagDataEntry()
	{
		int num = currentIndex - 8;
		uint num2 = ReadUInt32();
		ReadUInt32();
		IccLocalizedString[] array = new IccLocalizedString[num2];
		CultureInfo[] array2 = new CultureInfo[num2];
		uint[] array3 = new uint[num2];
		uint[] array4 = new uint[num2];
		for (int i = 0; i < num2; i++)
		{
			string language2 = ReadAsciiString(2);
			string country2 = ReadAsciiString(2);
			array2[i] = ReadCulture(language2, country2);
			array3[i] = ReadUInt32();
			array4[i] = ReadUInt32();
		}
		for (int j = 0; j < num2; j++)
		{
			currentIndex = (int)(num + array4[j]);
			array[j] = new IccLocalizedString(array2[j], ReadUnicodeString((int)array3[j]));
		}
		return new IccMultiLocalizedUnicodeTagDataEntry(array);
		static CultureInfo ReadCulture(string language, string country)
		{
			if (string.IsNullOrWhiteSpace(language))
			{
				return CultureInfo.InvariantCulture;
			}
			if (string.IsNullOrWhiteSpace(country))
			{
				try
				{
					return new CultureInfo(language);
				}
				catch (CultureNotFoundException)
				{
					return CultureInfo.InvariantCulture;
				}
			}
			try
			{
				return new CultureInfo(language + "-" + country);
			}
			catch (CultureNotFoundException)
			{
				return ReadCulture(language, null);
			}
		}
	}

	public IccMultiProcessElementsTagDataEntry ReadMultiProcessElementsTagDataEntry()
	{
		int num = currentIndex - 8;
		ReadUInt16();
		ReadUInt16();
		uint num2 = ReadUInt32();
		IccPositionNumber[] array = new IccPositionNumber[num2];
		for (int i = 0; i < num2; i++)
		{
			array[i] = ReadPositionNumber();
		}
		IccMultiProcessElement[] array2 = new IccMultiProcessElement[num2];
		for (int j = 0; j < num2; j++)
		{
			currentIndex = (int)array[j].Offset + num;
			array2[j] = ReadMultiProcessElement();
		}
		return new IccMultiProcessElementsTagDataEntry(array2);
	}

	public IccNamedColor2TagDataEntry ReadNamedColor2TagDataEntry()
	{
		int vendorFlags = ReadInt32();
		uint num = ReadUInt32();
		uint deviceCoordCount = ReadUInt32();
		string prefix = ReadAsciiString(32);
		string suffix = ReadAsciiString(32);
		IccNamedColor[] array = new IccNamedColor[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadNamedColor(deviceCoordCount);
		}
		return new IccNamedColor2TagDataEntry(vendorFlags, prefix, suffix, array);
	}

	public IccParametricCurveTagDataEntry ReadParametricCurveTagDataEntry()
	{
		return new IccParametricCurveTagDataEntry(ReadParametricCurve());
	}

	public IccProfileSequenceDescTagDataEntry ReadProfileSequenceDescTagDataEntry()
	{
		uint num = ReadUInt32();
		IccProfileDescription[] array = new IccProfileDescription[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadProfileDescription();
		}
		return new IccProfileSequenceDescTagDataEntry(array);
	}

	public IccProfileSequenceIdentifierTagDataEntry ReadProfileSequenceIdentifierTagDataEntry()
	{
		int num = currentIndex - 8;
		uint num2 = ReadUInt32();
		IccPositionNumber[] array = new IccPositionNumber[num2];
		for (int i = 0; i < num2; i++)
		{
			array[i] = ReadPositionNumber();
		}
		IccProfileSequenceIdentifier[] array2 = new IccProfileSequenceIdentifier[num2];
		for (int j = 0; j < num2; j++)
		{
			currentIndex = (int)(num + array[j].Offset);
			IccProfileId id = ReadProfileId();
			ReadCheckTagDataEntryHeader(IccTypeSignature.MultiLocalizedUnicode);
			IccMultiLocalizedUnicodeTagDataEntry iccMultiLocalizedUnicodeTagDataEntry = ReadMultiLocalizedUnicodeTagDataEntry();
			array2[j] = new IccProfileSequenceIdentifier(id, iccMultiLocalizedUnicodeTagDataEntry.Texts);
		}
		return new IccProfileSequenceIdentifierTagDataEntry(array2);
	}

	public IccResponseCurveSet16TagDataEntry ReadResponseCurveSet16TagDataEntry()
	{
		int num = currentIndex - 8;
		ushort channelCount = ReadUInt16();
		ushort num2 = ReadUInt16();
		uint[] array = new uint[num2];
		for (int i = 0; i < num2; i++)
		{
			array[i] = ReadUInt32();
		}
		IccResponseCurve[] array2 = new IccResponseCurve[num2];
		for (int j = 0; j < num2; j++)
		{
			currentIndex = (int)(num + array[j]);
			array2[j] = ReadResponseCurve(channelCount);
		}
		return new IccResponseCurveSet16TagDataEntry(array2);
	}

	public IccFix16ArrayTagDataEntry ReadFix16ArrayTagDataEntry(uint size)
	{
		uint num = (size - 8) / 4;
		float[] array = new float[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadFix16() / 256f;
		}
		return new IccFix16ArrayTagDataEntry(array);
	}

	public IccSignatureTagDataEntry ReadSignatureTagDataEntry()
	{
		return new IccSignatureTagDataEntry(ReadAsciiString(4));
	}

	public IccTextTagDataEntry ReadTextTagDataEntry(uint size)
	{
		return new IccTextTagDataEntry(ReadAsciiString((int)(size - 8)));
	}

	public IccUFix16ArrayTagDataEntry ReadUFix16ArrayTagDataEntry(uint size)
	{
		uint num = (size - 8) / 4;
		float[] array = new float[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadUFix16();
		}
		return new IccUFix16ArrayTagDataEntry(array);
	}

	public IccUInt16ArrayTagDataEntry ReadUInt16ArrayTagDataEntry(uint size)
	{
		uint num = (size - 8) / 2;
		ushort[] array = new ushort[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadUInt16();
		}
		return new IccUInt16ArrayTagDataEntry(array);
	}

	public IccUInt32ArrayTagDataEntry ReadUInt32ArrayTagDataEntry(uint size)
	{
		uint num = (size - 8) / 4;
		uint[] array = new uint[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadUInt32();
		}
		return new IccUInt32ArrayTagDataEntry(array);
	}

	public IccUInt64ArrayTagDataEntry ReadUInt64ArrayTagDataEntry(uint size)
	{
		uint num = (size - 8) / 8;
		ulong[] array = new ulong[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadUInt64();
		}
		return new IccUInt64ArrayTagDataEntry(array);
	}

	public IccUInt8ArrayTagDataEntry ReadUInt8ArrayTagDataEntry(uint size)
	{
		int count = (int)(size - 8);
		return new IccUInt8ArrayTagDataEntry(ReadBytes(count));
	}

	public IccViewingConditionsTagDataEntry ReadViewingConditionsTagDataEntry()
	{
		return new IccViewingConditionsTagDataEntry(ReadXyzNumber(), ReadXyzNumber(), (IccStandardIlluminant)ReadUInt32());
	}

	public IccXyzTagDataEntry ReadXyzTagDataEntry(uint size)
	{
		uint num = (size - 8) / 12;
		Vector3[] array = new Vector3[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = ReadXyzNumber();
		}
		return new IccXyzTagDataEntry(array);
	}

	public IccTextDescriptionTagDataEntry ReadTextDescriptionTagDataEntry()
	{
		string unicode;
		string scriptCode;
		string ascii = (unicode = (scriptCode = null));
		int num = (int)ReadUInt32();
		if (num > 0)
		{
			ascii = ReadAsciiString(num - 1);
			AddIndex(1);
		}
		uint unicodeLanguageCode = ReadUInt32();
		int num2 = (int)ReadUInt32();
		if (num2 > 0)
		{
			unicode = ReadUnicodeString(num2 * 2 - 2);
			AddIndex(2);
		}
		ushort scriptCodeCode = ReadUInt16();
		int num3 = Math.Min(data[AddIndex(1)], (byte)67);
		if (num3 > 0)
		{
			scriptCode = ReadAsciiString(num3 - 1);
			AddIndex(1);
		}
		return new IccTextDescriptionTagDataEntry(ascii, unicode, scriptCode, unicodeLanguageCode, scriptCodeCode);
	}

	public IccCrdInfoTagDataEntry ReadCrdInfoTagDataEntry()
	{
		uint length = ReadUInt32();
		string postScriptProductName = ReadAsciiString((int)length);
		uint length2 = ReadUInt32();
		string renderingIntent0Crd = ReadAsciiString((int)length2);
		uint length3 = ReadUInt32();
		string renderingIntent1Crd = ReadAsciiString((int)length3);
		uint length4 = ReadUInt32();
		string renderingIntent2Crd = ReadAsciiString((int)length4);
		uint length5 = ReadUInt32();
		string renderingIntent3Crd = ReadAsciiString((int)length5);
		return new IccCrdInfoTagDataEntry(postScriptProductName, renderingIntent0Crd, renderingIntent1Crd, renderingIntent2Crd, renderingIntent3Crd);
	}

	public IccScreeningTagDataEntry ReadScreeningTagDataEntry()
	{
		IccScreeningFlag flags = (IccScreeningFlag)ReadInt32();
		IccScreeningChannel[] array = new IccScreeningChannel[ReadUInt32()];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = ReadScreeningChannel();
		}
		return new IccScreeningTagDataEntry(flags, array);
	}

	public IccUcrBgTagDataEntry ReadUcrBgTagDataEntry(uint size)
	{
		uint num = ReadUInt32();
		ushort[] array = new ushort[num];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = ReadUInt16();
		}
		uint num2 = ReadUInt32();
		ushort[] array2 = new ushort[num2];
		for (int j = 0; j < array2.Length; j++)
		{
			array2[j] = ReadUInt16();
		}
		uint num3 = (num + num2) * 2 + 8;
		int length = (int)(size - 8 - num3);
		string description = ReadAsciiString(length);
		return new IccUcrBgTagDataEntry(array, array2, description);
	}
}
