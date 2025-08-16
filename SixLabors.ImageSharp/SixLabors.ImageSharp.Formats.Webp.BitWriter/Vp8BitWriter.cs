using System;
using System.Buffers.Binary;
using System.IO;
using SixLabors.ImageSharp.Formats.Webp.Lossy;

namespace SixLabors.ImageSharp.Formats.Webp.BitWriter;

internal class Vp8BitWriter : BitWriterBase
{
	private const int DC_PRED = 0;

	private const int TM_PRED = 1;

	private const int V_PRED = 2;

	private const int H_PRED = 3;

	private const int B_DC_PRED = 0;

	private const int B_TM_PRED = 1;

	private const int B_VE_PRED = 2;

	private const int B_HE_PRED = 3;

	private const int B_RD_PRED = 4;

	private const int B_VR_PRED = 5;

	private const int B_LD_PRED = 6;

	private const int B_VL_PRED = 7;

	private const int B_HD_PRED = 8;

	private const int B_HU_PRED = 9;

	private readonly Vp8Encoder enc;

	private int range;

	private int value;

	private int run;

	private int nbBits;

	private uint pos;

	private readonly int maxPos;

	public override int NumBytes => (int)pos;

	public Vp8BitWriter(int expectedSize, Vp8Encoder enc)
		: base(expectedSize)
	{
		range = 254;
		value = 0;
		run = 0;
		nbBits = -8;
		pos = 0u;
		maxPos = 0;
		this.enc = enc;
	}

	public int PutCoeffs(int ctx, Vp8Residual residual)
	{
		int first = residual.First;
		Vp8ProbaArray vp8ProbaArray = residual.Prob[first].Probabilities[ctx];
		if (!PutBit(residual.Last >= 0, vp8ProbaArray.Probabilities[0]))
		{
			return 0;
		}
		while (first < 16)
		{
			int num = residual.Coeffs[first++];
			bool flag = num < 0;
			int num2 = (flag ? (-num) : num);
			if (!PutBit(num2 != 0, vp8ProbaArray.Probabilities[1]))
			{
				vp8ProbaArray = residual.Prob[WebpConstants.Vp8EncBands[first]].Probabilities[0];
				continue;
			}
			if (!PutBit(num2 > 1, vp8ProbaArray.Probabilities[2]))
			{
				vp8ProbaArray = residual.Prob[WebpConstants.Vp8EncBands[first]].Probabilities[1];
			}
			else
			{
				if (!PutBit(num2 > 4, vp8ProbaArray.Probabilities[3]))
				{
					if (PutBit(num2 != 2, vp8ProbaArray.Probabilities[4]))
					{
						PutBit(num2 == 4, vp8ProbaArray.Probabilities[5]);
					}
				}
				else if (!PutBit(num2 > 10, vp8ProbaArray.Probabilities[6]))
				{
					if (!PutBit(num2 > 6, vp8ProbaArray.Probabilities[7]))
					{
						PutBit(num2 == 6, 159);
					}
					else
					{
						PutBit(num2 >= 9, 165);
						PutBit((num2 & 1) == 0, 145);
					}
				}
				else
				{
					int num3;
					byte[] array;
					if (num2 < 19)
					{
						PutBit(0, vp8ProbaArray.Probabilities[8]);
						PutBit(0, vp8ProbaArray.Probabilities[9]);
						num2 -= 11;
						num3 = 4;
						array = WebpConstants.Cat3;
					}
					else if (num2 < 35)
					{
						PutBit(0, vp8ProbaArray.Probabilities[8]);
						PutBit(1, vp8ProbaArray.Probabilities[9]);
						num2 -= 19;
						num3 = 8;
						array = WebpConstants.Cat4;
					}
					else if (num2 < 67)
					{
						PutBit(1, vp8ProbaArray.Probabilities[8]);
						PutBit(0, vp8ProbaArray.Probabilities[10]);
						num2 -= 35;
						num3 = 16;
						array = WebpConstants.Cat5;
					}
					else
					{
						PutBit(1, vp8ProbaArray.Probabilities[8]);
						PutBit(1, vp8ProbaArray.Probabilities[10]);
						num2 -= 67;
						num3 = 1024;
						array = WebpConstants.Cat6;
					}
					int num4 = 0;
					while (num3 != 0)
					{
						PutBit(num2 & num3, array[num4++]);
						num3 >>= 1;
					}
				}
				vp8ProbaArray = residual.Prob[WebpConstants.Vp8EncBands[first]].Probabilities[2];
			}
			PutBitUniform(flag ? 1 : 0);
			if (first != 16 && PutBit(first <= residual.Last, vp8ProbaArray.Probabilities[0]))
			{
				continue;
			}
			return 1;
		}
		return 1;
	}

