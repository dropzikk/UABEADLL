using System.Collections.Generic;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal struct HTreeGroup
{
	public List<HuffmanCode[]> HTrees { get; }

	public bool IsTrivialLiteral { get; set; }

	public uint LiteralArb { get; set; }

	public bool IsTrivialCode { get; set; }

	public bool UsePackedTable { get; set; }

	public HuffmanCode[] PackedTable { get; set; }

	public HTreeGroup(uint packedTableSize)
	{
		HTrees = new List<HuffmanCode[]>(5);
		PackedTable = new HuffmanCode[packedTableSize];
		IsTrivialCode = false;
		IsTrivialLiteral = false;
		LiteralArb = 0u;
		UsePackedTable = false;
	}
}
