namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public sealed class SaturateProcessor : FilterProcessor
{
	public float Amount { get; }

	public SaturateProcessor(float amount)
		: base(KnownFilterMatrices.CreateSaturateFilter(amount))
	{
		Amount = amount;
	}
}
