namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public sealed class GrayscaleBt601Processor : FilterProcessor
{
	public float Amount { get; }

	public GrayscaleBt601Processor(float amount)
		: base(KnownFilterMatrices.CreateGrayscaleBt601Filter(amount))
	{
		Amount = amount;
	}
}
