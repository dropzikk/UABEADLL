using SixLabors.ImageSharp.Processing.Processors.Dithering;

namespace SixLabors.ImageSharp.Processing.Processors.Quantization;

public static class QuantizerConstants
{
	public const int MinColors = 1;

	public const int MaxColors = 256;

	public const float MinDitherScale = 0f;

	public const float MaxDitherScale = 1f;

	public static IDither DefaultDither { get; } = KnownDitherings.FloydSteinberg;
}
