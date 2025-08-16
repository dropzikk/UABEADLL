namespace SixLabors.ImageSharp.Processing.Processors.Filters;

public sealed class DeuteranomalyProcessor : FilterProcessor
{
	public DeuteranomalyProcessor()
		: base(KnownFilterMatrices.DeuteranomalyFilter)
	{
	}
}
