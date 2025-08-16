using System;

namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Encoder;

internal class JpegQuantizationTableConfig
{
	public int DestinationIndex { get; }

	public Block8x8 Table { get; }

	public JpegQuantizationTableConfig(int destIndex, ReadOnlySpan<byte> quantizationTable)
	{
		DestinationIndex = destIndex;
		Table = Block8x8.Load(quantizationTable);
	}
}