	public override void BitWriterResize(int extraSize)
	{
		long num = pos + extraSize;
		if (num > maxPos)
		{
			ResizeBuffer(maxPos, (int)num);
		}
	}

	public override void Finish()
	{
		PutBits(0u, 9 - nbBits);
		nbBits = 0;
		Flush();
	}

	public void PutSegment(int s, Span<byte> p)
	{
		if (PutBit(s >= 2, p[0]))
		{
			p = p.Slice(1, p.Length - 1);
		}
		PutBit(s & 1, p[1]);
	}

	public void PutI16Mode(int mode)
	{
		bool bit = ((mode == 1 || mode == 3) ? true : false);
		if (PutBit(bit, 156))
		{
			PutBit(mode == 1, 128);
		}
		else
		{
			PutBit(mode == 2, 163);
		}
	}

	public int PutI4Mode(int mode, Span<byte> prob)
	{
		if (PutBit(mode != 0, prob[0]) && PutBit(mode != 1, prob[1]) && PutBit(mode != 2, prob[2]))
		{
			if (!PutBit(mode >= 6, prob[3]))
			{
				if (PutBit(mode != 3, prob[4]))
				{
					PutBit(mode != 4, prob[5]);
				}
			}
			else if (PutBit(mode != 6, prob[6]) && PutBit(mode != 7, prob[7]))
			{
				PutBit(mode != 8, prob[8]);
			}
		}
		return mode;
	}

	public void PutUvMode(int uvMode)
	{
		if (PutBit(uvMode != 0, 142) && PutBit(uvMode != 2, 114))
		{
			PutBit(uvMode != 3, 183);
		}
	}

	private void PutBits(uint value, int nbBits)
	{
		for (uint num = (uint)(1 << nbBits - 1); num != 0; num >>= 1)
		{
			PutBitUniform((int)(value & num));
		}
	}

	private bool PutBit(bool bit, int prob)
	{
		return PutBit(bit ? 1 : 0, prob);
	}

	private bool PutBit(int bit, int prob)
	{
		int num = range * prob >> 8;
		if (bit != 0)
		{
			value += num + 1;
			range -= num + 1;
		}
		else
		{
			range = num;
		}
		if (range < 127)
		{
			int num2 = WebpLookupTables.Norm[range];
			range = WebpLookupTables.NewRange[range];
			value <<= num2;
			nbBits += num2;
			if (nbBits > 0)
			{
				Flush();
			}
		}
		return bit != 0;
	}

	private int PutBitUniform(int bit)
	{
		int num = range >> 1;
		if (bit != 0)
		{
			value += num + 1;
			range -= num + 1;
		}
		else
		{
			range = num;
		}
		if (range < 127)
		{
			range = WebpLookupTables.NewRange[range];
			value <<= 1;
			nbBits++;
			if (nbBits > 0)
			{
				Flush();
			}
		}
		return bit;
	}

	private void PutSignedBits(int value, int nbBits)
	{
		if (PutBitUniform((value != 0) ? 1 : 0) != 0)
		{
			if (value < 0)
			{
				int num = (-value << 1) | 1;
				PutBits((uint)num, nbBits + 1);
			}
			else
			{
				PutBits((uint)(value << 1), nbBits + 1);
			}
		}
	}

