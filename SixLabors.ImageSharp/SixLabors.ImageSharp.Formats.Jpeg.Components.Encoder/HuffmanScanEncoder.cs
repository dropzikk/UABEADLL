using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Encoder;

internal class HuffmanScanEncoder
{
	private const int MaxBytesPerBlock = 416;

	private const int MaxBytesPerBlockMultiplier = 2;

	private const int OutputBufferLengthMultiplier = 2;

	private readonly HuffmanLut[] dcHuffmanTables = new HuffmanLut[4];

	private readonly HuffmanLut[] acHuffmanTables = new HuffmanLut[4];

	private uint accumulatedBits;

	private readonly uint[] emitBuffer;

	private readonly byte[] streamWriteBuffer;

	private int bitCount;

	private int emitWriteIndex;

	private readonly Stream target;

	private bool IsStreamFlushNeeded
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return emitWriteIndex < (int)((uint)emitBuffer.Length / 2u);
		}
	}

	public HuffmanScanEncoder(int blocksPerCodingUnit, Stream outputStream)
	{
		int num = 416 * blocksPerCodingUnit;
		emitBuffer = new uint[num / 4];
		emitWriteIndex = emitBuffer.Length;
		streamWriteBuffer = new byte[num * 2];
		target = outputStream;
	}

	public void BuildHuffmanTable(JpegHuffmanTableConfig tableConfig)
	{
		((tableConfig.Class == 0) ? dcHuffmanTables : acHuffmanTables)[tableConfig.DestinationIndex] = new HuffmanLut(tableConfig.Table);
	}

	public void EncodeScanBaselineInterleaved<TPixel>(JpegEncodingColor color, JpegFrame frame, SpectralConverter<TPixel> converter, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		if (color == JpegEncodingColor.YCbCrRatio444 || color == JpegEncodingColor.Rgb)
		{
			EncodeThreeComponentBaselineInterleavedScanNoSubsampling(frame, converter, cancellationToken);
		}
		else
		{
			EncodeScanBaselineInterleaved(frame, converter, cancellationToken);
		}
	}

	public void EncodeScanBaselineSingleComponent<TPixel>(Component component, SpectralConverter<TPixel> converter, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		int heightInBlocks = component.HeightInBlocks;
		int widthInBlocks = component.WidthInBlocks;
		ref HuffmanLut dcTable = ref dcHuffmanTables[component.DcTableId];
		ref HuffmanLut acTable = ref acHuffmanTables[component.AcTableId];
		for (int i = 0; i < heightInBlocks; i++)
		{
			cancellationToken.ThrowIfCancellationRequested();
			converter.ConvertStrideBaseline();
			ref Block8x8 reference = ref MemoryMarshal.GetReference(component.SpectralBlocks.DangerousGetRowSpan(0));
			for (nuint num = 0u; num < (uint)widthInBlocks; num++)
			{
				WriteBlock(component, ref Unsafe.Add(ref reference, num), ref dcTable, ref acTable);
				if (IsStreamFlushNeeded)
				{
					FlushToStream();
				}
			}
		}
		FlushRemainingBytes();
	}

	public void EncodeScanBaseline(Component component, CancellationToken cancellationToken)
	{
		int heightInBlocks = component.HeightInBlocks;
		int widthInBlocks = component.WidthInBlocks;
		ref HuffmanLut dcTable = ref dcHuffmanTables[component.DcTableId];
		ref HuffmanLut acTable = ref acHuffmanTables[component.AcTableId];
		for (int i = 0; i < heightInBlocks; i++)
		{
			cancellationToken.ThrowIfCancellationRequested();
			ref Block8x8 reference = ref MemoryMarshal.GetReference(component.SpectralBlocks.DangerousGetRowSpan(i));
			for (nuint num = 0u; num < (uint)widthInBlocks; num++)
			{
				WriteBlock(component, ref Unsafe.Add(ref reference, num), ref dcTable, ref acTable);
				if (IsStreamFlushNeeded)
				{
					FlushToStream();
				}
			}
		}
		FlushRemainingBytes();
	}

	private void EncodeScanBaselineInterleaved<TPixel>(JpegFrame frame, SpectralConverter<TPixel> converter, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		int num = 0;
		int mcusPerColumn = frame.McusPerColumn;
		int mcusPerLine = frame.McusPerLine;
		for (int i = 0; i < mcusPerColumn; i++)
		{
			cancellationToken.ThrowIfCancellationRequested();
			converter.ConvertStrideBaseline();
			for (int j = 0; j < mcusPerLine; j++)
			{
				int num2 = num % mcusPerLine;
				for (int k = 0; k < frame.Components.Length; k++)
				{
					Component component = frame.Components[k];
					ref HuffmanLut dcTable = ref dcHuffmanTables[component.DcTableId];
					ref HuffmanLut acTable = ref acHuffmanTables[component.AcTableId];
					int horizontalSamplingFactor = component.HorizontalSamplingFactor;
					int verticalSamplingFactor = component.VerticalSamplingFactor;
					nuint num3 = (uint)(num2 * horizontalSamplingFactor);
					for (int l = 0; l < verticalSamplingFactor; l++)
					{
						ref Block8x8 reference = ref MemoryMarshal.GetReference(component.SpectralBlocks.DangerousGetRowSpan(l));
						for (nuint num4 = 0u; num4 < (uint)horizontalSamplingFactor; num4++)
						{
							nuint elementOffset = num3 + num4;
							WriteBlock(component, ref Unsafe.Add(ref reference, elementOffset), ref dcTable, ref acTable);
						}
					}
				}
				num++;
				if (IsStreamFlushNeeded)
				{
					FlushToStream();
				}
			}
		}
		FlushRemainingBytes();
	}

	private void EncodeThreeComponentBaselineInterleavedScanNoSubsampling<TPixel>(JpegFrame frame, SpectralConverter<TPixel> converter, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
	{
		nuint num = (uint)frame.McusPerColumn;
		nuint num2 = (uint)frame.McusPerLine;
		Component component = frame.Components[2];
		Component component2 = frame.Components[1];
		Component component3 = frame.Components[0];
		ref HuffmanLut dcTable = ref dcHuffmanTables[component3.DcTableId];
		ref HuffmanLut acTable = ref acHuffmanTables[component3.AcTableId];
		ref HuffmanLut dcTable2 = ref dcHuffmanTables[component2.DcTableId];
		ref HuffmanLut acTable2 = ref acHuffmanTables[component2.AcTableId];
		ref HuffmanLut dcTable3 = ref dcHuffmanTables[component.DcTableId];
		ref HuffmanLut acTable3 = ref acHuffmanTables[component.AcTableId];
		ref Block8x8 reference = ref MemoryMarshal.GetReference(component3.SpectralBlocks.DangerousGetRowSpan(0));
		ref Block8x8 reference2 = ref MemoryMarshal.GetReference(component2.SpectralBlocks.DangerousGetRowSpan(0));
		ref Block8x8 reference3 = ref MemoryMarshal.GetReference(component.SpectralBlocks.DangerousGetRowSpan(0));
		for (nuint num3 = 0u; num3 < num; num3++)
		{
			cancellationToken.ThrowIfCancellationRequested();
			converter.ConvertStrideBaseline();
			for (nuint num4 = 0u; num4 < num2; num4++)
			{
				WriteBlock(component3, ref Unsafe.Add(ref reference, num4), ref dcTable, ref acTable);
				WriteBlock(component2, ref Unsafe.Add(ref reference2, num4), ref dcTable2, ref acTable2);
				WriteBlock(component, ref Unsafe.Add(ref reference3, num4), ref dcTable3, ref acTable3);
				if (IsStreamFlushNeeded)
				{
					FlushToStream();
				}
			}
		}
		FlushRemainingBytes();
	}

	private void WriteBlock(Component component, ref Block8x8 block, ref HuffmanLut dcTable, ref HuffmanLut acTable)
	{
		int num = block[0];
		EmitHuffRLE(dcTable.Values, 0, num - component.DcPredictor);
		component.DcPredictor = num;
		int[] values = acTable.Values;
		nint lastNonZeroIndex = block.GetLastNonZeroIndex();
		int num2 = 0;
		ref short source = ref Unsafe.As<Block8x8, short>(ref block);
		for (nint num3 = 1; num3 <= lastNonZeroIndex; num3++)
		{
			int num4 = Unsafe.Add(ref source, num3);
			if (num4 == 0)
			{
				num2 += 16;
				continue;
			}
			while (num2 >= 256)
			{
				EmitHuff(values, 240);
				num2 -= 256;
			}
			EmitHuffRLE(values, num2, num4);
			num2 = 0;
		}
		if (lastNonZeroIndex != 63)
		{
			EmitHuff(values, 0);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Emit(uint bits, int count)
	{
		accumulatedBits |= bits >> bitCount;
		count += bitCount;
		if (count >= 32)
		{
			emitBuffer[--emitWriteIndex] = accumulatedBits;
			accumulatedBits = bits << 32 - bitCount;
			count -= 32;
		}
		bitCount = count;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void EmitHuff(int[] table, int value)
	{
		int num = table[value];
		Emit((uint)(num & -256), num & 0xFF);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void EmitHuffRLE(int[] table, int runLength, int value)
	{
		int num = value;
		int num2 = value;
		if (num < 0)
		{
			num = -value;
			num2 = value - 1;
		}
		int huffmanEncodingLength = GetHuffmanEncodingLength((uint)num);
		int num3 = table[runLength | huffmanEncodingLength];
		int num4 = num3 & 0xFF;
		uint num5 = (uint)(num3 & -65536);
		uint num6 = (uint)(num2 << 32 - huffmanEncodingLength);
		Emit(num5 | (num6 >> num4), num4 + huffmanEncodingLength);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static int GetHuffmanEncodingLength(uint value)
	{
		return 32 - BitOperations.LeadingZeroCount(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void FlushToStream(int endIndex)
	{
		Span<byte> span = MemoryMarshal.AsBytes(emitBuffer.AsSpan());
		int count = 0;
		int num = span.Length - 1;
		if (BitConverter.IsLittleEndian)
		{
			for (int num2 = num; num2 >= endIndex; num2--)
			{
				byte b = span[num2];
				streamWriteBuffer[count++] = b;
				if (b == byte.MaxValue)
				{
					streamWriteBuffer[count++] = 0;
				}
			}
		}
		else
		{
			for (int num3 = num; num3 >= endIndex; num3 -= 4)
			{
				for (int i = num3 - 3; i <= num3; i++)
				{
					byte b2 = span[i];
					streamWriteBuffer[count++] = b2;
					if (b2 == byte.MaxValue)
					{
						streamWriteBuffer[count++] = 0;
					}
				}
			}
		}
		target.Write(streamWriteBuffer, 0, count);
		emitWriteIndex = emitBuffer.Length;
	}

	private void FlushToStream()
	{
		FlushToStream(emitWriteIndex * 4);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void FlushRemainingBytes()
	{
		int num = (int)Numerics.DivideCeil((uint)bitCount, 8u);
		uint num2 = accumulatedBits | (uint)(-1 >>> bitCount);
		emitBuffer[emitWriteIndex - 1] = num2;
		int endIndex = emitWriteIndex * 4 - num;
		FlushToStream(endIndex);
		bitCount = 0;
		accumulatedBits = 0u;
	}
}
