using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccDataWriter : IDisposable
{
	private readonly MemoryStream dataStream;

	private bool isDisposed;

	public uint Length => (uint)dataStream.Length;

	public IccDataWriter()
	{
		dataStream = new MemoryStream();
	}

	public byte[] GetData()
	{
		return dataStream.ToArray();
	}

	public void SetIndex(int index)
	{
		dataStream.Position = index;
	}

	public int WriteArray(byte[] data)
	{
		dataStream.Write(data, 0, data.Length);
		return data.Length;
	}

	public int WriteArray(ushort[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			WriteUInt16(data[i]);
		}
		return data.Length * 2;
	}

	public int WriteArray(short[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			WriteInt16(data[i]);
		}
		return data.Length * 2;
	}

	public int WriteArray(uint[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			WriteUInt32(data[i]);
		}
		return data.Length * 4;
	}

	public int WriteArray(int[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			WriteInt32(data[i]);
		}
		return data.Length * 4;
	}

	public int WriteArray(ulong[] data)
	{
		for (int i = 0; i < data.Length; i++)
		{
			WriteUInt64(data[i]);
		}
		return data.Length * 8;
	}

	public int WriteEmpty(int length)
	{
		for (int i = 0; i < length; i++)
		{
			dataStream.WriteByte(0);
		}
		return length;
	}

	public int WritePadding()
	{
		int num = 4 - (int)dataStream.Position % 4;
		return WriteEmpty((num < 4) ? num : 0);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	private unsafe int WriteBytes(byte* data, int length)
	{
		if (BitConverter.IsLittleEndian)
		{
			for (int num = length - 1; num >= 0; num--)
			{
				dataStream.WriteByte(data[num]);
			}
		}
		else
		{
			WriteBytesDirect(data, length);
		}
		return length;
	}

	private unsafe int WriteBytesDirect(byte* data, int length)
	{
		for (int i = 0; i < length; i++)
		{
			dataStream.WriteByte(data[i]);
		}
		return length;
	}

	private int WriteCurves(IccTagDataEntry[] curves)
	{
		int num = 0;
		foreach (IccTagDataEntry iccTagDataEntry in curves)
		{
			if (iccTagDataEntry.Signature != IccTypeSignature.Curve && iccTagDataEntry.Signature != IccTypeSignature.ParametricCurve)
			{
				throw new InvalidIccProfileException("Curve has to be either \"IccTypeSignature.Curve\" or \"IccTypeSignature.ParametricCurve\" for LutAToB- and LutBToA-TagDataEntries");
			}
			num += WriteTagDataEntry(iccTagDataEntry);
			num += WritePadding();
		}
		return num;
	}

	private void Dispose(bool disposing)
	{
		if (!isDisposed)
		{
			if (disposing)
			{
				dataStream?.Dispose();
			}
			isDisposed = true;
		}
	}

	public int WriteOneDimensionalCurve(IccOneDimensionalCurve value)
	{
		int num = WriteUInt16((ushort)value.Segments.Length);
		num += WriteEmpty(2);
		float[] breakPoints = value.BreakPoints;
		foreach (float value2 in breakPoints)
		{
			num += WriteSingle(value2);
		}
		IccCurveSegment[] segments = value.Segments;
		foreach (IccCurveSegment value3 in segments)
		{
			num += WriteCurveSegment(value3);
		}
		return num;
	}

	public int WriteResponseCurve(IccResponseCurve value)
	{
		int num = WriteUInt32((uint)value.CurveType);
		IccResponseNumber[][] responseArrays = value.ResponseArrays;
		foreach (IccResponseNumber[] array in responseArrays)
		{
			num += WriteUInt32((uint)array.Length);
		}
		Vector3[] xyzValues = value.XyzValues;
		foreach (Vector3 value2 in xyzValues)
		{
			num += WriteXyzNumber(value2);
		}
		responseArrays = value.ResponseArrays;
		foreach (IccResponseNumber[] array2 in responseArrays)
		{
			for (int j = 0; j < array2.Length; j++)
			{
				IccResponseNumber value3 = array2[j];
				num += WriteResponseNumber(in value3);
			}
		}
		return num;
	}

	public int WriteParametricCurve(IccParametricCurve value)
	{
		ushort type = (ushort)value.Type;
		int num = WriteUInt16(type);
		num += WriteEmpty(2);
		if (type <= 4)
		{
			num += WriteFix16(value.G);
		}
		if (type > 0 && type <= 4)
		{
			num += WriteFix16(value.A);
			num += WriteFix16(value.B);
		}
		if (type > 1 && type <= 4)
		{
			num += WriteFix16(value.C);
		}
		if (type > 2 && type <= 4)
		{
			num += WriteFix16(value.D);
		}
		if (type == 4)
		{
			num += WriteFix16(value.E);
			num += WriteFix16(value.F);
		}
		return num;
	}

	public int WriteCurveSegment(IccCurveSegment value)
	{
		int num = WriteUInt32((uint)value.Signature);
		num += WriteEmpty(4);
		return value.Signature switch
		{
			IccCurveSegmentSignature.FormulaCurve => num + WriteFormulaCurveElement((IccFormulaCurveElement)value), 
			IccCurveSegmentSignature.SampledCurve => num + WriteSampledCurveElement((IccSampledCurveElement)value), 
			_ => throw new InvalidIccProfileException($"Invalid CurveSegment type of {value.Signature}"), 
		};
	}

	public int WriteFormulaCurveElement(IccFormulaCurveElement value)
	{
		int num = WriteUInt16((ushort)value.Type);
		num += WriteEmpty(2);
		if (value.Type == IccFormulaCurveType.Type1 || value.Type == IccFormulaCurveType.Type2)
		{
			num += WriteSingle(value.Gamma);
		}
		num += WriteSingle(value.A);
		num += WriteSingle(value.B);
		num += WriteSingle(value.C);
		if (value.Type == IccFormulaCurveType.Type2 || value.Type == IccFormulaCurveType.Type3)
		{
			num += WriteSingle(value.D);
		}
		if (value.Type == IccFormulaCurveType.Type3)
		{
			num += WriteSingle(value.E);
		}
		return num;
	}

	public int WriteSampledCurveElement(IccSampledCurveElement value)
	{
		int num = WriteUInt32((uint)value.CurveEntries.Length);
		float[] curveEntries = value.CurveEntries;
		foreach (float value2 in curveEntries)
		{
			num += WriteSingle(value2);
		}
		return num;
	}

	public int WriteLut8(IccLut value)
	{
		float[] values = value.Values;
		foreach (float num in values)
		{
			WriteByte((byte)Numerics.Clamp(num * 255f + 0.5f, 0f, 255f));
		}
		return value.Values.Length;
	}

	public int WriteLut16(IccLut value)
	{
		float[] values = value.Values;
		foreach (float num in values)
		{
			WriteUInt16((ushort)Numerics.Clamp(num * 65535f + 0.5f, 0f, 65535f));
		}
		return value.Values.Length * 2;
	}

	public int WriteClut(IccClut value)
	{
		int num = WriteArray(value.GridPointCount);
		num += WriteEmpty(16 - value.GridPointCount.Length);
		switch (value.DataType)
		{
		case IccClutDataType.Float:
			return num + WriteClutF32(value);
		case IccClutDataType.UInt8:
			num += WriteByte(1);
			num += WriteEmpty(3);
			return num + WriteClut8(value);
		case IccClutDataType.UInt16:
			num += WriteByte(2);
			num += WriteEmpty(3);
			return num + WriteClut16(value);
		default:
			throw new InvalidIccProfileException($"Invalid CLUT data type of {value.DataType}");
		}
	}

	public int WriteClut8(IccClut value)
	{
		int num = 0;
		float[][] values = value.Values;
		foreach (float[] array in values)
		{
			foreach (float num2 in array)
			{
				num += WriteByte((byte)Numerics.Clamp(num2 * 255f + 0.5f, 0f, 255f));
			}
		}
		return num;
	}

	public int WriteClut16(IccClut value)
	{
		int num = 0;
		float[][] values = value.Values;
		foreach (float[] array in values)
		{
			foreach (float num2 in array)
			{
				num += WriteUInt16((ushort)Numerics.Clamp(num2 * 65535f + 0.5f, 0f, 65535f));
			}
		}
		return num;
	}

	public int WriteClutF32(IccClut value)
	{
		int num = 0;
		float[][] values = value.Values;
		foreach (float[] array in values)
		{
			foreach (float value2 in array)
			{
				num += WriteSingle(value2);
			}
		}
		return num;
	}

	public int WriteMatrix(Matrix4x4 value, bool isSingle)
	{
		int num = 0;
		if (isSingle)
		{
			num += WriteSingle(value.M11);
			num += WriteSingle(value.M21);
			num += WriteSingle(value.M31);
			num += WriteSingle(value.M12);
			num += WriteSingle(value.M22);
			num += WriteSingle(value.M32);
			num += WriteSingle(value.M13);
			num += WriteSingle(value.M23);
			return num + WriteSingle(value.M33);
		}
		num += WriteFix16(value.M11);
		num += WriteFix16(value.M21);
		num += WriteFix16(value.M31);
		num += WriteFix16(value.M12);
		num += WriteFix16(value.M22);
		num += WriteFix16(value.M32);
		num += WriteFix16(value.M13);
		num += WriteFix16(value.M23);
		return num + WriteFix16(value.M33);
	}

	public int WriteMatrix(in DenseMatrix<float> value, bool isSingle)
	{
		int num = 0;
		for (int i = 0; i < value.Rows; i++)
		{
			for (int j = 0; j < value.Columns; j++)
			{
				num = ((!isSingle) ? (num + WriteFix16(value[j, i])) : (num + WriteSingle(value[j, i])));
			}
		}
		return num;
	}

	public int WriteMatrix(float[,] value, bool isSingle)
	{
		int num = 0;
		for (int i = 0; i < value.GetLength(1); i++)
		{
			for (int j = 0; j < value.GetLength(0); j++)
			{
				num = ((!isSingle) ? (num + WriteFix16(value[j, i])) : (num + WriteSingle(value[j, i])));
			}
		}
		return num;
	}

	public int WriteMatrix(Vector3 value, bool isSingle)
	{
		int num = 0;
		if (isSingle)
		{
			num += WriteSingle(value.X);
			num += WriteSingle(value.Y);
			return num + WriteSingle(value.Z);
		}
		num += WriteFix16(value.X);
		num += WriteFix16(value.Y);
		return num + WriteFix16(value.Z);
	}

	public int WriteMatrix(float[] value, bool isSingle)
	{
		int num = 0;
		for (int i = 0; i < value.Length; i++)
		{
			num = ((!isSingle) ? (num + WriteFix16(value[i])) : (num + WriteSingle(value[i])));
		}
		return num;
	}

	public int WriteMultiProcessElement(IccMultiProcessElement value)
	{
		int num = WriteUInt32((uint)value.Signature);
		num += WriteUInt16((ushort)value.InputChannelCount);
		num += WriteUInt16((ushort)value.OutputChannelCount);
		switch (value.Signature)
		{
		case IccMultiProcessElementSignature.CurveSet:
			return num + WriteCurveSetProcessElement((IccCurveSetProcessElement)value);
		case IccMultiProcessElementSignature.Matrix:
			return num + WriteMatrixProcessElement((IccMatrixProcessElement)value);
		case IccMultiProcessElementSignature.Clut:
			return num + WriteClutProcessElement((IccClutProcessElement)value);
		case IccMultiProcessElementSignature.BAcs:
		case IccMultiProcessElementSignature.EAcs:
			return num + WriteEmpty(8);
		default:
			throw new InvalidIccProfileException($"Invalid MultiProcessElement type of {value.Signature}");
		}
	}

	public int WriteCurveSetProcessElement(IccCurveSetProcessElement value)
	{
		int num = 0;
		IccOneDimensionalCurve[] curves = value.Curves;
		foreach (IccOneDimensionalCurve value2 in curves)
		{
			num += WriteOneDimensionalCurve(value2);
			num += WritePadding();
		}
		return num;
	}

	public int WriteMatrixProcessElement(IccMatrixProcessElement value)
	{
		return WriteMatrix(value.MatrixIxO, isSingle: true) + WriteMatrix(value.MatrixOx1, isSingle: true);
	}

	public int WriteClutProcessElement(IccClutProcessElement value)
	{
		return WriteClut(value.ClutValue);
	}

	public int WriteDateTime(DateTime value)
	{
		return WriteUInt16((ushort)value.Year) + WriteUInt16((ushort)value.Month) + WriteUInt16((ushort)value.Day) + WriteUInt16((ushort)value.Hour) + WriteUInt16((ushort)value.Minute) + WriteUInt16((ushort)value.Second);
	}

	public int WriteVersionNumber(in IccVersion value)
	{
		int num = Numerics.Clamp(value.Major, 0, 255);
		int num2 = Numerics.Clamp(value.Minor, 0, 15);
		int num3 = Numerics.Clamp(value.Patch, 0, 15);
		int value2 = (num << 24) | (num2 << 20) | (num3 << 16);
		return WriteInt32(value2);
	}

	public int WriteXyzNumber(Vector3 value)
	{
		return WriteFix16(value.X) + WriteFix16(value.Y) + WriteFix16(value.Z);
	}

	public int WriteProfileId(in IccProfileId value)
	{
		return WriteUInt32(value.Part1) + WriteUInt32(value.Part2) + WriteUInt32(value.Part3) + WriteUInt32(value.Part4);
	}

	public int WritePositionNumber(in IccPositionNumber value)
	{
		return WriteUInt32(value.Offset) + WriteUInt32(value.Size);
	}

	public int WriteResponseNumber(in IccResponseNumber value)
	{
		return WriteUInt16(value.DeviceCode) + WriteFix16(value.MeasurementValue);
	}

	public int WriteNamedColor(in IccNamedColor value)
	{
		return WriteAsciiString(value.Name, 32, ensureNullTerminator: true) + WriteArray(value.PcsCoordinates) + WriteArray(value.DeviceCoordinates);
	}

	public int WriteProfileDescription(in IccProfileDescription value)
	{
		return WriteUInt32(value.DeviceManufacturer) + WriteUInt32(value.DeviceModel) + WriteInt64((long)value.DeviceAttributes) + WriteUInt32((uint)value.TechnologyInformation) + WriteTagDataEntryHeader(IccTypeSignature.MultiLocalizedUnicode) + WriteMultiLocalizedUnicodeTagDataEntry(new IccMultiLocalizedUnicodeTagDataEntry(value.DeviceManufacturerInfo)) + WriteTagDataEntryHeader(IccTypeSignature.MultiLocalizedUnicode) + WriteMultiLocalizedUnicodeTagDataEntry(new IccMultiLocalizedUnicodeTagDataEntry(value.DeviceModelInfo));
	}

	public int WriteScreeningChannel(in IccScreeningChannel value)
	{
		return WriteFix16(value.Frequency) + WriteFix16(value.Angle) + WriteInt32((int)value.SpotShape);
	}

	public int WriteByte(byte value)
	{
		dataStream.WriteByte(value);
		return 1;
	}

	public unsafe int WriteUInt16(ushort value)
	{
		return WriteBytes((byte*)(&value), 2);
	}

	public unsafe int WriteInt16(short value)
	{
		return WriteBytes((byte*)(&value), 2);
	}

	public unsafe int WriteUInt32(uint value)
	{
		return WriteBytes((byte*)(&value), 4);
	}

	public unsafe int WriteInt32(int value)
	{
		return WriteBytes((byte*)(&value), 4);
	}

	public unsafe int WriteUInt64(ulong value)
	{
		return WriteBytes((byte*)(&value), 8);
	}

	public unsafe int WriteInt64(long value)
	{
		return WriteBytes((byte*)(&value), 8);
	}

	public unsafe int WriteSingle(float value)
	{
		return WriteBytes((byte*)(&value), 4);
	}

	public unsafe int WriteDouble(double value)
	{
		return WriteBytes((byte*)(&value), 8);
	}

	public int WriteFix16(double value)
	{
		value = Numerics.Clamp(value, -32768.0, 32767.99998474121);
		value *= 65536.0;
		return WriteInt32((int)Math.Round(value, MidpointRounding.AwayFromZero));
	}

	public int WriteUFix16(double value)
	{
		value = Numerics.Clamp(value, 0.0, 65535.99998474121);
		value *= 65536.0;
		return WriteUInt32((uint)Math.Round(value, MidpointRounding.AwayFromZero));
	}

	public int WriteU1Fix15(double value)
	{
		value = Numerics.Clamp(value, 0.0, 1.999969482421875);
		value *= 32768.0;
		return WriteUInt16((ushort)Math.Round(value, MidpointRounding.AwayFromZero));
	}

	public int WriteUFix8(double value)
	{
		value = Numerics.Clamp(value, 0.0, 255.99609375);
		value *= 256.0;
		return WriteUInt16((ushort)Math.Round(value, MidpointRounding.AwayFromZero));
	}

	public int WriteAsciiString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return 0;
		}
		byte[] bytes = Encoding.ASCII.GetBytes(value);
		dataStream.Write(bytes, 0, bytes.Length);
		return bytes.Length;
	}

	public int WriteAsciiString(string value, int length, bool ensureNullTerminator)
	{
		if (length == 0)
		{
			return 0;
		}
		Guard.MustBeGreaterThan(length, 0, "length");
		if (value == null)
		{
			value = string.Empty;
		}
		byte value2 = 32;
		int num = 0;
		if (ensureNullTerminator)
		{
			value2 = 0;
			num = 1;
		}
		value = value.Substring(0, Math.Min(length - num, value.Length));
		byte[] bytes = Encoding.ASCII.GetBytes(value);
		int num2 = Math.Min(length - num, bytes.Length);
		dataStream.Write(bytes, 0, num2);
		for (int i = 0; i < length - num2; i++)
		{
			dataStream.WriteByte(value2);
		}
		return length;
	}

	public int WriteUnicodeString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return 0;
		}
		byte[] bytes = Encoding.BigEndianUnicode.GetBytes(value);
		dataStream.Write(bytes, 0, bytes.Length);
		return bytes.Length;
	}

	public int WriteTagDataEntry(IccTagDataEntry data, out IccTagTableEntry table)
	{
		uint offset = (uint)dataStream.Position;
		int num = WriteTagDataEntry(data);
		WritePadding();
		table = new IccTagTableEntry(data.TagSignature, offset, (uint)num);
		return num;
	}

	public int WriteTagDataEntry(IccTagDataEntry entry)
	{
		int num = WriteTagDataEntryHeader(entry.Signature);
		return num + entry.Signature switch
		{
			IccTypeSignature.Chromaticity => WriteChromaticityTagDataEntry((IccChromaticityTagDataEntry)entry), 
			IccTypeSignature.ColorantOrder => WriteColorantOrderTagDataEntry((IccColorantOrderTagDataEntry)entry), 
			IccTypeSignature.ColorantTable => WriteColorantTableTagDataEntry((IccColorantTableTagDataEntry)entry), 
			IccTypeSignature.Curve => WriteCurveTagDataEntry((IccCurveTagDataEntry)entry), 
			IccTypeSignature.Data => WriteDataTagDataEntry((IccDataTagDataEntry)entry), 
			IccTypeSignature.DateTime => WriteDateTimeTagDataEntry((IccDateTimeTagDataEntry)entry), 
			IccTypeSignature.Lut16 => WriteLut16TagDataEntry((IccLut16TagDataEntry)entry), 
			IccTypeSignature.Lut8 => WriteLut8TagDataEntry((IccLut8TagDataEntry)entry), 
			IccTypeSignature.LutAToB => WriteLutAtoBTagDataEntry((IccLutAToBTagDataEntry)entry), 
			IccTypeSignature.LutBToA => WriteLutBtoATagDataEntry((IccLutBToATagDataEntry)entry), 
			IccTypeSignature.Measurement => WriteMeasurementTagDataEntry((IccMeasurementTagDataEntry)entry), 
			IccTypeSignature.MultiLocalizedUnicode => WriteMultiLocalizedUnicodeTagDataEntry((IccMultiLocalizedUnicodeTagDataEntry)entry), 
			IccTypeSignature.MultiProcessElements => WriteMultiProcessElementsTagDataEntry((IccMultiProcessElementsTagDataEntry)entry), 
			IccTypeSignature.NamedColor2 => WriteNamedColor2TagDataEntry((IccNamedColor2TagDataEntry)entry), 
			IccTypeSignature.ParametricCurve => WriteParametricCurveTagDataEntry((IccParametricCurveTagDataEntry)entry), 
			IccTypeSignature.ProfileSequenceDesc => WriteProfileSequenceDescTagDataEntry((IccProfileSequenceDescTagDataEntry)entry), 
			IccTypeSignature.ProfileSequenceIdentifier => WriteProfileSequenceIdentifierTagDataEntry((IccProfileSequenceIdentifierTagDataEntry)entry), 
			IccTypeSignature.ResponseCurveSet16 => WriteResponseCurveSet16TagDataEntry((IccResponseCurveSet16TagDataEntry)entry), 
			IccTypeSignature.S15Fixed16Array => WriteFix16ArrayTagDataEntry((IccFix16ArrayTagDataEntry)entry), 
			IccTypeSignature.Signature => WriteSignatureTagDataEntry((IccSignatureTagDataEntry)entry), 
			IccTypeSignature.Text => WriteTextTagDataEntry((IccTextTagDataEntry)entry), 
			IccTypeSignature.U16Fixed16Array => WriteUFix16ArrayTagDataEntry((IccUFix16ArrayTagDataEntry)entry), 
			IccTypeSignature.UInt16Array => WriteUInt16ArrayTagDataEntry((IccUInt16ArrayTagDataEntry)entry), 
			IccTypeSignature.UInt32Array => WriteUInt32ArrayTagDataEntry((IccUInt32ArrayTagDataEntry)entry), 
			IccTypeSignature.UInt64Array => WriteUInt64ArrayTagDataEntry((IccUInt64ArrayTagDataEntry)entry), 
			IccTypeSignature.UInt8Array => WriteUInt8ArrayTagDataEntry((IccUInt8ArrayTagDataEntry)entry), 
			IccTypeSignature.ViewingConditions => WriteViewingConditionsTagDataEntry((IccViewingConditionsTagDataEntry)entry), 
			IccTypeSignature.Xyz => WriteXyzTagDataEntry((IccXyzTagDataEntry)entry), 
			IccTypeSignature.TextDescription => WriteTextDescriptionTagDataEntry((IccTextDescriptionTagDataEntry)entry), 
			IccTypeSignature.CrdInfo => WriteCrdInfoTagDataEntry((IccCrdInfoTagDataEntry)entry), 
			IccTypeSignature.Screening => WriteScreeningTagDataEntry((IccScreeningTagDataEntry)entry), 
			IccTypeSignature.UcrBg => WriteUcrBgTagDataEntry((IccUcrBgTagDataEntry)entry), 
			_ => WriteUnknownTagDataEntry(entry as IccUnknownTagDataEntry), 
		};
	}

	public int WriteTagDataEntryHeader(IccTypeSignature signature)
	{
		return WriteUInt32((uint)signature) + WriteEmpty(4);
	}

	public int WriteUnknownTagDataEntry(IccUnknownTagDataEntry value)
	{
		return WriteArray(value.Data);
	}

	public int WriteChromaticityTagDataEntry(IccChromaticityTagDataEntry value)
	{
		int num = WriteUInt16((ushort)value.ChannelCount);
		num += WriteUInt16((ushort)value.ColorantType);
		for (int i = 0; i < value.ChannelCount; i++)
		{
			num += WriteUFix16(value.ChannelValues[i][0]);
			num += WriteUFix16(value.ChannelValues[i][1]);
		}
		return num;
	}

	public int WriteColorantOrderTagDataEntry(IccColorantOrderTagDataEntry value)
	{
		return WriteUInt32((uint)value.ColorantNumber.Length) + WriteArray(value.ColorantNumber);
	}

	public int WriteColorantTableTagDataEntry(IccColorantTableTagDataEntry value)
	{
		int num = WriteUInt32((uint)value.ColorantData.Length);
		for (int i = 0; i < value.ColorantData.Length; i++)
		{
			ref IccColorantTableEntry reference = ref value.ColorantData[i];
			num += WriteAsciiString(reference.Name, 32, ensureNullTerminator: true);
			num += WriteUInt16(reference.Pcs1);
			num += WriteUInt16(reference.Pcs2);
			num += WriteUInt16(reference.Pcs3);
		}
		return num;
	}

	public int WriteCurveTagDataEntry(IccCurveTagDataEntry value)
	{
		int num = 0;
		if (value.IsIdentityResponse)
		{
			num += WriteUInt32(0u);
		}
		else if (value.IsGamma)
		{
			num += WriteUInt32(1u);
			num += WriteUFix8(value.Gamma);
		}
		else
		{
			num += WriteUInt32((uint)value.CurveData.Length);
			for (int i = 0; i < value.CurveData.Length; i++)
			{
				num += WriteUInt16((ushort)Numerics.Clamp(value.CurveData[i] * 65535f + 0.5f, 0f, 65535f));
			}
		}
		return num;
	}

	public int WriteDataTagDataEntry(IccDataTagDataEntry value)
	{
		return WriteEmpty(3) + WriteByte(value.IsAscii ? ((byte)1) : ((byte)0)) + WriteArray(value.Data);
	}

	public int WriteDateTimeTagDataEntry(IccDateTimeTagDataEntry value)
	{
		return WriteDateTime(value.Value);
	}

	public int WriteLut16TagDataEntry(IccLut16TagDataEntry value)
	{
		int num = WriteByte((byte)value.InputValues.Length);
		num += WriteByte((byte)value.OutputValues.Length);
		num += WriteByte(value.ClutValues.GridPointCount[0]);
		num += WriteEmpty(1);
		num += WriteMatrix(value.Matrix, isSingle: false);
		num += WriteUInt16((ushort)value.InputValues[0].Values.Length);
		num += WriteUInt16((ushort)value.OutputValues[0].Values.Length);
		IccLut[] inputValues = value.InputValues;
		foreach (IccLut value2 in inputValues)
		{
			num += WriteLut16(value2);
		}
		num += WriteClut16(value.ClutValues);
		inputValues = value.OutputValues;
		foreach (IccLut value3 in inputValues)
		{
			num += WriteLut16(value3);
		}
		return num;
	}

	public int WriteLut8TagDataEntry(IccLut8TagDataEntry value)
	{
		int num = WriteByte((byte)value.InputChannelCount);
		num += WriteByte((byte)value.OutputChannelCount);
		num += WriteByte((byte)value.ClutValues.Values[0].Length);
		num += WriteEmpty(1);
		num += WriteMatrix(value.Matrix, isSingle: false);
		IccLut[] inputValues = value.InputValues;
		foreach (IccLut value2 in inputValues)
		{
			num += WriteLut8(value2);
		}
		num += WriteClut8(value.ClutValues);
		inputValues = value.OutputValues;
		foreach (IccLut value3 in inputValues)
		{
			num += WriteLut8(value3);
		}
		return num;
	}

	public int WriteLutAtoBTagDataEntry(IccLutAToBTagDataEntry value)
	{
		long num = dataStream.Position - 8;
		int num2 = WriteByte((byte)value.InputChannelCount);
		num2 += WriteByte((byte)value.OutputChannelCount);
		num2 += WriteEmpty(2);
		long num3 = 0L;
		long num4 = 0L;
		long num5 = 0L;
		long num6 = 0L;
		long num7 = 0L;
		long position = dataStream.Position;
		dataStream.Position += 20L;
		if (value.CurveB != null)
		{
			num3 = dataStream.Position;
			num2 += WriteCurves(value.CurveB);
			num2 += WritePadding();
		}
		if (value.Matrix3x1.HasValue && value.Matrix3x3.HasValue)
		{
			num4 = dataStream.Position;
			num2 += WriteMatrix(value.Matrix3x3.Value, isSingle: false);
			num2 += WriteMatrix(value.Matrix3x1.Value, isSingle: false);
			num2 += WritePadding();
		}
		if (value.CurveM != null)
		{
			num5 = dataStream.Position;
			num2 += WriteCurves(value.CurveM);
			num2 += WritePadding();
		}
		if (value.ClutValues != null)
		{
			num6 = dataStream.Position;
			num2 += WriteClut(value.ClutValues);
			num2 += WritePadding();
		}
		if (value.CurveA != null)
		{
			num7 = dataStream.Position;
			num2 += WriteCurves(value.CurveA);
			num2 += WritePadding();
		}
		long position2 = dataStream.Position;
		dataStream.Position = position;
		if (num3 != 0L)
		{
			num3 -= num;
		}
		if (num4 != 0L)
		{
			num4 -= num;
		}
		if (num5 != 0L)
		{
			num5 -= num;
		}
		if (num6 != 0L)
		{
			num6 -= num;
		}
		if (num7 != 0L)
		{
			num7 -= num;
		}
		num2 += WriteUInt32((uint)num3);
		num2 += WriteUInt32((uint)num4);
		num2 += WriteUInt32((uint)num5);
		num2 += WriteUInt32((uint)num6);
		num2 += WriteUInt32((uint)num7);
		dataStream.Position = position2;
		return num2;
	}

	public int WriteLutBtoATagDataEntry(IccLutBToATagDataEntry value)
	{
		long num = dataStream.Position - 8;
		int num2 = WriteByte((byte)value.InputChannelCount);
		num2 += WriteByte((byte)value.OutputChannelCount);
		num2 += WriteEmpty(2);
		long num3 = 0L;
		long num4 = 0L;
		long num5 = 0L;
		long num6 = 0L;
		long num7 = 0L;
		long position = dataStream.Position;
		dataStream.Position += 20L;
		if (value.CurveB != null)
		{
			num3 = dataStream.Position;
			num2 += WriteCurves(value.CurveB);
			num2 += WritePadding();
		}
		if (value.Matrix3x1.HasValue && value.Matrix3x3.HasValue)
		{
			num4 = dataStream.Position;
			num2 += WriteMatrix(value.Matrix3x3.Value, isSingle: false);
			num2 += WriteMatrix(value.Matrix3x1.Value, isSingle: false);
			num2 += WritePadding();
		}
		if (value.CurveM != null)
		{
			num5 = dataStream.Position;
			num2 += WriteCurves(value.CurveM);
			num2 += WritePadding();
		}
		if (value.ClutValues != null)
		{
			num6 = dataStream.Position;
			num2 += WriteClut(value.ClutValues);
			num2 += WritePadding();
		}
		if (value.CurveA != null)
		{
			num7 = dataStream.Position;
			num2 += WriteCurves(value.CurveA);
			num2 += WritePadding();
		}
		long position2 = dataStream.Position;
		dataStream.Position = position;
		if (num3 != 0L)
		{
			num3 -= num;
		}
		if (num4 != 0L)
		{
			num4 -= num;
		}
		if (num5 != 0L)
		{
			num5 -= num;
		}
		if (num6 != 0L)
		{
			num6 -= num;
		}
		if (num7 != 0L)
		{
			num7 -= num;
		}
		num2 += WriteUInt32((uint)num3);
		num2 += WriteUInt32((uint)num4);
		num2 += WriteUInt32((uint)num5);
		num2 += WriteUInt32((uint)num6);
		num2 += WriteUInt32((uint)num7);
		dataStream.Position = position2;
		return num2;
	}

	public int WriteMeasurementTagDataEntry(IccMeasurementTagDataEntry value)
	{
		return WriteUInt32((uint)value.Observer) + WriteXyzNumber(value.XyzBacking) + WriteUInt32((uint)value.Geometry) + WriteUFix16(value.Flare) + WriteUInt32((uint)value.Illuminant);
	}

	public int WriteMultiLocalizedUnicodeTagDataEntry(IccMultiLocalizedUnicodeTagDataEntry value)
	{
		long num = dataStream.Position - 8;
		int num2 = value.Texts.Length;
		int num3 = WriteUInt32((uint)num2);
		num3 += WriteUInt32(12u);
		long position = dataStream.Position;
		dataStream.Position += num2 * 12;
		IGrouping<string, IccLocalizedString>[] array = (from t in value.Texts
			group t by t.Text).ToArray();
		uint[] array2 = new uint[array.Length];
		int[] array3 = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = (uint)(dataStream.Position - num);
			num3 += (array3[i] = WriteUnicodeString(array[i].Key));
		}
		long position2 = dataStream.Position;
		dataStream.Position = position;
		for (int j = 0; j < array.Length; j++)
		{
			foreach (IccLocalizedString item in array[j])
			{
				string name = item.Culture.Name;
				if (string.IsNullOrEmpty(name))
				{
					num3 += WriteAsciiString("xx", 2, ensureNullTerminator: false);
					num3 += WriteAsciiString("\0\0", 2, ensureNullTerminator: false);
				}
				else if (name.Contains('-'))
				{
					string[] array4 = name.Split('-');
					num3 += WriteAsciiString(array4[0].ToLower(item.Culture), 2, ensureNullTerminator: false);
					num3 += WriteAsciiString(array4[1].ToUpper(item.Culture), 2, ensureNullTerminator: false);
				}
				else
				{
					num3 += WriteAsciiString(name, 2, ensureNullTerminator: false);
					num3 += WriteAsciiString("\0\0", 2, ensureNullTerminator: false);
				}
				num3 += WriteUInt32((uint)array3[j]);
				num3 += WriteUInt32(array2[j]);
			}
		}
		dataStream.Position = position2;
		return num3;
	}

	public int WriteMultiProcessElementsTagDataEntry(IccMultiProcessElementsTagDataEntry value)
	{
		long num = dataStream.Position - 8;
		int num2 = WriteUInt16((ushort)value.InputChannelCount);
		num2 += WriteUInt16((ushort)value.OutputChannelCount);
		num2 += WriteUInt32((uint)value.Data.Length);
		long position = dataStream.Position;
		dataStream.Position += value.Data.Length * 8;
		IccPositionNumber[] array = new IccPositionNumber[value.Data.Length];
		for (int i = 0; i < value.Data.Length; i++)
		{
			uint offset = (uint)(dataStream.Position - num);
			int num3 = WriteMultiProcessElement(value.Data[i]);
			num2 += WritePadding();
			array[i] = new IccPositionNumber(offset, (uint)num3);
			num2 += num3;
		}
		long position2 = dataStream.Position;
		dataStream.Position = position;
		IccPositionNumber[] array2 = array;
		for (int j = 0; j < array2.Length; j++)
		{
			IccPositionNumber value2 = array2[j];
			num2 += WritePositionNumber(in value2);
		}
		dataStream.Position = position2;
		return num2;
	}

	public int WriteNamedColor2TagDataEntry(IccNamedColor2TagDataEntry value)
	{
		int num = WriteInt32(value.VendorFlags) + WriteUInt32((uint)value.Colors.Length) + WriteUInt32((uint)value.CoordinateCount) + WriteAsciiString(value.Prefix, 32, ensureNullTerminator: true) + WriteAsciiString(value.Suffix, 32, ensureNullTerminator: true);
		IccNamedColor[] colors = value.Colors;
		for (int i = 0; i < colors.Length; i++)
		{
			IccNamedColor value2 = colors[i];
			num += WriteNamedColor(in value2);
		}
		return num;
	}

	public int WriteParametricCurveTagDataEntry(IccParametricCurveTagDataEntry value)
	{
		return WriteParametricCurve(value.Curve);
	}

	public int WriteProfileSequenceDescTagDataEntry(IccProfileSequenceDescTagDataEntry value)
	{
		int num = WriteUInt32((uint)value.Descriptions.Length);
		for (int i = 0; i < value.Descriptions.Length; i++)
		{
			num += WriteProfileDescription(in value.Descriptions[i]);
		}
		return num;
	}

	public int WriteProfileSequenceIdentifierTagDataEntry(IccProfileSequenceIdentifierTagDataEntry value)
	{
		long num = dataStream.Position - 8;
		int num2 = value.Data.Length;
		int num3 = WriteUInt32((uint)num2);
		long position = dataStream.Position;
		dataStream.Position += num2 * 8;
		IccPositionNumber[] array = new IccPositionNumber[num2];
		for (int i = 0; i < num2; i++)
		{
			ref IccProfileSequenceIdentifier reference = ref value.Data[i];
			uint offset = (uint)(dataStream.Position - num);
			int num4 = WriteProfileId(reference.Id);
			num4 += WriteTagDataEntry(new IccMultiLocalizedUnicodeTagDataEntry(reference.Description));
			num4 += WritePadding();
			array[i] = new IccPositionNumber(offset, (uint)num4);
			num3 += num4;
		}
		long position2 = dataStream.Position;
		dataStream.Position = position;
		IccPositionNumber[] array2 = array;
		for (int j = 0; j < array2.Length; j++)
		{
			IccPositionNumber value2 = array2[j];
			num3 += WritePositionNumber(in value2);
		}
		dataStream.Position = position2;
		return num3;
	}

	public int WriteResponseCurveSet16TagDataEntry(IccResponseCurveSet16TagDataEntry value)
	{
		long num = dataStream.Position - 8;
		int num2 = WriteUInt16(value.ChannelCount);
		num2 += WriteUInt16((ushort)value.Curves.Length);
		long position = dataStream.Position;
		dataStream.Position += value.Curves.Length * 4;
		uint[] array = new uint[value.Curves.Length];
		for (int i = 0; i < value.Curves.Length; i++)
		{
			array[i] = (uint)(dataStream.Position - num);
			num2 += WriteResponseCurve(value.Curves[i]);
			num2 += WritePadding();
		}
		long position2 = dataStream.Position;
		dataStream.Position = position;
		num2 += WriteArray(array);
		dataStream.Position = position2;
		return num2;
	}

	public int WriteFix16ArrayTagDataEntry(IccFix16ArrayTagDataEntry value)
	{
		int num = 0;
		for (int i = 0; i < value.Data.Length; i++)
		{
			num += WriteFix16((double)value.Data[i] * 256.0);
		}
		return num;
	}

	public int WriteSignatureTagDataEntry(IccSignatureTagDataEntry value)
	{
		return WriteAsciiString(value.SignatureData, 4, ensureNullTerminator: false);
	}

	public int WriteTextTagDataEntry(IccTextTagDataEntry value)
	{
		return WriteAsciiString(value.Text);
	}

	public int WriteUFix16ArrayTagDataEntry(IccUFix16ArrayTagDataEntry value)
	{
		int num = 0;
		for (int i = 0; i < value.Data.Length; i++)
		{
			num += WriteUFix16(value.Data[i]);
		}
		return num;
	}

	public int WriteUInt16ArrayTagDataEntry(IccUInt16ArrayTagDataEntry value)
	{
		return WriteArray(value.Data);
	}

	public int WriteUInt32ArrayTagDataEntry(IccUInt32ArrayTagDataEntry value)
	{
		return WriteArray(value.Data);
	}

	public int WriteUInt64ArrayTagDataEntry(IccUInt64ArrayTagDataEntry value)
	{
		return WriteArray(value.Data);
	}

	public int WriteUInt8ArrayTagDataEntry(IccUInt8ArrayTagDataEntry value)
	{
		return WriteArray(value.Data);
	}

	public int WriteViewingConditionsTagDataEntry(IccViewingConditionsTagDataEntry value)
	{
		return WriteXyzNumber(value.IlluminantXyz) + WriteXyzNumber(value.SurroundXyz) + WriteUInt32((uint)value.Illuminant);
	}

	public int WriteXyzTagDataEntry(IccXyzTagDataEntry value)
	{
		int num = 0;
		for (int i = 0; i < value.Data.Length; i++)
		{
			num += WriteXyzNumber(value.Data[i]);
		}
		return num;
	}

	public int WriteTextDescriptionTagDataEntry(IccTextDescriptionTagDataEntry value)
	{
		int num = 0;
		if (value.Ascii == null)
		{
			num += WriteUInt32(0u);
		}
		else
		{
			dataStream.Position += 4L;
			int num2;
			num += (num2 = WriteAsciiString(value.Ascii + "\0"));
			dataStream.Position -= num2 + 4;
			num += WriteUInt32((uint)num2);
			dataStream.Position += num2;
		}
		if (value.Unicode == null)
		{
			num += WriteUInt32(0u);
			num += WriteUInt32(0u);
		}
		else
		{
			dataStream.Position += 8L;
			int num2;
			num += (num2 = WriteUnicodeString(value.Unicode + "\0"));
			dataStream.Position -= num2 + 8;
			num += WriteUInt32(value.UnicodeLanguageCode);
			num += WriteUInt32((uint)(value.Unicode.Length + 1));
			dataStream.Position += num2;
		}
		if (value.ScriptCode == null)
		{
			num += WriteUInt16(0);
			num += WriteByte(0);
			num += WriteEmpty(67);
		}
		else
		{
			dataStream.Position += 3L;
			int num2;
			num += (num2 = WriteAsciiString(value.ScriptCode, 67, ensureNullTerminator: true));
			dataStream.Position -= num2 + 3;
			num += WriteUInt16(value.ScriptCodeCode);
			num += WriteByte((byte)((value.ScriptCode.Length > 66) ? 67u : ((uint)(value.ScriptCode.Length + 1))));
			dataStream.Position += num2;
		}
		return num;
	}

	public int WriteCrdInfoTagDataEntry(IccCrdInfoTagDataEntry value)
	{
		int count = 0;
		WriteString(value.PostScriptProductName);
		WriteString(value.RenderingIntent0Crd);
		WriteString(value.RenderingIntent1Crd);
		WriteString(value.RenderingIntent2Crd);
		WriteString(value.RenderingIntent3Crd);
		return count;
		void WriteString(string text)
		{
			int num = ((!string.IsNullOrEmpty(text)) ? (text.Length + 1) : 0);
			count += WriteUInt32((uint)num);
			count += WriteAsciiString(text, num, ensureNullTerminator: true);
		}
	}

	public int WriteScreeningTagDataEntry(IccScreeningTagDataEntry value)
	{
		int num = 0;
		num += WriteInt32((int)value.Flags);
		num += WriteUInt32((uint)value.Channels.Length);
		for (int i = 0; i < value.Channels.Length; i++)
		{
			num += WriteScreeningChannel(in value.Channels[i]);
		}
		return num;
	}

	public int WriteUcrBgTagDataEntry(IccUcrBgTagDataEntry value)
	{
		int num = 0;
		num += WriteUInt32((uint)value.UcrCurve.Length);
		for (int i = 0; i < value.UcrCurve.Length; i++)
		{
			num += WriteUInt16(value.UcrCurve[i]);
		}
		num += WriteUInt32((uint)value.BgCurve.Length);
		for (int j = 0; j < value.BgCurve.Length; j++)
		{
			num += WriteUInt16(value.BgCurve[j]);
		}
		return num + WriteAsciiString(value.Description + "\0");
	}
}