	private void Flush()
	{
		int num = 8 + nbBits;
		int num2 = value >> num;
		value -= num2 << num;
		nbBits -= 8;
		if ((num2 & 0xFF) != 255)
		{
			uint num3 = pos;
			BitWriterResize(run + 1);
			if ((num2 & 0x100) != 0 && num3 != 0)
			{
				base.Buffer[num3 - 1]++;
			}
			if (run > 0)
			{
				int num4 = (((num2 & 0x100) == 0) ? 255 : 0);
				while (run > 0)
				{
					base.Buffer[num3++] = (byte)num4;
					run--;
				}
			}
			base.Buffer[num3++] = (byte)(num2 & 0xFF);
			pos = num3;
		}
		else
		{
			run++;
		}
	}

	public override void WriteEncodedImageToStream(Stream stream)
	{
		uint numBytes = (uint)NumBytes;
		Vp8BitWriter vp8BitWriter = new Vp8BitWriter((int)((uint)(enc.Mbw * enc.Mbh * 7) / 8u), enc);
		uint num = vp8BitWriter.GeneratePartition0();
		uint num2 = 10 + num;
		num2 += numBytes;
		uint num3 = num2 & 1;
		num2 += num3;
		WriteVp8Header(stream, num2);
		WriteFrameHeader(stream, num);
		vp8BitWriter.WriteToStream(stream);
		WriteToStream(stream);
		if (num3 == 1)
		{
			stream.WriteByte(0);
		}
	}

	private uint GeneratePartition0()
	{
		PutBitUniform(0);
		PutBitUniform(0);
		WriteSegmentHeader();
		WriteFilterHeader();
		PutBits(0u, 2);
		WriteQuant();
		PutBitUniform(0);
		WriteProbas();
		CodeIntraModes();
		Finish();
		return (uint)NumBytes;
	}

	private void WriteSegmentHeader()
	{
		Vp8EncSegmentHeader segmentHeader = enc.SegmentHeader;
		Vp8EncProba proba = enc.Proba;
		if (PutBitUniform((segmentHeader.NumSegments > 1) ? 1 : 0) == 0)
		{
			return;
		}
		int bit = 1;
		PutBitUniform(segmentHeader.UpdateMap ? 1 : 0);
		if (PutBitUniform(bit) != 0)
		{
			PutBitUniform(1);
			for (int i = 0; i < 4; i++)
			{
				PutSignedBits(enc.SegmentInfos[i].Quant, 7);
			}
			for (int j = 0; j < 4; j++)
			{
				PutSignedBits(enc.SegmentInfos[j].FStrength, 6);
			}
		}
		if (!segmentHeader.UpdateMap)
		{
			return;
		}
		for (int k = 0; k < 3; k++)
		{
			if (PutBitUniform((proba.Segments[k] != byte.MaxValue) ? 1 : 0) != 0)
			{
				PutBits(proba.Segments[k], 8);
			}
		}
	}

	private void WriteFilterHeader()
	{
		Vp8FilterHeader filterHeader = enc.FilterHeader;
		bool flag = filterHeader.I4x4LfDelta != 0;
		PutBitUniform(filterHeader.Simple ? 1 : 0);
		PutBits((uint)filterHeader.FilterLevel, 6);
		PutBits((uint)filterHeader.Sharpness, 3);
		if (PutBitUniform(flag ? 1 : 0) != 0)
		{
			bool flag2 = filterHeader.I4x4LfDelta != 0;
			if (PutBitUniform(flag2 ? 1 : 0) != 0)
			{
				PutBits(0u, 4);
				PutSignedBits(filterHeader.I4x4LfDelta, 6);
				PutBits(0u, 3);
			}
		}
	}

