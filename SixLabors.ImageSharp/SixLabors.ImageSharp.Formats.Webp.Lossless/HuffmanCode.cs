using System.Diagnostics;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

[DebuggerDisplay("BitsUsed: {BitsUsed}, Value: {Value}")]
internal struct HuffmanCode
{
	public int BitsUsed { get; set; }

	public uint Value { get; set; }
}
