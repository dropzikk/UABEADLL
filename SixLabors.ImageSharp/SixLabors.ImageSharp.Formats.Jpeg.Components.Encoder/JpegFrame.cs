using System;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Encoder;

internal sealed class JpegFrame : IDisposable
{
	public JpegColorSpace ColorSpace { get; }

	public bool Interleaved { get; }

	public int PixelHeight { get; }

	public int PixelWidth { get; }

	public Component[] Components { get; }

	public int McusPerLine { get; }

	public int McusPerColumn { get; }

	public int BlocksPerMcu { get; }

	public JpegFrame(Image image, JpegFrameConfig frameConfig, bool interleaved)
	{
		ColorSpace = frameConfig.ColorType;
		Interleaved = interleaved;
		PixelWidth = image.Width;
		PixelHeight = image.Height;
		MemoryAllocator memoryAllocator = image.Configuration.MemoryAllocator;
		JpegComponentConfig[] components = frameConfig.Components;
		Components = new Component[components.Length];
		for (int i = 0; i < Components.Length; i++)
		{
			JpegComponentConfig jpegComponentConfig = components[i];
			Components[i] = new Component(memoryAllocator, jpegComponentConfig.HorizontalSampleFactor, jpegComponentConfig.VerticalSampleFactor, jpegComponentConfig.QuantizatioTableIndex)
			{
				DcTableId = jpegComponentConfig.DcTableSelector,
				AcTableId = jpegComponentConfig.AcTableSelector
			};
			BlocksPerMcu += jpegComponentConfig.HorizontalSampleFactor * jpegComponentConfig.VerticalSampleFactor;
		}
		int maxHorizontalSamplingFactor = frameConfig.MaxHorizontalSamplingFactor;
		int maxVerticalSamplingFactor = frameConfig.MaxVerticalSamplingFactor;
		McusPerLine = (int)Numerics.DivideCeil((uint)image.Width, (uint)(maxHorizontalSamplingFactor * 8));
		McusPerColumn = (int)Numerics.DivideCeil((uint)image.Height, (uint)(maxVerticalSamplingFactor * 8));
		for (int j = 0; j < Components.Length; j++)
		{
			Components[j].Init(this, maxHorizontalSamplingFactor, maxVerticalSamplingFactor);
		}
	}

	public void Dispose()
	{
		for (int i = 0; i < Components.Length; i++)
		{
			Components[i].Dispose();
		}
	}

	public void AllocateComponents(bool fullScan)
	{
		for (int i = 0; i < Components.Length; i++)
		{
			Components[i].AllocateSpectral(fullScan);
		}
	}
}
