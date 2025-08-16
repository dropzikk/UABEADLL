using System.Diagnostics;

namespace SixLabors.ImageSharp.Formats.Tiff.Compression.Decompressors;

[DebuggerDisplay("Type = {Type}")]
internal readonly struct CcittTwoDimensionalCode
{
	private readonly ushort value;

	public CcittTwoDimensionalCodeType Type => (CcittTwoDimensionalCodeType)(value & 0xFF);

	public int Code { get; }

	public CcittTwoDimensionalCode(int code, CcittTwoDimensionalCodeType type, int bitsRequired, int extensionBits = 0)
	{
		Code = code;
		value = (ushort)((byte)type | ((bitsRequired & 0xF) << 8) | ((extensionBits & 7) << 11));
	}
}
