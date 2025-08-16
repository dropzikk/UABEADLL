using System.Diagnostics;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

[DebuggerDisplay("Code = {Code}, ExtraBits = {ExtraBits}")]
internal class HuffmanTreeToken
{
	public byte Code { get; set; }

	public byte ExtraBits { get; set; }
}
