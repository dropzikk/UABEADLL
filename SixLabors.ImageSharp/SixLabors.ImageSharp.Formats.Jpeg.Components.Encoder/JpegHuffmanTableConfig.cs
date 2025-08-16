namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Encoder;

internal class JpegHuffmanTableConfig
{
	public int Class { get; }

	public int DestinationIndex { get; }

	public HuffmanSpec Table { get; }

	public JpegHuffmanTableConfig(int @class, int destIndex, HuffmanSpec table)
	{
		Class = @class;
		DestinationIndex = destIndex;
		Table = table;
	}
}
