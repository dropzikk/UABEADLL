namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public sealed class HueProcessor : FilterProcessor
{
	public float Degrees { get; }

	public HueProcessor(float degrees)
		: base(KnownFilterMatrices.CreateHueFilter(degrees))
	{
		Degrees = degrees;
	}
}
