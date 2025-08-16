namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public sealed class OpacityProcessor : FilterProcessor
{
	public float Amount { get; }

	public OpacityProcessor(float amount)
		: base(KnownFilterMatrices.CreateOpacityFilter(amount))
	{
		Amount = amount;
	}
}
