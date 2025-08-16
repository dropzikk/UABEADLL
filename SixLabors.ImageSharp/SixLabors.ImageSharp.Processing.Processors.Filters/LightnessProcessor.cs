namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public sealed class LightnessProcessor : FilterProcessor
{
	public float Amount { get; }

	public LightnessProcessor(float amount)
		: base(KnownFilterMatrices.CreateLightnessFilter(amount))
	{
		Amount = amount;
	}
}
