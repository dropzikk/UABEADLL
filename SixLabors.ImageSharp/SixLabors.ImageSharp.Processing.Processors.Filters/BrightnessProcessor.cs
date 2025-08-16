namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public sealed class BrightnessProcessor : FilterProcessor
{
	public float Amount { get; }

	public BrightnessProcessor(float amount)
		: base(KnownFilterMatrices.CreateBrightnessFilter(amount))
	{
		Amount = amount;
	}
}
