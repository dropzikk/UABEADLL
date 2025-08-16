namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Encoder;

internal class JpegComponentConfig
{
	public byte Id { get; }

	public int HorizontalSampleFactor { get; }

	public int VerticalSampleFactor { get; }

	public int QuantizatioTableIndex { get; }

	public int DcTableSelector { get; }

	public int AcTableSelector { get; }

	public JpegComponentConfig(byte id, int hsf, int vsf, int quantIndex, int dcIndex, int acIndex)
	{
		Id = id;
		HorizontalSampleFactor = hsf;
		VerticalSampleFactor = vsf;
		QuantizatioTableIndex = quantIndex;
		DcTableSelector = dcIndex;
		AcTableSelector = acIndex;
	}
}
