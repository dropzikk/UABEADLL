using System;

namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Encoder;

internal class JpegFrameConfig
{
	public JpegColorSpace ColorType { get; }

	public JpegEncodingColor EncodingColor { get; }

	public JpegComponentConfig[] Components { get; }

	public JpegHuffmanTableConfig[] HuffmanTables { get; }

	public JpegQuantizationTableConfig[] QuantizationTables { get; }

	public int MaxHorizontalSamplingFactor { get; }

	public int MaxVerticalSamplingFactor { get; }

	public byte? AdobeColorTransformMarkerFlag { get; set; }

	public JpegFrameConfig(JpegColorSpace colorType, JpegEncodingColor encodingColor, JpegComponentConfig[] components, JpegHuffmanTableConfig[] huffmanTables, JpegQuantizationTableConfig[] quantTables)
	{
		ColorType = colorType;
		EncodingColor = encodingColor;
		Components = components;
		HuffmanTables = huffmanTables;
		QuantizationTables = quantTables;
		MaxHorizontalSamplingFactor = components[0].HorizontalSampleFactor;
		MaxVerticalSamplingFactor = components[0].VerticalSampleFactor;
		for (int i = 1; i < components.Length; i++)
		{
			JpegComponentConfig jpegComponentConfig = components[i];
			MaxHorizontalSamplingFactor = Math.Max(MaxHorizontalSamplingFactor, jpegComponentConfig.HorizontalSampleFactor);
			MaxVerticalSamplingFactor = Math.Max(MaxVerticalSamplingFactor, jpegComponentConfig.VerticalSampleFactor);
		}
	}
}
