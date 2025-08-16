namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal struct HuffmanTreeCode
{
	public int NumSymbols { get; set; }

	public byte[] CodeLengths { get; set; }

	public short[] Codes { get; set; }
}
