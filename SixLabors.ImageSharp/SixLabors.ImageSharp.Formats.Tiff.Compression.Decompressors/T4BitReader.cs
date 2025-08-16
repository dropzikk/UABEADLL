using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Formats.Tiff.Constants;
using SixLabors.ImageSharp.IO;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

internal class T4BitReader
{
	private readonly TiffFillOrder fillOrder;

	private bool isFirstScanLine;

	private bool terminationCodeFound;

	private bool isStartOfRow;

	private readonly bool eolPadding;

	private const int MinCodeLength = 2;

	private readonly int maxCodeLength = 13;

	private static readonly Dictionary<uint, uint> WhiteLen4TermCodes = new Dictionary<uint, uint>
	{
		{ 7u, 2u },
		{ 8u, 3u },
		{ 11u, 4u },
		{ 12u, 5u },
		{ 14u, 6u },
		{ 15u, 7u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen5TermCodes = new Dictionary<uint, uint>
	{
		{ 19u, 8u },
		{ 20u, 9u },
		{ 7u, 10u },
		{ 8u, 11u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen6TermCodes = new Dictionary<uint, uint>
	{
		{ 7u, 1u },
		{ 8u, 12u },
		{ 3u, 13u },
		{ 52u, 14u },
		{ 53u, 15u },
		{ 42u, 16u },
		{ 43u, 17u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen7TermCodes = new Dictionary<uint, uint>
	{
		{ 39u, 18u },
		{ 12u, 19u },
		{ 8u, 20u },
		{ 23u, 21u },
		{ 3u, 22u },
		{ 4u, 23u },
		{ 40u, 24u },
		{ 43u, 25u },
		{ 19u, 26u },
		{ 36u, 27u },
		{ 24u, 28u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen8TermCodes = new Dictionary<uint, uint>
	{
		{ 53u, 0u },
		{ 2u, 29u },
		{ 3u, 30u },
		{ 26u, 31u },
		{ 27u, 32u },
		{ 18u, 33u },
		{ 19u, 34u },
		{ 20u, 35u },
		{ 21u, 36u },
		{ 22u, 37u },
		{ 23u, 38u },
		{ 40u, 39u },
		{ 41u, 40u },
		{ 42u, 41u },
		{ 43u, 42u },
		{ 44u, 43u },
		{ 45u, 44u },
		{ 4u, 45u },
		{ 5u, 46u },
		{ 10u, 47u },
		{ 11u, 48u },
		{ 82u, 49u },
		{ 83u, 50u },
		{ 84u, 51u },
		{ 85u, 52u },
		{ 36u, 53u },
		{ 37u, 54u },
		{ 88u, 55u },
		{ 89u, 56u },
		{ 90u, 57u },
		{ 91u, 58u },
		{ 74u, 59u },
		{ 75u, 60u },
		{ 50u, 61u },
		{ 51u, 62u },
		{ 52u, 63u }
	};

	private static readonly Dictionary<uint, uint> BlackLen2TermCodes = new Dictionary<uint, uint>
	{
		{ 3u, 2u },
		{ 2u, 3u }
	};

	private static readonly Dictionary<uint, uint> BlackLen3TermCodes = new Dictionary<uint, uint>
	{
		{ 2u, 1u },
		{ 3u, 4u }
	};

	private static readonly Dictionary<uint, uint> BlackLen4TermCodes = new Dictionary<uint, uint>
	{
		{ 3u, 5u },
		{ 2u, 6u }
	};

	private static readonly Dictionary<uint, uint> BlackLen5TermCodes = new Dictionary<uint, uint> { { 3u, 7u } };

	private static readonly Dictionary<uint, uint> BlackLen6TermCodes = new Dictionary<uint, uint>
	{
		{ 5u, 8u },
		{ 4u, 9u }
	};

	private static readonly Dictionary<uint, uint> BlackLen7TermCodes = new Dictionary<uint, uint>
	{
		{ 4u, 10u },
		{ 5u, 11u },
		{ 7u, 12u }
	};

	private static readonly Dictionary<uint, uint> BlackLen8TermCodes = new Dictionary<uint, uint>
	{
		{ 4u, 13u },
		{ 7u, 14u }
	};

	private static readonly Dictionary<uint, uint> BlackLen9TermCodes = new Dictionary<uint, uint> { { 24u, 15u } };

	private static readonly Dictionary<uint, uint> BlackLen10TermCodes = new Dictionary<uint, uint>
	{
		{ 55u, 0u },
		{ 23u, 16u },
		{ 24u, 17u },
		{ 8u, 18u }
	};

	private static readonly Dictionary<uint, uint> BlackLen11TermCodes = new Dictionary<uint, uint>
	{
		{ 103u, 19u },
		{ 104u, 20u },
		{ 108u, 21u },
		{ 55u, 22u },
		{ 40u, 23u },
		{ 23u, 24u },
		{ 24u, 25u }
	};

	private static readonly Dictionary<uint, uint> BlackLen12TermCodes = new Dictionary<uint, uint>
	{
		{ 202u, 26u },
		{ 203u, 27u },
		{ 204u, 28u },
		{ 205u, 29u },
		{ 104u, 30u },
		{ 105u, 31u },
		{ 106u, 32u },
		{ 107u, 33u },
		{ 210u, 34u },
		{ 211u, 35u },
		{ 212u, 36u },
		{ 213u, 37u },
		{ 214u, 38u },
		{ 215u, 39u },
		{ 108u, 40u },
		{ 109u, 41u },
		{ 218u, 42u },
		{ 219u, 43u },
		{ 84u, 44u },
		{ 85u, 45u },
		{ 86u, 46u },
		{ 87u, 47u },
		{ 100u, 48u },
		{ 101u, 49u },
		{ 82u, 50u },
		{ 83u, 51u },
		{ 36u, 52u },
		{ 55u, 53u },
		{ 56u, 54u },
		{ 39u, 55u },
		{ 40u, 56u },
		{ 88u, 57u },
		{ 89u, 58u },
		{ 43u, 59u },
		{ 44u, 60u },
		{ 90u, 61u },
		{ 102u, 62u },
		{ 103u, 63u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen5MakeupCodes = new Dictionary<uint, uint>
	{
		{ 27u, 64u },
		{ 18u, 128u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen6MakeupCodes = new Dictionary<uint, uint>
	{
		{ 23u, 192u },
		{ 24u, 1664u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen8MakeupCodes = new Dictionary<uint, uint>
	{
		{ 54u, 320u },
		{ 55u, 384u },
		{ 100u, 448u },
		{ 101u, 512u },
		{ 104u, 576u },
		{ 103u, 640u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen7MakeupCodes = new Dictionary<uint, uint> { { 55u, 256u } };

	private static readonly Dictionary<uint, uint> WhiteLen9MakeupCodes = new Dictionary<uint, uint>
	{
		{ 204u, 704u },
		{ 205u, 768u },
		{ 210u, 832u },
		{ 211u, 896u },
		{ 212u, 960u },
		{ 213u, 1024u },
		{ 214u, 1088u },
		{ 215u, 1152u },
		{ 216u, 1216u },
		{ 217u, 1280u },
		{ 218u, 1344u },
		{ 219u, 1408u },
		{ 152u, 1472u },
		{ 153u, 1536u },
		{ 154u, 1600u },
		{ 155u, 1728u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen11MakeupCodes = new Dictionary<uint, uint>
	{
		{ 8u, 1792u },
		{ 12u, 1856u },
		{ 13u, 1920u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen12MakeupCodes = new Dictionary<uint, uint>
	{
		{ 18u, 1984u },
		{ 19u, 2048u },
		{ 20u, 2112u },
		{ 21u, 2176u },
		{ 22u, 2240u },
		{ 23u, 2304u },
		{ 28u, 2368u },
		{ 29u, 2432u },
		{ 30u, 2496u },
		{ 31u, 2560u }
	};

	private static readonly Dictionary<uint, uint> BlackLen10MakeupCodes = new Dictionary<uint, uint> { { 15u, 64u } };

	private static readonly Dictionary<uint, uint> BlackLen11MakeupCodes = new Dictionary<uint, uint>
	{
		{ 8u, 1792u },
		{ 12u, 1856u },
		{ 13u, 1920u }
	};

	private static readonly Dictionary<uint, uint> BlackLen12MakeupCodes = new Dictionary<uint, uint>
	{
		{ 200u, 128u },
		{ 201u, 192u },
		{ 91u, 256u },
		{ 51u, 320u },
		{ 52u, 384u },
		{ 53u, 448u },
		{ 18u, 1984u },
		{ 19u, 2048u },
		{ 20u, 2112u },
		{ 21u, 2176u },
		{ 22u, 2240u },
		{ 23u, 2304u },
		{ 28u, 2368u },
		{ 29u, 2432u },
		{ 30u, 2496u },
		{ 31u, 2560u }
	};

	private static readonly Dictionary<uint, uint> BlackLen13MakeupCodes = new Dictionary<uint, uint>
	{
		{ 108u, 512u },
		{ 109u, 576u },
		{ 74u, 640u },
		{ 75u, 704u },
		{ 76u, 768u },
		{ 77u, 832u },
		{ 114u, 896u },
		{ 115u, 960u },
		{ 116u, 1024u },
		{ 117u, 1088u },
		{ 118u, 1152u },
		{ 119u, 1216u },
		{ 82u, 1280u },
		{ 83u, 1344u },
		{ 84u, 1408u },
		{ 85u, 1472u },
		{ 90u, 1536u },
		{ 91u, 1600u },
		{ 100u, 1664u },
		{ 101u, 1728u }
	};

	private readonly BufferedReadStream stream;

	private byte DataAtPosition { get; set; }

	protected uint Value { get; private set; }

	protected int CurValueBitsRead { get; private set; }

	protected int BitsRead { get; private set; }

	protected int DataLength { get; }

	protected ulong Position { get; set; }

	public virtual bool HasMoreData => Position < (ulong)((long)DataLength - 1L);

	public bool IsWhiteRun { get; protected set; }

	public uint RunLength { get; private set; }

	public virtual bool IsEndOfScanLine
	{
		get
		{
			if (eolPadding)
			{
				if (CurValueBitsRead >= 12)
				{
					return Value == 1;
				}
				return false;
			}
			if (CurValueBitsRead == 12)
			{
				return Value == 1;
			}
			return false;
		}
	}

	public T4BitReader(BufferedReadStream input, TiffFillOrder fillOrder, int bytesToRead, bool eolPadding = false)
	{
		stream = input;
		this.fillOrder = fillOrder;
		DataLength = bytesToRead;
		BitsRead = 0;
		Value = 0u;
		CurValueBitsRead = 0;
		Position = 0uL;
		IsWhiteRun = true;
		isFirstScanLine = true;
		isStartOfRow = true;
		terminationCodeFound = false;
		RunLength = 0u;
		this.eolPadding = eolPadding;
		ReadNextByte();
		if (this.eolPadding)
		{
			maxCodeLength = 24;
		}
	}

	public void ReadNextRun()
	{
		if (terminationCodeFound)
		{
			IsWhiteRun = !IsWhiteRun;
			terminationCodeFound = false;
		}
		Reset();
		ReadEolBeforeFirstData();
		Value = ReadValue(2);
		do
		{
			if (CurValueBitsRead > maxCodeLength)
			{
				TiffThrowHelper.ThrowImageFormatException("ccitt compression parsing error: invalid code length read");
			}
			if (IsMakeupCode())
			{
				if (IsWhiteRun)
				{
					RunLength += WhiteMakeupCodeRunLength();
				}
				else
				{
					RunLength += BlackMakeupCodeRunLength();
				}
				isStartOfRow = false;
				Reset(resetRunLength: false);
				continue;
			}
			if (IsTerminatingCode())
			{
				if (isStartOfRow && IsWhiteRun && WhiteTerminatingCodeRunLength() == 0)
				{
					Reset();
					isStartOfRow = false;
					terminationCodeFound = true;
					RunLength = 0u;
					break;
				}
				if (IsWhiteRun)
				{
					RunLength += WhiteTerminatingCodeRunLength();
				}
				else
				{
					RunLength += BlackTerminatingCodeRunLength();
				}
				terminationCodeFound = true;
				isStartOfRow = false;
				break;
			}
			uint num = ReadValue(1);
			Value = (Value << 1) | num;
			if (IsEndOfScanLine)
			{
				StartNewRow();
			}
		}
		while (!IsEndOfScanLine);
		isFirstScanLine = false;
	}

	public virtual void StartNewRow()
	{
		IsWhiteRun = true;
		isStartOfRow = true;
		terminationCodeFound = false;
	}

	protected virtual void ReadEolBeforeFirstData()
	{
		if (isFirstScanLine)
		{
			Value = ReadValue(eolPadding ? 16 : 12);
			if (!IsEndOfScanLine)
			{
				TiffThrowHelper.ThrowImageFormatException("ccitt compression parsing error: expected start of data marker not found");
			}
			Reset();
		}
	}

	protected void Reset(bool resetRunLength = true)
	{
		Value = 0u;
		CurValueBitsRead = 0;
		if (resetRunLength)
		{
			RunLength = 0u;
		}
	}

	protected void ResetBitsRead()
	{
		BitsRead = 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected uint ReadValue(int nBits)
	{
		uint num = 0u;
		int num2 = nBits;
		while (num2-- > 0)
		{
			uint bit = GetBit();
			num |= bit << num2;
			CurValueBitsRead++;
		}
		return num;
	}

	protected bool AdvancePosition()
	{
		if (LoadNewByte())
		{
			return true;
		}
		return false;
	}

	private uint WhiteTerminatingCodeRunLength()
	{
		return CurValueBitsRead switch
		{
			4 => WhiteLen4TermCodes[Value], 
			5 => WhiteLen5TermCodes[Value], 
			6 => WhiteLen6TermCodes[Value], 
			7 => WhiteLen7TermCodes[Value], 
			8 => WhiteLen8TermCodes[Value], 
			_ => 0u, 
		};
	}

	private uint BlackTerminatingCodeRunLength()
	{
		return CurValueBitsRead switch
		{
			2 => BlackLen2TermCodes[Value], 
			3 => BlackLen3TermCodes[Value], 
			4 => BlackLen4TermCodes[Value], 
			5 => BlackLen5TermCodes[Value], 
			6 => BlackLen6TermCodes[Value], 
			7 => BlackLen7TermCodes[Value], 
			8 => BlackLen8TermCodes[Value], 
			9 => BlackLen9TermCodes[Value], 
			10 => BlackLen10TermCodes[Value], 
			11 => BlackLen11TermCodes[Value], 
			12 => BlackLen12TermCodes[Value], 
			_ => 0u, 
		};
	}

	private uint WhiteMakeupCodeRunLength()
	{
		return CurValueBitsRead switch
		{
			5 => WhiteLen5MakeupCodes[Value], 
			6 => WhiteLen6MakeupCodes[Value], 
			7 => WhiteLen7MakeupCodes[Value], 
			8 => WhiteLen8MakeupCodes[Value], 
			9 => WhiteLen9MakeupCodes[Value], 
			11 => WhiteLen11MakeupCodes[Value], 
			12 => WhiteLen12MakeupCodes[Value], 
			_ => 0u, 
		};
	}

	private uint BlackMakeupCodeRunLength()
	{
		return CurValueBitsRead switch
		{
			10 => BlackLen10MakeupCodes[Value], 
			11 => BlackLen11MakeupCodes[Value], 
			12 => BlackLen12MakeupCodes[Value], 
			13 => BlackLen13MakeupCodes[Value], 
			_ => 0u, 
		};
	}

	private bool IsMakeupCode()
	{
		if (IsWhiteRun)
		{
			return IsWhiteMakeupCode();
		}
		return IsBlackMakeupCode();
	}

	private bool IsWhiteMakeupCode()
	{
		return CurValueBitsRead switch
		{
			5 => WhiteLen5MakeupCodes.ContainsKey(Value), 
			6 => WhiteLen6MakeupCodes.ContainsKey(Value), 
			7 => WhiteLen7MakeupCodes.ContainsKey(Value), 
			8 => WhiteLen8MakeupCodes.ContainsKey(Value), 
			9 => WhiteLen9MakeupCodes.ContainsKey(Value), 
			11 => WhiteLen11MakeupCodes.ContainsKey(Value), 
			12 => WhiteLen12MakeupCodes.ContainsKey(Value), 
			_ => false, 
		};
	}

	private bool IsBlackMakeupCode()
	{
		return CurValueBitsRead switch
		{
			10 => BlackLen10MakeupCodes.ContainsKey(Value), 
			11 => BlackLen11MakeupCodes.ContainsKey(Value), 
			12 => BlackLen12MakeupCodes.ContainsKey(Value), 
			13 => BlackLen13MakeupCodes.ContainsKey(Value), 
			_ => false, 
		};
	}

	private bool IsTerminatingCode()
	{
		if (IsWhiteRun)
		{
			return IsWhiteTerminatingCode();
		}
		return IsBlackTerminatingCode();
	}

	private bool IsWhiteTerminatingCode()
	{
		return CurValueBitsRead switch
		{
			4 => WhiteLen4TermCodes.ContainsKey(Value), 
			5 => WhiteLen5TermCodes.ContainsKey(Value), 
			6 => WhiteLen6TermCodes.ContainsKey(Value), 
			7 => WhiteLen7TermCodes.ContainsKey(Value), 
			8 => WhiteLen8TermCodes.ContainsKey(Value), 
			_ => false, 
		};
	}

	private bool IsBlackTerminatingCode()
	{
		return CurValueBitsRead switch
		{
			2 => BlackLen2TermCodes.ContainsKey(Value), 
			3 => BlackLen3TermCodes.ContainsKey(Value), 
			4 => BlackLen4TermCodes.ContainsKey(Value), 
			5 => BlackLen5TermCodes.ContainsKey(Value), 
			6 => BlackLen6TermCodes.ContainsKey(Value), 
			7 => BlackLen7TermCodes.ContainsKey(Value), 
			8 => BlackLen8TermCodes.ContainsKey(Value), 
			9 => BlackLen9TermCodes.ContainsKey(Value), 
			10 => BlackLen10TermCodes.ContainsKey(Value), 
			11 => BlackLen11TermCodes.ContainsKey(Value), 
			12 => BlackLen12TermCodes.ContainsKey(Value), 
			_ => false, 
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint GetBit()
	{
		if (BitsRead >= 8)
		{
			AdvancePosition();
		}
		int num = 8 - BitsRead - 1;
		bool result = (DataAtPosition & (1 << num)) != 0;
		BitsRead++;
		return result ? 1u : 0u;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool LoadNewByte()
	{
		if (Position < (ulong)DataLength)
		{
			ReadNextByte();
			Position++;
			return true;
		}
		Position++;
		DataAtPosition = 0;
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void ReadNextByte()
	{
		int num = stream.ReadByte();
		if (num == -1)
		{
			TiffThrowHelper.ThrowImageFormatException("Tiff fax compression error: not enough data.");
		}
		ResetBitsRead();
		DataAtPosition = ((fillOrder == TiffFillOrder.LeastSignificantBitFirst) ? ReverseBits((byte)num) : ((byte)num));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte ReverseBits(byte b)
	{
		return (byte)((ulong)((((long)b * 2149582850L) & 0x884422110L) * 4311810305L) >> 32);
	}
}
