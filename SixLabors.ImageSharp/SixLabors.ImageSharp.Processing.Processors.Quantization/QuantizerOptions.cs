using SixLabors.ImageSharp.Processing.Processors.Dithering;

namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

public class QuantizerOptions
{
	private float ditherScale = 1f;

	private int maxColors = 256;

	public IDither? Dither { get; set; } = QuantizerConstants.DefaultDither;

	public float DitherScale
	{
		get
		{
			return ditherScale;
		}
		set
		{
			ditherScale = Numerics.Clamp(value, 0f, 1f);
		}
	}

	public int MaxColors
	{
		get
		{
			return maxColors;
		}
		set
		{
			maxColors = Numerics.Clamp(value, 1, 256);
		}
	}
}
