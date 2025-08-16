namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public sealed class AchromatopsiaProcessor : FilterProcessor
{
	public AchromatopsiaProcessor()
		: base(KnownFilterMatrices.AchromatopsiaFilter)
	{
	}
}
