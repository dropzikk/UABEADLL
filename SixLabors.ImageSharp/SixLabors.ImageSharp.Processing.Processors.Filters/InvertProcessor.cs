namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public sealed class InvertProcessor : FilterProcessor
{
	public float Amount { get; }

	public InvertProcessor(float amount)
		: base(KnownFilterMatrices.CreateInvertFilter(amount))
	{
		Amount = amount;
	}
}
