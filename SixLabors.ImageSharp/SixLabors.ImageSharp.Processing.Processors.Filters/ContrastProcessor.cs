namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public sealed class ContrastProcessor : FilterProcessor
{
	public float Amount { get; }

	public ContrastProcessor(float amount)
		: base(KnownFilterMatrices.CreateContrastFilter(amount))
	{
		Amount = amount;
	}
}
