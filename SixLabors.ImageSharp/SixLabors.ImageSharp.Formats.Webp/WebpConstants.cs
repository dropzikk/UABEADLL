using System.Collections.Generic;

namespace SixLabors.ImageSharp.Formats.Webp;

internal static class WebpConstants
{
	public static readonly IEnumerable<string> FileExtensions = new string[1] { "webp" };

	public static readonly IEnumerable<string> MimeTypes = new string[1] { "image/webp" };

	public static readonly byte[] Vp8HeaderMagicBytes = new byte[3] { 157, 1, 42 };

	public const byte Vp8LHeaderMagicByte = 47;

	public static readonly byte[] RiffFourCc = new byte[4] { 82, 73, 70, 70 };

	public static readonly byte[] WebpHeader = new byte[4] { 87, 69, 66, 80 };

	public const string WebpFourCc = "WEBP";

	public const int Vp8LVersionBits = 3;

	public const int Vp8LImageSizeBits = 14;

	public const int Vp8FrameHeaderSize = 10;

	public const int ChunkHeaderSize = 8;

	public const int RiffHeaderSize = 12;

	public const int TagSize = 4;

	public const int Vp8LVersion = 0;

	public const int MaxHuffImageSize = 2600;

	public const int MinHuffmanBits = 2;

	public const int MaxHuffmanBits = 9;

	public const int MaxPaletteSize = 256;

	public const int MaxColorCacheBits = 10;

	public const int MaxNumberOfTransforms = 4;

	public const int MaxTransformBits = 6;

	public const int TransformPresent = 1;

	public const int MaxDimension = 16383;

	public const int MaxAllowedCodeLength = 15;

	public const int DefaultCodeLength = 8;

	public const int HuffmanCodesPerMetaCode = 5;

	public const uint ArgbBlack = 4278190080u;

	public const int NumArgbCacheRows = 16;

	public const int NumLiteralCodes = 256;

	public const int NumLengthCodes = 24;

	public const int NumDistanceCodes = 40;

	public const int CodeLengthCodes = 19;

	public const int LengthTableBits = 7;

	public const uint CodeLengthLiterals = 16u;

	public const int CodeLengthRepeatCode = 16;

	public static readonly int[] CodeLengthExtraBits = new int[3] { 2, 3, 7 };

	public static readonly int[] CodeLengthRepeatOffsets = new int[3] { 3, 3, 11 };

	public static readonly int[] AlphabetSize = new int[5] { 280, 256, 256, 256, 40 };

	public const int NumMbSegments = 4;

	public const int MaxNumPartitions = 8;

	public const int NumTypes = 4;

	public const int NumBands = 8;

	public const int NumProbas = 11;

	public const int NumPredModes = 4;

	public const int NumBModes = 10;

	public const int NumCtx = 3;

	public const int MaxVariableLevel = 67;

	public const int FlatnessLimitI16 = 0;

	public const int FlatnessLimitIUv = 2;

	public const int FlatnessLimitI4 = 3;

	public const int FlatnessPenality = 140;

	public const int Bps = 32;

	public const double Gamma = 0.8;

	public const int GammaFix = 12;

	public const int GammaScale = 4095;

	public const int GammaTabFix = 7;

	public const int GammaTabSize = 32;

	public const int GammaTabScale = 128;

	public const int GammaTabRounder = 64;

	public const int AlphaFix = 19;

	public const int MaxAlpha = 255;

	public const int AlphaScale = 510;

	public const int QuantEncMidAlpha = 64;

	public const int QuantEncMinAlpha = 30;

	public const int QuantEncMaxAlpha = 100;

	public const double SnsToDq = 0.9;

	public const int QuantEncMaxDqUv = 6;

	public const int QuantEncMinDqUv = -4;

	public const int QFix = 17;

	public const int MaxDelzaSize = 64;

	public const int FilterStrengthCutoff = 2;

	public const int Vp8MaxPartition0Size = 524288;

	public static readonly short[] Vp8FixedCostsUv = new short[4] { 302, 984, 439, 642 };

	public static readonly short[] Vp8FixedCostsI16 = new short[4] { 663, 919, 872, 919 };

	public const int RdDistoMult = 256;

	public static readonly byte[] FilterExtraRows = new byte[3] { 0, 2, 8 };

	public static readonly int[] Vp8EncBands = new int[17]
	{
		0, 1, 2, 3, 6, 4, 5, 6, 6, 6,
		6, 6, 6, 6, 6, 7, 0
	};

	public static readonly short[] Scan = new short[16]
	{
		0, 4, 8, 12, 128, 132, 136, 140, 256, 260,
		264, 268, 384, 388, 392, 396
	};

	public static readonly byte[] Cat3 = new byte[3] { 173, 148, 140 };

	public static readonly byte[] Cat4 = new byte[4] { 176, 155, 140, 135 };

	public static readonly byte[] Cat5 = new byte[5] { 180, 157, 141, 134, 130 };

	public static readonly byte[] Cat6 = new byte[11]
	{
		254, 254, 243, 230, 196, 177, 153, 140, 133, 130,
		129
	};

	public static readonly byte[] Zigzag = new byte[16]
	{
		0, 1, 4, 8, 5, 2, 3, 6, 9, 12,
		13, 10, 7, 11, 14, 15
	};

	public static readonly sbyte[] YModesIntra4 = new sbyte[18]
	{
		0, 1, -1, 2, -2, 3, 4, 6, -3, 5,
		-4, -5, -6, 7, -7, 8, -8, -9
	};
}
