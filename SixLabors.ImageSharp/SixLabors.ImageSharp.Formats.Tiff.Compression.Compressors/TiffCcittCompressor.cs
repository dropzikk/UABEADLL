using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Compressors;

internal abstract class TiffCcittCompressor : TiffBaseCompressor
{
	protected const uint WhiteZeroRunTermCode = 53u;

	protected const uint BlackZeroRunTermCode = 55u;

	private static readonly uint[] MakeupRunLength = new uint[40]
	{
		64u, 128u, 192u, 256u, 320u, 384u, 448u, 512u, 576u, 640u,
		704u, 768u, 832u, 896u, 960u, 1024u, 1088u, 1152u, 1216u, 1280u,
		1344u, 1408u, 1472u, 1536u, 1600u, 1664u, 1728u, 1792u, 1856u, 1920u,
		1984u, 2048u, 2112u, 2176u, 2240u, 2304u, 2368u, 2432u, 2496u, 2560u
	};

	private static readonly Dictionary<uint, uint> WhiteLen4TermCodes = new Dictionary<uint, uint>
	{
		{ 2u, 7u },
		{ 3u, 8u },
		{ 4u, 11u },
		{ 5u, 12u },
		{ 6u, 14u },
		{ 7u, 15u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen5TermCodes = new Dictionary<uint, uint>
	{
		{ 8u, 19u },
		{ 9u, 20u },
		{ 10u, 7u },
		{ 11u, 8u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen6TermCodes = new Dictionary<uint, uint>
	{
		{ 1u, 7u },
		{ 12u, 8u },
		{ 13u, 3u },
		{ 14u, 52u },
		{ 15u, 53u },
		{ 16u, 42u },
		{ 17u, 43u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen7TermCodes = new Dictionary<uint, uint>
	{
		{ 18u, 39u },
		{ 19u, 12u },
		{ 20u, 8u },
		{ 21u, 23u },
		{ 22u, 3u },
		{ 23u, 4u },
		{ 24u, 40u },
		{ 25u, 43u },
		{ 26u, 19u },
		{ 27u, 36u },
		{ 28u, 24u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen8TermCodes = new Dictionary<uint, uint>
	{
		{ 0u, 53u },
		{ 29u, 2u },
		{ 30u, 3u },
		{ 31u, 26u },
		{ 32u, 27u },
		{ 33u, 18u },
		{ 34u, 19u },
		{ 35u, 20u },
		{ 36u, 21u },
		{ 37u, 22u },
		{ 38u, 23u },
		{ 39u, 40u },
		{ 40u, 41u },
		{ 41u, 42u },
		{ 42u, 43u },
		{ 43u, 44u },
		{ 44u, 45u },
		{ 45u, 4u },
		{ 46u, 5u },
		{ 47u, 10u },
		{ 48u, 11u },
		{ 49u, 82u },
		{ 50u, 83u },
		{ 51u, 84u },
		{ 52u, 85u },
		{ 53u, 36u },
		{ 54u, 37u },
		{ 55u, 88u },
		{ 56u, 89u },
		{ 57u, 90u },
		{ 58u, 91u },
		{ 59u, 74u },
		{ 60u, 75u },
		{ 61u, 50u },
		{ 62u, 51u },
		{ 63u, 52u }
	};

	private static readonly Dictionary<uint, uint> BlackLen2TermCodes = new Dictionary<uint, uint>
	{
		{ 2u, 3u },
		{ 3u, 2u }
	};

	private static readonly Dictionary<uint, uint> BlackLen3TermCodes = new Dictionary<uint, uint>
	{
		{ 1u, 2u },
		{ 4u, 3u }
	};

	private static readonly Dictionary<uint, uint> BlackLen4TermCodes = new Dictionary<uint, uint>
	{
		{ 5u, 3u },
		{ 6u, 2u }
	};

	private static readonly Dictionary<uint, uint> BlackLen5TermCodes = new Dictionary<uint, uint> { { 7u, 3u } };

	private static readonly Dictionary<uint, uint> BlackLen6TermCodes = new Dictionary<uint, uint>
	{
		{ 8u, 5u },
		{ 9u, 4u }
	};

	private static readonly Dictionary<uint, uint> BlackLen7TermCodes = new Dictionary<uint, uint>
	{
		{ 10u, 4u },
		{ 11u, 5u },
		{ 12u, 7u }
	};

	private static readonly Dictionary<uint, uint> BlackLen8TermCodes = new Dictionary<uint, uint>
	{
		{ 13u, 4u },
		{ 14u, 7u }
	};

	private static readonly Dictionary<uint, uint> BlackLen9TermCodes = new Dictionary<uint, uint> { { 15u, 24u } };

	private static readonly Dictionary<uint, uint> BlackLen10TermCodes = new Dictionary<uint, uint>
	{
		{ 0u, 55u },
		{ 16u, 23u },
		{ 17u, 24u },
		{ 18u, 8u }
	};

	private static readonly Dictionary<uint, uint> BlackLen11TermCodes = new Dictionary<uint, uint>
	{
		{ 19u, 103u },
		{ 20u, 104u },
		{ 21u, 108u },
		{ 22u, 55u },
		{ 23u, 40u },
		{ 24u, 23u },
		{ 25u, 24u }
	};

	private static readonly Dictionary<uint, uint> BlackLen12TermCodes = new Dictionary<uint, uint>
	{
		{ 26u, 202u },
		{ 27u, 203u },
		{ 28u, 204u },
		{ 29u, 205u },
		{ 30u, 104u },
		{ 31u, 105u },
		{ 32u, 106u },
		{ 33u, 107u },
		{ 34u, 210u },
		{ 35u, 211u },
		{ 36u, 212u },
		{ 37u, 213u },
		{ 38u, 214u },
		{ 39u, 215u },
		{ 40u, 108u },
		{ 41u, 109u },
		{ 42u, 218u },
		{ 43u, 219u },
		{ 44u, 84u },
		{ 45u, 85u },
		{ 46u, 86u },
		{ 47u, 87u },
		{ 48u, 100u },
		{ 49u, 101u },
		{ 50u, 82u },
		{ 51u, 83u },
		{ 52u, 36u },
		{ 53u, 55u },
		{ 54u, 56u },
		{ 55u, 39u },
		{ 56u, 40u },
		{ 57u, 88u },
		{ 58u, 89u },
		{ 59u, 43u },
		{ 60u, 44u },
		{ 61u, 90u },
		{ 62u, 102u },
		{ 63u, 103u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen5MakeupCodes = new Dictionary<uint, uint>
	{
		{ 64u, 27u },
		{ 128u, 18u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen6MakeupCodes = new Dictionary<uint, uint>
	{
		{ 192u, 23u },
		{ 1664u, 24u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen8MakeupCodes = new Dictionary<uint, uint>
	{
		{ 320u, 54u },
		{ 384u, 55u },
		{ 448u, 100u },
		{ 512u, 101u },
		{ 576u, 104u },
		{ 640u, 103u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen7MakeupCodes = new Dictionary<uint, uint> { { 256u, 55u } };

	private static readonly Dictionary<uint, uint> WhiteLen9MakeupCodes = new Dictionary<uint, uint>
	{
		{ 704u, 204u },
		{ 768u, 205u },
		{ 832u, 210u },
		{ 896u, 211u },
		{ 960u, 212u },
		{ 1024u, 213u },
		{ 1088u, 214u },
		{ 1152u, 215u },
		{ 1216u, 216u },
		{ 1280u, 217u },
		{ 1344u, 218u },
		{ 1408u, 219u },
		{ 1472u, 152u },
		{ 1536u, 153u },
		{ 1600u, 154u },
		{ 1728u, 155u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen11MakeupCodes = new Dictionary<uint, uint>
	{
		{ 1792u, 8u },
		{ 1856u, 12u },
		{ 1920u, 13u }
	};

	private static readonly Dictionary<uint, uint> WhiteLen12MakeupCodes = new Dictionary<uint, uint>
	{
		{ 1984u, 18u },
		{ 2048u, 19u },
		{ 2112u, 20u },
		{ 2176u, 21u },
		{ 2240u, 22u },
		{ 2304u, 23u },
		{ 2368u, 28u },
		{ 2432u, 29u },
		{ 2496u, 30u },
		{ 2560u, 31u }
	};

	private static readonly Dictionary<uint, uint> BlackLen10MakeupCodes = new Dictionary<uint, uint> { { 64u, 15u } };

	private static readonly Dictionary<uint, uint> BlackLen11MakeupCodes = new Dictionary<uint, uint>
	{
		{ 1792u, 8u },
		{ 1856u, 12u },
		{ 1920u, 13u }
	};

	private static readonly Dictionary<uint, uint> BlackLen12MakeupCodes = new Dictionary<uint, uint>
	{
		{ 128u, 200u },
		{ 192u, 201u },
		{ 256u, 91u },
		{ 320u, 51u },
		{ 384u, 52u },
		{ 448u, 53u },
		{ 1984u, 18u },
		{ 2048u, 19u },
		{ 2112u, 20u },
		{ 2176u, 21u },
		{ 2240u, 22u },
		{ 2304u, 23u },
		{ 2368u, 28u },
		{ 2432u, 29u },
		{ 2496u, 30u },
		{ 2560u, 31u }
	};

	private static readonly Dictionary<uint, uint> BlackLen13MakeupCodes = new Dictionary<uint, uint>
	{
		{ 512u, 108u },
		{ 576u, 109u },
		{ 640u, 74u },
		{ 704u, 75u },
		{ 768u, 76u },
		{ 832u, 77u },
		{ 896u, 114u },
		{ 960u, 115u },
		{ 1024u, 116u },
		{ 1088u, 117u },
		{ 1152u, 118u },
		{ 1216u, 119u },
		{ 1280u, 82u },
		{ 1344u, 83u },
		{ 1408u, 84u },
		{ 1472u, 85u },
		{ 1536u, 90u },
		{ 1600u, 91u },
		{ 1664u, 100u },
		{ 1728u, 101u }
	};

	private int bytePosition;

	private byte bitPosition;

	private IMemoryOwner<byte> compressedDataBuffer;

	protected TiffCcittCompressor(Stream output, MemoryAllocator allocator, int width, int bitsPerPixel)
		: base(output, allocator, width, bitsPerPixel)
	{
		bytePosition = 0;
		bitPosition = 0;
	}

	private static uint GetWhiteMakeupCode(uint runLength, out uint codeLength)
	{
		codeLength = 0u;
		if (WhiteLen5MakeupCodes.TryGetValue(runLength, out var value))
		{
			codeLength = 5u;
			return value;
		}
		if (WhiteLen6MakeupCodes.TryGetValue(runLength, out value))
		{
			codeLength = 6u;
			return value;
		}
		if (WhiteLen7MakeupCodes.TryGetValue(runLength, out value))
		{
			codeLength = 7u;
			return value;
		}
		if (WhiteLen8MakeupCodes.TryGetValue(runLength, out value))
		{
			codeLength = 8u;
			return value;
		}
		if (WhiteLen9MakeupCodes.TryGetValue(runLength, out value))
		{
			codeLength = 9u;
			return value;
		}
		if (WhiteLen11MakeupCodes.TryGetValue(runLength, out value))
		{
			codeLength = 11u;
			return value;
		}
		if (WhiteLen12MakeupCodes.TryGetValue(runLength, out value))
		{
			codeLength = 12u;
			return value;
		}
		return 0u;
	}

	private static uint GetBlackMakeupCode(uint runLength, out uint codeLength)
	{
		codeLength = 0u;
		if (BlackLen10MakeupCodes.TryGetValue(runLength, out var value))
		{
			codeLength = 10u;
			return value;
		}
		if (BlackLen11MakeupCodes.TryGetValue(runLength, out value))
		{
			codeLength = 11u;
			return value;
		}
		if (BlackLen12MakeupCodes.TryGetValue(runLength, out value))
		{
			codeLength = 12u;
			return value;
		}
		if (BlackLen13MakeupCodes.TryGetValue(runLength, out value))
		{
			codeLength = 13u;
			return value;
		}
		return 0u;
	}

	private static uint GetWhiteTermCode(uint runLength, out uint codeLength)
	{
		codeLength = 0u;
		if (WhiteLen4TermCodes.TryGetValue(runLength, out var value))
		{
			codeLength = 4u;
			return value;
		}
		if (WhiteLen5TermCodes.TryGetValue(runLength, out value))
		{
			codeLength = 5u;
			return value;
		}
		if (WhiteLen6TermCodes.TryGetValue(runLength, out value))
		{
			codeLength = 6u;
			return value;
		}
		if (WhiteLen7TermCodes.TryGetValue(runLength, out value))
		{
			codeLength = 7u;
			return value;
		}
		if (WhiteLen8TermCodes.TryGetValue(runLength, out value))
		{
			codeLength = 8u;
			return value;
		}
		return 0u;
	}

	private static uint GetBlackTermCode(uint runLength, out uint codeLength)
	{
		codeLength = 0u;
		if (BlackLen2TermCodes.TryGetValue(runLength, out var value))
		{
			codeLength = 2u;
			return value;
		}
		if (BlackLen3TermCodes.TryGetValue(runLength, out value))
		{
			codeLength = 3u;
			return value;
		}
		if (BlackLen4TermCodes.TryGetValue(runLength, out value))
		{
			codeLength = 4u;
			return value;
		}
		if (BlackLen5TermCodes.TryGetValue(runLength, out value))
		{
			codeLength = 5u;
			return value;
		}
		if (BlackLen6TermCodes.TryGetValue(runLength, out value))
		{
			codeLength = 6u;
			return value;
		}
		if (BlackLen7TermCodes.TryGetValue(runLength, out value))
		{
			codeLength = 7u;
			return value;
		}
		if (BlackLen8TermCodes.TryGetValue(runLength, out value))
		{
			codeLength = 8u;
			return value;
		}
		if (BlackLen9TermCodes.TryGetValue(runLength, out value))
		{
			codeLength = 9u;
			return value;
		}
		if (BlackLen10TermCodes.TryGetValue(runLength, out value))
		{
			codeLength = 10u;
			return value;
		}
		if (BlackLen11TermCodes.TryGetValue(runLength, out value))
		{
			codeLength = 11u;
			return value;
		}
		if (BlackLen12TermCodes.TryGetValue(runLength, out value))
		{
			codeLength = 12u;
			return value;
		}
		return 0u;
	}

	protected static uint GetBestFittingMakeupRunLength(uint runLength)
	{
		for (int i = 0; i < MakeupRunLength.Length - 1; i++)
		{
			if (MakeupRunLength[i] <= runLength && MakeupRunLength[i + 1] > runLength)
			{
				return MakeupRunLength[i];
			}
		}
		return MakeupRunLength[^1];
	}

	protected static uint GetTermCode(uint runLength, out uint codeLength, bool isWhiteRun)
	{
		if (isWhiteRun)
		{
			return GetWhiteTermCode(runLength, out codeLength);
		}
		return GetBlackTermCode(runLength, out codeLength);
	}

	protected static uint GetMakeupCode(uint runLength, out uint codeLength, bool isWhiteRun)
	{
		if (isWhiteRun)
		{
			return GetWhiteMakeupCode(runLength, out codeLength);
		}
		return GetBlackMakeupCode(runLength, out codeLength);
	}

	protected void PadByte()
	{
		if (Numerics.Modulo8(bitPosition) != 0)
		{
			bytePosition++;
			bitPosition = 0;
		}
	}

	protected void WriteCode(uint codeLength, uint code, Span<byte> compressedData)
	{
		while (codeLength != 0)
		{
			int num = (int)codeLength;
			if ((code & (1 << num - 1)) != 0)
			{
				BitWriterUtils.WriteBit(compressedData, bytePosition, bitPosition);
			}
			else
			{
				BitWriterUtils.WriteZeroBit(compressedData, bytePosition, bitPosition);
			}
			bitPosition++;
			if (bitPosition == 8)
			{
				bytePosition++;
				bitPosition = 0;
			}
			codeLength--;
		}
	}

	public override void CompressStrip(Span<byte> rows, int height)
	{
		compressedDataBuffer.Clear();
		Span<byte> span = compressedDataBuffer.GetSpan();
		bytePosition = 0;
		bitPosition = 0;
		CompressStrip(rows, height, span);
		int num = ((bitPosition != 0) ? (bytePosition + 1) : bytePosition);
		base.Output.Write(span[..num]);
	}

	protected abstract void CompressStrip(Span<byte> pixelsAsGray, int height, Span<byte> compressedData);

	protected override void Dispose(bool disposing)
	{
		compressedDataBuffer?.Dispose();
	}

	public override void Initialize(int rowsPerStrip)
	{
		int length = base.Width * rowsPerStrip;
		compressedDataBuffer = base.Allocator.Allocate<byte>(length);
	}
}