	private void WriteQuant()
	{
		PutBits((uint)enc.BaseQuant, 7);
		PutSignedBits(enc.DqY1Dc, 4);
		PutSignedBits(enc.DqY2Dc, 4);
		PutSignedBits(enc.DqY2Ac, 4);
		PutSignedBits(enc.DqUvDc, 4);
		PutSignedBits(enc.DqUvAc, 4);
	}

	private void WriteProbas()
	{
		Vp8EncProba proba = enc.Proba;
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				for (int k = 0; k < 3; k++)
				{
					for (int l = 0; l < 11; l++)
					{
						byte b = proba.Coeffs[i][j].Probabilities[k].Probabilities[l];
						bool bit = b != WebpLookupTables.DefaultCoeffsProba[i, j, k, l];
						if (PutBit(bit, WebpLookupTables.CoeffsUpdateProba[i, j, k, l]))
						{
							PutBits(b, 8);
						}
					}
				}
			}
		}
		if (PutBitUniform(proba.UseSkipProba ? 1 : 0) != 0)
		{
			PutBits(proba.SkipProba, 8);
		}
	}

	private void CodeIntraModes()
	{
		Vp8EncIterator vp8EncIterator = new Vp8EncIterator(enc);
		int predsWidth = enc.PredsWidth;
		do
		{
			Vp8MacroBlockInfo currentMacroBlockInfo = vp8EncIterator.CurrentMacroBlockInfo;
			int num = vp8EncIterator.PredIdx;
			Span<byte> span = vp8EncIterator.Preds.AsSpan(num);
			if (enc.SegmentHeader.UpdateMap)
			{
				PutSegment(currentMacroBlockInfo.Segment, enc.Proba.Segments);
			}
			if (enc.Proba.UseSkipProba)
			{
				PutBit(currentMacroBlockInfo.Skip, enc.Proba.SkipProba);
			}
			if (PutBit(currentMacroBlockInfo.MacroBlockType != Vp8MacroBlockType.I4X4, 145))
			{
				PutI16Mode(span[0]);
			}
			else
			{
				Span<byte> span2 = vp8EncIterator.Preds.AsSpan(num - predsWidth);
				for (int i = 0; i < 4; i++)
				{
					int num2 = vp8EncIterator.Preds[num - 1];
					for (int j = 0; j < 4; j++)
					{
						byte[] array = WebpLookupTables.ModesProba[span2[j], num2];
						num2 = PutI4Mode(vp8EncIterator.Preds[num + j], array);
					}
					span2 = vp8EncIterator.Preds.AsSpan(num);
					num += predsWidth;
				}
			}
			PutUvMode(currentMacroBlockInfo.UvMode);
		}
		while (vp8EncIterator.Next());
	}

	private void WriteVp8Header(Stream stream, uint size)
	{
		Span<byte> span = stackalloc byte[4];
		BinaryPrimitives.WriteUInt32BigEndian(span, 1448097824u);
		stream.Write(span);
		BinaryPrimitives.WriteUInt32LittleEndian(span, size);
		stream.Write(span);
	}

	private void WriteFrameHeader(Stream stream, uint size0)
	{
		uint num = 0u;
		int width = enc.Width;
		int height = enc.Height;
		byte[] array = new byte[10];
		uint num2 = 0 | (num << 1) | 0x10 | (size0 << 5);
		array[0] = (byte)(num2 & 0xFF);
		array[1] = (byte)((num2 >> 8) & 0xFF);
		array[2] = (byte)((num2 >> 16) & 0xFF);
		array[3] = WebpConstants.Vp8HeaderMagicBytes[0];
		array[4] = WebpConstants.Vp8HeaderMagicBytes[1];
		array[5] = WebpConstants.Vp8HeaderMagicBytes[2];
		array[6] = (byte)(width & 0xFF);
		array[7] = (byte)(width >> 8);
		array[8] = (byte)(height & 0xFF);
		array[9] = (byte)(height >> 8);
		stream.Write(array);
	}
}
