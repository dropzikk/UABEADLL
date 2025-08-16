namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public sealed class GrayscaleBt709Processor : FilterProcessor
{
	public float Amount { get; }

	public GrayscaleBt709Processor(float amount)
		: base(KnownFilterMatrices.CreateGrayscaleBt709Filter(amount))
	{
		Amount = amount;
	}
}
