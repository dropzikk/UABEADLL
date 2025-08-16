using System;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.IO;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal sealed class T6BitReader : T4BitReader
{
	private readonly int maxCodeLength = 12;

	private static readonly CcittTwoDimensionalCode None = new CcittTwoDimensionalCode(0, CcittTwoDimensionalCodeType.None, 0);

	private static readonly CcittTwoDimensionalCode Len1Code1 = new CcittTwoDimensionalCode(1, CcittTwoDimensionalCodeType.Vertical0, 1);

	private static readonly CcittTwoDimensionalCode Len3Code001 = new CcittTwoDimensionalCode(1, CcittTwoDimensionalCodeType.Horizontal, 3);

	private static readonly CcittTwoDimensionalCode Len3Code010 = new CcittTwoDimensionalCode(2, CcittTwoDimensionalCodeType.VerticalL1, 3);

	private static readonly CcittTwoDimensionalCode Len3Code011 = new CcittTwoDimensionalCode(3, CcittTwoDimensionalCodeType.VerticalR1, 3);

	private static readonly CcittTwoDimensionalCode Len4Code0001 = new CcittTwoDimensionalCode(1, CcittTwoDimensionalCodeType.Pass, 4);

	private static readonly CcittTwoDimensionalCode Len6Code000011 = new CcittTwoDimensionalCode(3, CcittTwoDimensionalCodeType.VerticalR2, 6);

	private static readonly CcittTwoDimensionalCode Len6Code000010 = new CcittTwoDimensionalCode(2, CcittTwoDimensionalCodeType.VerticalL2, 6);

	private static readonly CcittTwoDimensionalCode Len7Code0000011 = new CcittTwoDimensionalCode(3, CcittTwoDimensionalCodeType.VerticalR3, 7);

	private static readonly CcittTwoDimensionalCode Len7Code0000010 = new CcittTwoDimensionalCode(2, CcittTwoDimensionalCodeType.VerticalL3, 7);

	private static readonly CcittTwoDimensionalCode Len7Code0000001 = new CcittTwoDimensionalCode(1, CcittTwoDimensionalCodeType.Extensions2D, 7);

	private static readonly CcittTwoDimensionalCode Len7Code0000000 = new CcittTwoDimensionalCode(0, CcittTwoDimensionalCodeType.Extensions1D, 7);

	public override bool HasMoreData
	{
		get
		{
			if (base.Position >= (ulong)((long)base.DataLength - 1L))
			{
				return (uint)(base.BitsRead - 1) < 6u;
			}
			return true;
		}
	}

	public CcittTwoDimensionalCode Code { get; internal set; }

	public T6BitReader(BufferedReadStream input, TiffFillOrder fillOrder, int bytesToRead)
		: base(input, fillOrder, bytesToRead)
	{
	}

	public bool ReadNextCodeWord()
	{
		Code = None;
		Reset();
		uint num = ReadValue(1);
		do
		{
			if (base.CurValueBitsRead > maxCodeLength)
			{
				TiffThrowHelper.ThrowImageFormatException("ccitt compression parsing error: invalid code length read");
			}
			switch (base.CurValueBitsRead)
			{
			case 1:
				if (num == Len1Code1.Code)
				{
					Code = Len1Code1;
					return false;
				}
				break;
			case 3:
				if (num == Len3Code001.Code)
				{
					Code = Len3Code001;
					return false;
				}
				if (num == Len3Code010.Code)
				{
					Code = Len3Code010;
					return false;
				}
				if (num == Len3Code011.Code)
				{
					Code = Len3Code011;
					return false;
				}
				break;
			case 4:
				if (num == Len4Code0001.Code)
				{
					Code = Len4Code0001;
					return false;
				}
				break;
			case 6:
				if (num == Len6Code000010.Code)
				{
					Code = Len6Code000010;
					return false;
				}
				if (num == Len6Code000011.Code)
				{
					Code = Len6Code000011;
					return false;
				}
				break;
			case 7:
				if (num == Len7Code0000000.Code)
				{
					Code = Len7Code0000000;
					if (ReadValue(5) == 1)
					{
						return true;
					}
					throw new NotSupportedException("ccitt extensions 1D codes are not supported.");
				}
				if (num == Len7Code0000001.Code)
				{
					Code = Len7Code0000001;
					if (ReadValue(5) == 1)
					{
						return true;
					}
					throw new NotSupportedException("ccitt extensions 2D codes are not supported.");
				}
				if (num == Len7Code0000011.Code)
				{
					Code = Len7Code0000011;
					return false;
				}
				if (num == Len7Code0000010.Code)
				{
					Code = Len7Code0000010;
					return false;
				}
				break;
			}
			uint num2 = ReadValue(1);
			num = (num << 1) | num2;
		}
		while (!IsEndOfScanLine);
		if (IsEndOfScanLine)
		{
			return true;
		}
		return false;
	}

	protected override void ReadEolBeforeFirstData()
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SwapColor()
	{
		base.IsWhiteRun = !base.IsWhiteRun;
	}
}
