namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public sealed class SepiaProcessor : FilterProcessor
{
	public float Amount { get; }

	public SepiaProcessor(float amount)
		: base(KnownFilterMatrices.CreateSepiaFilter(amount))
	{
		Amount = amount;
	}
}
